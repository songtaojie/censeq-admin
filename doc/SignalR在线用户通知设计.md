# SignalR 在线用户通知功能设计

> 适用范围：`censeq-admin-api`（ABP v8 / .NET 8）+ `censeq-admin-web`（Vue 3 + OIDC）
> 目标：在现有架构上以最小侵入方式集成 ASP.NET Core SignalR，支撑 **上线 / 下线广播、强制下线、站内通知、定向消息** 等服务器→客户端实时推送场景。

---

## 1. 需求背景

`censeq-admin` 目前所有交互均为前端主动 REST 调用，缺乏 **服务器主动通知** 能力，难以满足以下业务：

| 场景 | 描述 |
|------|------|
| 在线用户上线 / 下线提醒 | 管理员能实时看到“XX 上线 / 下线”，并维护当前在线用户列表。 |
| 强制下线 | 管理员可踢出指定连接，被踢端立即跳出登录。 |
| 站内通知 | 系统公告、单条消息、@某人，前端实时弹窗 / 红点。 |
| 业务事件推送 | 任务完成、订单状态变化等可订阅事件（后续可扩展）。 |

---

## 2. 设计目标与原则

1. **模块化**：在 `framework/` 下独立 `Censeq.Framework.SignalR` 模块，内部堆叠在 ABP 官方 `Volo.Abp.AspNetCore.SignalR` 之上；Host 只需 `DependsOn` 即可启用。
2. **复用 ABP 能力**：权限 / 多租户 / 动态 Claims / 审计 / `UserIdProvider` / Hub 自动 Map 全部走 ABP，框架模块只负责 **业务能力（在线表 + 通知发送）**。
3. **多租户**：连接按 `TenantId` 分组（Group），跨租户数据隔离。
4. **身份一致**：复用现有 OpenIddict + Bearer 认证管线，不再独立维护令牌。
5. **可水平扩展**：默认进程内 SignalR；多实例部署时开启 `StackExchangeRedis` Backplane 即可，不改业务代码。
6. **可观测**：Hub RPC 调用自动进入 ABP 审计日志（`AbpAuditHubFilter`）；业务推送额外写 Serilog。
7. **前端零侵入**：通过 `composables/useSignalR.ts` 统一封装，业务页面只用 `on/off/invoke`。
8. **与 `IdentitySession` 职责分离**（见 §2.1）：现有 `IdentitySession` 不承担"实时在线"语义，本方案与它互补而非取代。

### 2.1 与现有 `IdentitySession` 的关系

`censeq-admin` 现有 `Censeq.Identity` 模块里的 `IdentitySession` 容易被误解为"在线用户"，但二者本质不同。**`IdentitySession` 是登录会话凭证，不是实时连接**：

| 维度 | `IdentitySession`（ABP 现有） | SignalR 在线用户（本设计） |
|------|------------------------------|---------------------------|
| **本质** | 登录会话凭证：一次成功登录 = 一条记录 | 实时活动连接：一个浏览器标签页 = 一个 `ConnectionId` |
| **存储** | 数据库表 `IdentitySession`（持久化） | `IDistributedCache` / 内存（瞬态） |
| **写入** | `IdentitySessionManager.CreateAsync`（登录成功时） | `Hub.OnConnectedAsync` |
| **更新** | `IdentitySessionMiddleware` 每次 HTTP 请求刷新 `LastAccessed`（节流 1 分钟） | 连接期间天然存在 |
| **移除** | `IdentitySessionCleanupBackgroundWorker` 周期清理超时记录 | `OnDisconnectedAsync` 即时移除 |
| **离线感知精度** | **分钟级**，依赖前端持续发请求 | **秒级**，断开立即感知 |
| **通信方向** | 服务器被动记录，**无推送能力** | 服务器→客户端主动推送（上下线 / 强制下线 / 通知） |
| **关闭浏览器后** | 仍保留为"最后访问 5 分钟前"，看起来还在线 | WebSocket 心跳超时即从在线列表移除 |
| **强制下线** | 删 `IdentitySession` + Revoke Token，**下次请求才生效** | `Clients.Client(connId).ForceOffline()` **秒级**触发 |
| **历史记录** | 保留历史登录设备 | 仅当前活跃连接，不保留 |
| **跨实例共享** | DB 天然共享 | 默认单机；多副本需 Redis Backplane |

**举例**：用户登录后立即拔网线 / 关浏览器：
- `IdentitySession`：仍存在 ≥1 条记录，`LastAccessed` 显示几分钟前 → 系统认为"在线"。
- SignalR：WebSocket 心跳超时（默认 30 秒内）→ 立即广播"下线"。

**职责划分**：
- `IdentitySession` 继续承担 **登录设备列表 / 撤销 token / 我的活跃会话**（类似 GitHub Settings 里的 "Active sessions"）。
- SignalR 在线用户承担 **实时在线显示 / 实时推送 / 立即强制下线**。
- **联动点**：在 Hub `OnConnectedAsync` 时把 `AbpClaimTypes.SessionId` 一并存进 `OnlineUserInfo`，管理后台可同时展示「该用户的登录会话 → 当前活动 WebSocket 连接」。强制下线时双管齐下：先 `IdentitySessionManager.RevokeAsync`，再通过 SignalR 立即下发 `ForceOffline`。

---

## 3. 总体架构

```mermaid
flowchart LR
  subgraph Browser[censeq-admin-web]
    UI[业务页面]
    Hook[useSignalR.ts]
    OIDC[useOidc / accessToken]
  end

  subgraph Host[Censeq.Admin.HttpApi.Host]
    Pipe[OIDC Bearer 中间件]
    Hub1[/hubs/online-user/]
    Hub2[/hubs/notification/]
  end

  subgraph FW[Censeq.Framework.SignalR]
    Mod[CenseqSignalRModule]
    Reg[OnlineUserRegistry]
    Notifier[INotificationSender]
  end

  subgraph ABP[Volo.Abp.AspNetCore.SignalR]
    AbpMod[AbpAspNetCoreSignalRModule]
    AbpFilter[鉴权/审计/动态Claims HubFilter]
    AbpUid[AbpSignalRUserIdProvider]
  end

  Store[(IDistributedCache / Redis)]
  Backplane[(Redis Backplane*)]

  UI --> Hook --> Hub1
  Hook --> Hub2
  Hook -. access_token .- OIDC
  Hub1 --> Reg --> Store
  Hub2 --> Notifier
  Hub1 -. 多实例 .- Backplane
  Hub2 -. 多实例 .- Backplane
  Mod --> Hub1
  Mod --> Hub2
  Mod -. DependsOn .-> AbpMod
  AbpMod -. 自动 MapHub / 注入 .-> Hub1
  AbpMod -. 自动 MapHub / 注入 .-> Hub2
  AbpFilter -. 每次 RPC .- Hub1
  AbpFilter -. 每次 RPC .- Hub2
  AbpUid -. Clients.User(id) .- Hub2
```

*Backplane 默认不启用，单实例直接走进程内分发。*

---

## 4. 后端设计

### 4.1 项目与目录

新增框架模块（堆叠在 ABP 官方 SignalR 之上）：

```
censeq-admin-api/
  framework/
    Censeq.Framework.SignalR/
      Censeq.Framework.SignalR.csproj
      CenseqSignalRModule.cs                 # ABP 模块入口
      Censeq/
        Framework/
          SignalR/
            Hubs/
              OnlineUserHub.cs               # 在线用户 Hub（继承 AbpHub<T>）
              NotificationHub.cs             # 通知 Hub（继承 AbpHub<T>）
              IOnlineUserClient.cs           # 强类型客户端契约
              INotificationClient.cs
            Dto/
              OnlineUserInfo.cs
              OnlineUserChange.cs
              ForceOfflineInput.cs
              NotificationMessage.cs
            Services/
              IOnlineUserRegistry.cs
              OnlineUserRegistry.cs          # 默认基于 IDistributedCache
              INotificationSender.cs
              NotificationSender.cs
            Options/
              CenseqSignalROptions.cs
```

> **与初版设计的区别**：删除了原计划的 `AbpUserIdProvider.cs`、`Extensions/CenseqHubRouteAttribute.cs`、`Extensions/SignalRApplicationBuilderExtensions.cs`，这三个能力都由 `Volo.Abp.AspNetCore.SignalR` 内置提供：
>
> | 原自实现 | ABP 内置替代 |
> |---------|---------------|
> | 自写 `IUserIdProvider` | `AbpSignalRUserIdProvider`（默认绑定 `ICurrentUser.Id`） |
> | 自定义 `[CenseqHubRoute]` | `[HubRoute]` |
> | 反射扫描 `MapCenseqHubs` | `AbpAspNetCoreSignalRModule` 自动扫描所有 `Hub` 子类并注册到 `IEndpointRouteBuilder` |

引用关系：

* `Censeq.Framework.SignalR` 依赖
  `Volo.Abp.AspNetCore.SignalR`、`Volo.Abp.Caching`、`Volo.Abp.MultiTenancy`、`Censeq.Framework.AspNetCore`。
  > `Microsoft.AspNetCore.SignalR` 不需要显式引用，由 ABP 包传递引入。
  > Redis Backplane 如需在本模块中启用，还需按需增加 `Microsoft.AspNetCore.SignalR.StackExchangeRedis`。
* `Censeq.Admin.HttpApi.Host.csproj` 增加 `<ProjectReference Include="..\..\framework\Censeq.Framework.SignalR\Censeq.Framework.SignalR.csproj" />`。
* Host 模块只需 `[DependsOn(typeof(CenseqSignalRModule))]`；ABP SignalR 模块由 `CenseqSignalRModule` 内部 `DependsOn` 带入。

### 4.2 强类型 Hub 契约

```csharp
// Hubs/IOnlineUserClient.cs
public interface IOnlineUserClient
{
    Task OnlineChanged(OnlineUserChange change);          // 上下线广播
    Task OnlineList(IReadOnlyList<OnlineUserInfo> list);  // 全量在线列表
    Task ForceOffline(string reason);                     // 被强制下线
}

// Hubs/INotificationClient.cs
public interface INotificationClient
{
    Task ReceiveMessage(NotificationMessage message);     // 通知 / 公告 / @我
}
```

### 4.3 OnlineUserHub（关键逻辑）

```csharp
[Authorize]
[HubRoute("/hubs/online-user")]   // ABP 自带特性，AbpAspNetCoreSignalRModule 启动时自动 MapHub
public class OnlineUserHub : AbpHub<IOnlineUserClient>
{
    public const string TenantGroupPrefix = "tenant:";

    private readonly IOnlineUserRegistry _registry;
    private readonly IHubContext<OnlineUserHub, IOnlineUserClient> _hub;

    // CurrentUser / CurrentTenant / Logger / L 由 AbpHub 基类提供，无需构造注入
    public OnlineUserHub(
        IOnlineUserRegistry registry,
        IHubContext<OnlineUserHub, IOnlineUserClient> hub)
    {
        _registry = registry;
        _hub      = hub;
    }

    public override async Task OnConnectedAsync()
    {
        if (!CurrentUser.IsAuthenticated) { Context.Abort(); return; }

        var http = Context.GetHttpContext()!;
        var info = new OnlineUserInfo
        {
            ConnectionId = Context.ConnectionId,
            UserId       = CurrentUser.Id!.Value,
            UserName     = CurrentUser.UserName ?? string.Empty,
            Name         = CurrentUser.Name,
            TenantId     = CurrentTenant.Id,
            SessionId    = CurrentUser.FindClaim(AbpClaimTypes.SessionId)?.Value, // 关联 IdentitySession
            Ip           = http.GetClientIp(),
            UserAgent    = http.Request.Headers.UserAgent.ToString(),
            ConnectedAt  = DateTimeOffset.UtcNow
        };

        await _registry.AddAsync(info);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupOfTenant(info.TenantId));

        await _hub.Clients
            .Group(GroupOfTenant(info.TenantId))
            .OnlineChanged(new OnlineUserChange(info, Online: true));

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var info = await _registry.RemoveByConnectionAsync(Context.ConnectionId);
        if (info is not null)
        {
            await _hub.Clients
                .Group(GroupOfTenant(info.TenantId))
                .OnlineChanged(new OnlineUserChange(info, Online: false));
        }
        await base.OnDisconnectedAsync(exception);
    }

    // 由前端主动拉取当前在线列表
    public async Task<IReadOnlyList<OnlineUserInfo>> GetOnlineList()
        => await _registry.GetByTenantAsync(CurrentTenant.Id);

    // 管理员强制下线（需权限校验）
    [Authorize("AbpIdentity.Users.ForceLogout")] // 名称示例，按实际权限定义
    public async Task ForceOffline(ForceOfflineInput input)
    {
        await _hub.Clients.Client(input.ConnectionId)
            .ForceOffline(input.Reason ?? "您已被管理员强制下线");
    }

    private static string GroupOfTenant(Guid? tenantId)
        => $"{TenantGroupPrefix}{tenantId?.ToString() ?? "host"}";
}
```

要点：

* **继承 `AbpHub<IOnlineUserClient>`**，直接从基类的 `LazyServiceProvider` 访问 `CurrentUser` / `CurrentTenant` / `Logger` / `L`，不再手工构造注入。
* `[HubRoute("/hubs/online-user")]` 使用 ABP 原生特性；ABP 模块启动时会反射扫描所有 `Hub` 子类并自动 MapHub（不需要谁手动调 `endpoints.MapHub<T>`）。
* `[Authorize]` 拒绝未认证连接；`[Authorize("PermissionName")]` 与 Controller 一致。
* 多租户隔离用 **Group**；`null` 租户（Host）单独成组。
* **在线状态不持久化到数据库**：属高频易变数据，统一放 `IDistributedCache`（开发期 In-Memory，生产期可挂 Redis，与项目现有缓存策略一致）。
* `AbpAuthenticationHubFilter` 会在每次 RPC 调用时重新安装 `ICurrentPrincipalAccessor`，避免长连接中 `CurrentUser/CurrentTenant` 串号。
* `ForceOffline` 仅做下行通知。动态 Claims 被撤销后，ABP 模块在下一次 RPC 会自动 `Abort` 该连接（默认 5 秒节流），形成双重保障（见 §4.9）。

### 4.4 OnlineUserRegistry（在线用户存储）

```csharp
public interface IOnlineUserRegistry
{
    Task AddAsync(OnlineUserInfo info);
    Task<OnlineUserInfo?> RemoveByConnectionAsync(string connectionId);
    Task<IReadOnlyList<OnlineUserInfo>> GetByTenantAsync(Guid? tenantId);
    Task<IReadOnlyList<string>> GetConnectionIdsByUserAsync(Guid userId);
}
```

默认实现 `OnlineUserRegistry` 使用 ABP `IDistributedCache<TenantOnlineSet>`：

* Key：`signalr:online:tenant:{tenantId|host}` → `HashSet<OnlineUserInfo>`
* Key：`signalr:online:conn:{connectionId}` → `OnlineUserInfo`（反查用）
* 写入 / 删除均带短 TTL（如 24h）作为兜底，避免异常掉线残留。
* 单实例 In-Memory 时即为本地字典；多实例切 Redis 后天然共享。

### 4.5 NotificationHub & 通知发送

```csharp
[Authorize]
[HubRoute("/hubs/notification")]
public class NotificationHub : AbpHub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, OnlineUserHub.GroupOfTenant(CurrentTenant.Id));
        await base.OnConnectedAsync();
    }
}

public interface INotificationSender   // 业务侧调用入口
{
    Task SendToAllAsync(NotificationMessage msg, CancellationToken ct = default);
    Task SendToTenantAsync(Guid? tenantId, NotificationMessage msg, CancellationToken ct = default);
    Task SendToUserAsync(Guid userId, NotificationMessage msg, CancellationToken ct = default);
    Task SendToUsersAsync(IEnumerable<Guid> userIds, NotificationMessage msg, CancellationToken ct = default);
}
```

实现要点：

* `NotificationHub` 同样继承 `AbpHub<INotificationClient>`，复用 `CurrentTenant` 将连接加入租户分组。
* `IHubContext<NotificationHub, INotificationClient>` 注入后，按用户 ID 调用 `Clients.User(userId.ToString())`。
* 不再自定义 `IUserIdProvider`。ABP 官方 `AbpSignalRUserIdProvider` 会把 SignalR `UserIdentifier` 绑定到 `ICurrentUser.Id`，`Clients.User(id)` 可直接使用。
* 需要按租户发送时，使用 `Clients.Group(OnlineUserHub.GroupOfTenant(tenantId))`；需要全站广播时，使用 `Clients.All`。
* 业务层（如 `AdminAppService`）通过构造注入 `INotificationSender`，**不直接依赖 SignalR 类型**。
* 如业务层需要知道“当前 Hub 调用来自哪个连接”，可注入 ABP 官方 `IAbpHubContextAccessor` 获取当前 Hub 上下文，不需要自行把 `ConnectionId` 写入 `HttpContext.Items`。

### 4.6 模块注册（CenseqSignalRModule）

```csharp
[DependsOn(
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpCachingModule),
    typeof(AbpMultiTenancyModule)
)]
public class CenseqSignalRModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var cfg = context.Services.GetConfiguration();
        var options = cfg.GetSection("SignalR").Get<CenseqSignalROptions>() ?? new();

        Configure<HubOptions>(o =>
        {
            o.EnableDetailedErrors = options.EnableDetailedErrors;
            o.KeepAliveInterval = TimeSpan.FromSeconds(options.KeepAliveSeconds);
            o.ClientTimeoutInterval = TimeSpan.FromSeconds(options.ClientTimeoutSeconds);
        });

        if (!string.IsNullOrWhiteSpace(options.RedisConnectionString))
        {
            PreConfigure<ISignalRServerBuilder>(builder =>
            {
                builder.AddStackExchangeRedis(options.RedisConnectionString, redis =>
                {
                    redis.Configuration.ChannelPrefix = new RedisChannel(
                        options.RedisChannelPrefix ?? "censeq:signalr",
                        RedisChannel.PatternMode.Literal);
                });
            });
        }

        context.Services.AddSingleton<IOnlineUserRegistry, OnlineUserRegistry>();
        context.Services.AddTransient<INotificationSender, NotificationSender>();
    }
}
```

模块注册口径：

* `CenseqSignalRModule` 依赖 `AbpAspNetCoreSignalRModule`，不再直接依赖 `AbpAspNetCoreModule`。
* 不再调用 `services.AddSignalR(...)`。ABP SignalR 模块会创建 `ISignalRServerBuilder`，业务模块通过 `Configure<HubOptions>` 和 `PreConfigure<ISignalRServerBuilder>` 追加配置。
* 不再注册自定义 `IUserIdProvider`，ABP 默认实现已经满足 `Clients.User(userId)`。
* 不再实现 `OnApplicationInitialization` 调 `MapCenseqHubs()`。ABP 会在 `UseConfiguredEndpoints` 阶段自动扫描 Hub 并 Map；显式路由由 `[HubRoute]` 控制。
* 只保留 `IOnlineUserRegistry`、`INotificationSender` 等业务服务注册。
* OpenIddict WebSocket query token 适配放在 Host 模块中，框架模块保持纯净，不感知认证实现细节。

### 4.7 与 Host 的集成

`CenseqHttpApiHostModule` 需要：

1. `[DependsOn(typeof(CenseqSignalRModule))]`。
2. 在 `appsettings.json` 增加：

   ```jsonc
   "SignalR": {
     "EnableDetailedErrors": false,
     "KeepAliveSeconds": 15,
     "ClientTimeoutSeconds": 30,
     "RedisConnectionString": null,        // 多实例部署时填写
     "RedisChannelPrefix": "censeq:signalr"
   },
   "App": {
     // 已有
     "CorsOrigins": "http://localhost:4200,https://localhost:5001"
   },
   "Cors": {
     "EnabledSignalR": true                // 触发 Censeq 框架的 SignalR CORS 分支
   }
   ```

3. 由于使用 **OpenIddict Validation**，在 Host 模块中追加 WebSocket query token 读取逻辑：

   ```csharp
   PreConfigure<OpenIddictValidationBuilder>(b =>
   {
       b.UseAspNetCore().Configure(o =>
       {
           o.Events.OnMessageReceived = ctx =>
           {
               var path = ctx.HttpContext.Request.Path;
               var token = ctx.Request.Query["access_token"].ToString();
               if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs"))
                   ctx.Token = token;
               return Task.CompletedTask;
           };
       });
   });
   ```

4. 中间件顺序保持 `UseRouting → UseCenseqCors → UseAuthentication → UseAbpOpenIddictValidation → UseAuthorization → UseConfiguredEndpoints`，SignalR Hub 通过 ABP 在 `UseConfiguredEndpoints` 阶段自动 Map，无需额外 `endpoints.MapHub<T>()`。

### 4.8 安全与多租户

| 维度 | 策略 |
|------|------|
| 认证 | `[Authorize]` + 复用 OpenIddict Bearer；WebSocket 协议无法带 Header 时从 `?access_token=` 取。 |
| 授权 | `ForceOffline` 等敏感方法用 `[Authorize("PermissionName")]`，权限名沿用 `Censeq.Admin.Application.Contracts/Permissions`。 |
| 多租户 | `ICurrentTenant.Id` 决定 Group；跨租户调用必须经 `using (_currentTenant.Change(tid)) { ... }` 切换。 |
| 防滥用 | Hub 方法仅暴露必要 RPC；前端公告/群发统一走 REST + `INotificationSender`，不允许客户端直接调用 Hub 的群发方法。 |
| 日志 | Hub RPC 复用 ABP 审计日志；`OnConnected/OnDisconnected/ForceOffline/Send*` 额外走 Serilog，附 `userId/tenantId/connectionId`。 |

### 4.9 动态 Claims 与自动失效

项目 Host 模块已开启 `IsDynamicClaimsEnabled = true`。集成 `Volo.Abp.AspNetCore.SignalR` 后，ABP 的 `AbpAuthenticationHubFilter` 会在 Hub RPC 调用前处理认证上下文：

* 将 `Context.User` 写入 `ICurrentPrincipalAccessor`，保证 `CurrentUser` / `CurrentTenant` 在 Hub 方法调用期间始终正确。
* 按 `AbpSignalROptions.CheckDynamicClaimsInterval` 节流检查动态 Claims，默认约 5 秒。
* 当用户权限、角色、Claims 或会话状态被撤销后，动态 Claims 失效会导致连接被 `Abort()`。
* 因此强制下线采用双层防护：
  1. **主动通道**：管理员点击强制下线，服务端调用 `Clients.Client(connectionId).ForceOffline(reason)`，前端立即登出。
  2. **兜底通道**：权限 / 会话撤销后，即使前端未处理 `ForceOffline`，后续 Hub 调用也会被 ABP 动态 Claims 机制中止。

前端 `useSignalR` 需要在 `connection.onclose` 中识别认证失败或连接被服务端中止的情况，触发 OIDC 登出或跳转登录页，避免用户停留在已失效页面。

### 4.10 自动 Map 与内部 Hub 约束

ABP SignalR 模块会自动扫描并映射所有未标记 `[DisableAutoHubMap]` 的 Hub 子类：

* 对外 Hub 必须显式标注 `[HubRoute("/hubs/...")]`，保持项目统一的 `/hubs` 路由风格。
* 临时内部 Hub、测试 Hub 或灰度期间不希望暴露的 Hub，必须标注 `[DisableAutoHubMap]`。
* 不使用 ABP 默认 `/signalr-hubs/{hub-kebab-name}` 路由，除非后续团队统一切换路由规范；当前方案保留 `/hubs/online-user`、`/hubs/notification`，减少前端和反向代理调整成本。

---

## 5. 前端设计（`censeq-admin-web`）

### 5.1 依赖

```bash
yarn add @microsoft/signalr
```

> 选 `@microsoft/signalr` 官方包，自动协商 WebSocket / SSE / LongPolling。

### 5.2 目录与文件

```
src/
  api/
    models/
      signalr/
        OnlineUserInfo.ts
        OnlineUserChange.ts
        NotificationMessage.ts
  composables/
    useSignalR.ts            # 通用连接管理
    useOnlineUserHub.ts      # 在线用户 Hub 封装
    useNotificationHub.ts    # 通知 Hub 封装
  stores/
    onlineUser.ts            # Pinia：当前在线列表
    notification.ts          # Pinia：未读消息
  views/
    system/
      online-user/
        index.vue            # 在线用户管理页（含强制下线按钮）
```

### 5.3 通用连接封装 `useSignalR.ts`

```ts
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { useOidc } from '@/composables/useOidc';

interface Options {
  hubUrl: string;          // 例如 '/hubs/online-user'
  automaticReconnect?: number[];
}

export function useSignalR({ hubUrl, automaticReconnect = [0, 2000, 5000, 10000, 30000] }: Options) {
  const { getAcessToken } = useOidc();
  const baseUrl = import.meta.env.VITE_API_URL.replace(/\/+$/, '');
  let connection: HubConnection | null = null;

  async function start() {
    if (connection?.state === HubConnectionState.Connected) return connection;
    connection = new HubConnectionBuilder()
      .withUrl(`${baseUrl}${hubUrl}`, {
        accessTokenFactory: async () => (await getAcessToken()) ?? ''
      })
      .withAutomaticReconnect(automaticReconnect)
      .configureLogging(LogLevel.Warning)
      .build();
    await connection.start();
    return connection;
  }

  async function stop() {
    if (connection && connection.state !== HubConnectionState.Disconnected) {
      await connection.stop();
    }
    connection = null;
  }

  function on<T = unknown>(event: string, handler: (payload: T) => void) {
    connection?.on(event, handler as (...args: unknown[]) => void);
  }
  function off(event: string) {
    connection?.off(event);
  }
  async function invoke<TResult = void>(method: string, ...args: unknown[]) {
    if (!connection) await start();
    return (await connection!.invoke(method, ...args)) as TResult;
  }

  return { start, stop, on, off, invoke };
}
```

### 5.4 在线用户 Hub 封装

```ts
// composables/useOnlineUserHub.ts
import { useSignalR } from './useSignalR';
import { useOnlineUserStore } from '@/stores/onlineUser';
import type { OnlineUserChange, OnlineUserInfo } from '@/api/models/signalr/OnlineUserChange';

const hub = useSignalR({ hubUrl: '/hubs/online-user' });
const store = useOnlineUserStore();

export async function startOnlineUserHub() {
  await hub.start();
  hub.on<OnlineUserChange>('OnlineChanged', (c) => store.applyChange(c));
  hub.on<OnlineUserInfo[]>('OnlineList',    (l) => store.replaceAll(l));
  hub.on<string>          ('ForceOffline',  (reason) => store.handleForceOffline(reason));
  const list = await hub.invoke<OnlineUserInfo[]>('GetOnlineList');
  store.replaceAll(list);
}

export async function stopOnlineUserHub() {
  await hub.stop();
}

export async function forceOffline(connectionId: string, reason?: string) {
  await hub.invoke('ForceOffline', { connectionId, reason });
}
```

### 5.5 启动时机与生命周期

* **登录成功后**：在 `App.vue` 或 `useOidc` 的 `onLoginSuccess` 回调中调用 `startOnlineUserHub()` / `startNotificationHub()`。
* **登出 / token 失效**：在 OIDC 登出钩子中调用 `stop*Hub()`，避免持有过期连接重连。
* **`ForceOffline` 客户端处理**：

  ```ts
  // stores/onlineUser.ts
  async function handleForceOffline(reason: string) {
    ElMessageBox.alert(reason ?? '您已被管理员强制下线', '提示', { type: 'warning' });
    await useOidc().logout();   // 立刻走 OIDC 登出
  }
  ```

* **断线重连**：`withAutomaticReconnect`；超过最大间隔仍未恢复，弹出“与服务器失去连接”提示并允许手动重试。

### 5.6 在线用户管理页（视图）

* 路由：`/system/online-user`，权限点位 `Admin.System.OnlineUser`（在后端 `Permissions` 中新增）。
* 表格列：用户名、姓名、IP、浏览器、登录时间、操作（强制下线）。
* 实时刷新：订阅 `OnlineChanged` 事件即时增删；不再轮询。

---

## 6. 跨域与反向代理

| 部署位置 | 关键点 |
|----------|--------|
| 开发：Vite Proxy | `vite.config.ts` 已经把 `/api` 代理到后端；新增 `/hubs` 代理，**必须开启 `ws: true`** 才能升级 WebSocket。 |
| 生产：Nginx | 在 `docker/nginx/conf/*.conf` 中：<br>`location /hubs/ { proxy_pass http://api; proxy_http_version 1.1; proxy_set_header Upgrade $http_upgrade; proxy_set_header Connection "upgrade"; proxy_read_timeout 1h; }` |
| CORS | 框架的 `CenseqCors` 已经识别 `EnabledSignalR=true`，会自动允许 `AllowCredentials` + 指定来源；**禁止 `AllowAnyOrigin`**，否则 WebSocket 升级失败。 |

---

## 7. 水平扩展（Backplane）

* 单实例部署：默认不启用。
* 多实例部署（K8s / Compose 多副本）：
  1. 在 `appsettings.json` 配置 `SignalR.RedisConnectionString`（与 ABP 现有 Redis 同库不同 DB 即可）。
  2. 启用 **Sticky Session** 或确保 LB 支持 WebSocket 升级。
  3. `IOnlineUserRegistry` 切换为 Redis 实现后，全集群共享在线列表。

---

## 8. 可观测性与运维

| 项 | 实现 |
|----|------|
| 连接计数 | 暴露内部 `HubLifetimeManager` 统计到 `/metrics`（与现有 Serilog/Prometheus 配套）。 |
| 慢调用 | 在 `Send*` 实现内打 `Activity`，对接 OpenTelemetry（与项目现有 Tracing 一致）。 |
| 异常 | Hub 内 try/catch → `_logger.LogError`；前端拦截 `connection.onclose` 上报。 |
| 健康检查 | 已有 `/health` 端点；如启用 Redis Backplane，加 `AddRedis()` 检查。 |

---

## 9. 测试策略

| 层级 | 内容 |
|------|------|
| 单元测试 | `OnlineUserRegistry` 的增删查；`NotificationSender` 的目标解析（用户 / 租户 / 全体）。 |
| 集成测试 | `Microsoft.AspNetCore.SignalR.Client` + `WebApplicationFactory` 启动 Host，模拟两个客户端验证 **上线广播**、**强制下线**、**跨租户隔离**。 |
| 前端测试 | 用 `vitest` mock `HubConnection`，验证 store 对事件的响应；E2E 用 Playwright 跑“两个标签页互相感知上下线”。 |
| 压测 | 用 `crankier` / `k6 ws` 模拟 N 个并发连接，验证 KeepAlive、Backplane 容量。 |

---

## 10. 实施计划（建议拆分）

| 阶段 | 交付物 | 备注 |
|------|--------|------|
| Phase 1 框架落地 | `Censeq.Framework.SignalR` 模块 + Host 集成 + `/hubs/online-user` Hub | 不含业务页；走联调验证连接/认证/分组。 |
| Phase 2 在线用户 | `IOnlineUserRegistry`（内存版）+ 前端 `useOnlineUserHub` + 在线用户管理页 + `ForceOffline` 权限 | 单实例完整闭环。 |
| Phase 3 通知 | `/hubs/notification` + `INotificationSender` + 业务事件接入（如租户公告） | 与业务模块结合。 |
| Phase 4 扩展 | 切换 Redis Registry + Redis Backplane + 多副本部署演练 | 上生产前完成。 |
| Phase 5 观测/治理 | 指标、Tracing、限频策略、滥用防护 | 持续优化。 |

---

## 11. 关键设计取舍

| 维度 | 取舍与理由 |
|------|-----------|
| 模块组织 | 在 `framework/` 下独立成 `Censeq.Framework.SignalR` 模块；Host 通过 `DependsOn` 启用，便于复用、替换与单测。 |
| Hub 注册方式 | 使用 ABP 官方 `[HubRoute]` + `AbpAspNetCoreSignalRModule` 自动 Map；避免自写扫描逻辑，并获得官方 HubFilter、审计、动态 Claims 与 `UserIdProvider`。 |
| 在线状态存储 | 选 `IDistributedCache`，不进数据库。在线/离线属高频易变事件，写库会带来无谓的 IO 与表膨胀；与项目现有缓存策略一致，单机 In-Memory、多机 Redis 无缝切换。 |
| 多租户 | 复用 `ICurrentTenant.Id` 作为 Group 维度，全程处在 ABP 多租户上下文，避免手工解析 Claim 与跨租户串号。 |
| 鉴权 | 复用 OpenIddict Validation + Bearer；为 WebSocket 适配 `?access_token=` 取 token，无需另起一套票据。 |
| 动态 Claims | 复用 ABP `AbpAuthenticationHubFilter`，权限 / 角色 / 会话失效后自动中止连接；`ForceOffline` 只负责立即提醒和前端登出。 |
| 契约分层 | 将 **Client 契约**（`IOnlineUserClient` / `INotificationClient`）与 **Server Hub** 拆开，类型清晰，便于前端按事件名强类型订阅。 |
| 推送入口 | 业务层只依赖 `INotificationSender`，不直接持有 `IHubContext`，便于单元测试与未来更换实现（如改走 MQ）。 |
| 与 `IdentitySession` | 不重叠、不替代：会话凭证持久化交给 `IdentitySession`，实时连接交给 SignalR；强制下线时双管齐下。 |

---

*本文档随 Phase 落地同步更新；接口名、权限点、配置键以最终实现为准。*

