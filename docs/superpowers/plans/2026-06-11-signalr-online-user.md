# SignalR Online User Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a single-instance SignalR online user and notification foundation for the ABP API host and Vue admin frontend.

**Architecture:** Create a reusable `Censeq.Framework.SignalR` ABP module that owns hubs, client contracts, online connection registry, and notification sender abstractions. The API host enables the module and OpenIddict access-token handling for `/hubs`, while the Vue app uses a small SignalR composable plus Pinia stores and an online-user management view.

**Tech Stack:** .NET 8, ABP v8, ASP.NET Core SignalR, OpenIddict Validation, Vue 3, Pinia, Element Plus, `@microsoft/signalr`.

---

### Task 1: Backend Registry Contract

**Files:**
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq.Framework.SignalR.csproj`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Dto/OnlineUserInfo.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Dto/OnlineUserChange.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Dto/ForceOfflineInput.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Services/IOnlineUserRegistry.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Services/OnlineUserRegistry.cs`

- [ ] **Step 1: Write failing registry tests**

Create a small console-free xUnit test project if no framework test project exists for the new module. Tests must cover add, tenant filtering, connection removal, and user lookup.

- [ ] **Step 2: Run registry tests and verify RED**

Run: `dotnet test censeq-admin-api/framework/Censeq.Framework.SignalR.Tests/Censeq.Framework.SignalR.Tests.csproj`
Expected: FAIL because `Censeq.Framework.SignalR` types do not exist yet.

- [ ] **Step 3: Implement minimal registry**

Use ABP `IDistributedCache<TenantOnlineUsersCacheItem>` and `IDistributedCache<OnlineUserInfo>` with cache keys:
`signalr:online:tenant:{tenantId|host}`, `signalr:online:conn:{connectionId}`.

- [ ] **Step 4: Run registry tests and verify GREEN**

Run: `dotnet test censeq-admin-api/framework/Censeq.Framework.SignalR.Tests/Censeq.Framework.SignalR.Tests.csproj`
Expected: PASS.

### Task 2: Backend Hubs And Module

**Files:**
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/CenseqSignalRModule.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Hubs/IOnlineUserClient.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Hubs/INotificationClient.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Hubs/OnlineUserHub.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Hubs/NotificationHub.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Dto/NotificationMessage.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Services/INotificationSender.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Services/NotificationSender.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Options/CenseqSignalROptions.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Extensions/SignalRApplicationBuilderExtensions.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/Extensions/CenseqHubRouteAttribute.cs`
- Create: `censeq-admin-api/framework/Censeq.Framework.SignalR/Censeq/Framework/SignalR/AbpUserIdProvider.cs`

- [ ] **Step 1: Write failing module build test**

Add a test that resolves `IOnlineUserRegistry`, `INotificationSender`, and `IUserIdProvider` from an ABP test module depending on `CenseqSignalRModule`.

- [ ] **Step 2: Run module tests and verify RED**

Run: `dotnet test censeq-admin-api/framework/Censeq.Framework.SignalR.Tests/Censeq.Framework.SignalR.Tests.csproj`
Expected: FAIL because module registrations and hub files do not exist.

- [ ] **Step 3: Implement hubs and module**

Implement authenticated hubs, tenant grouping, online changed broadcasts, force-offline RPC, notification sender methods, SignalR options, and route mapping for `[CenseqHubRoute]`.

- [ ] **Step 4: Run module tests and verify GREEN**

Run: `dotnet test censeq-admin-api/framework/Censeq.Framework.SignalR.Tests/Censeq.Framework.SignalR.Tests.csproj`
Expected: PASS.

### Task 3: Host Integration

**Files:**
- Modify: `censeq-admin-api/src/Censeq.Admin.HttpApi.Host/Censeq.Admin.HttpApi.Host.csproj`
- Modify: `censeq-admin-api/src/Censeq.Admin.HttpApi.Host/CenseqHttpApiHostModule.cs`
- Modify: `censeq-admin-api/src/Censeq.Admin.HttpApi.Host/appsettings.json`

- [ ] **Step 1: Add SignalR project reference and module dependency**

Reference `framework/Censeq.Framework.SignalR` and add `typeof(CenseqSignalRModule)` to the Host `DependsOn`.

- [ ] **Step 2: Configure OpenIddict token extraction**

PreConfigure OpenIddict Validation ASP.NET Core events so `/hubs` can read `access_token` from the query string.

- [ ] **Step 3: Add `SignalR` config defaults**

Add `EnableDetailedErrors`, `KeepAliveSeconds`, `ClientTimeoutSeconds`, `RedisConnectionString`, and `RedisChannelPrefix`.

- [ ] **Step 4: Verify backend build**

Run: `dotnet build censeq-admin-api/Censeq.Admin.sln`
Expected: PASS.

### Task 4: Frontend SignalR Foundation

**Files:**
- Modify: `censeq-admin-web/package.json`
- Modify: `censeq-admin-web/vite.config.ts`
- Create: `censeq-admin-web/src/api/models/signalr/index.ts`
- Create: `censeq-admin-web/src/composables/useSignalR.ts`
- Create: `censeq-admin-web/src/composables/useOnlineUserHub.ts`
- Create: `censeq-admin-web/src/composables/useNotificationHub.ts`
- Create: `censeq-admin-web/src/stores/onlineUser.ts`
- Create: `censeq-admin-web/src/stores/notification.ts`

- [ ] **Step 1: Add SignalR dependency**

Add `@microsoft/signalr` to dependencies.

- [ ] **Step 2: Implement typed models and composables**

Use the existing `useOidc().getAcessToken()` method for `accessTokenFactory`, and expose `start`, `stop`, `on`, `off`, and `invoke`.

- [ ] **Step 3: Implement stores**

Online user store handles replace, apply change, and forced logout. Notification store tracks received messages and unread count.

- [ ] **Step 4: Add Vite `/hubs` proxy**

Add a WebSocket proxy using `VITE_API_URL` as the target when present.

### Task 5: Frontend Online User Page And App Lifecycle

**Files:**
- Create: `censeq-admin-web/src/views/system/online-user/index.vue`
- Modify: `censeq-admin-web/src/router/route.ts`
- Modify: `censeq-admin-web/src/App.vue`
- Modify: `censeq-admin-web/src/i18n/lang/zh-cn.ts`
- Modify: `censeq-admin-web/src/i18n/lang/en.ts`
- Modify: `censeq-admin-web/src/i18n/lang/zh-tw.ts`

- [ ] **Step 1: Create management page**

Build an Element Plus table showing user name, display name, IP, user agent, connected time, and a force-offline action.

- [ ] **Step 2: Register route and labels**

Add `/system/online-user` route under the system group and add i18n labels.

- [ ] **Step 3: Start and stop hubs with auth lifecycle**

Start hubs after authenticated app mount and stop them on logout/unauthenticated state.

- [ ] **Step 4: Verify frontend build**

Run: `cd censeq-admin-web && pnpm install && pnpm build`
Expected: PASS.

### Task 6: Final Verification

**Files:**
- Inspect all changed files via `git diff --stat` and targeted `git diff`.

- [ ] **Step 1: Run backend tests**

Run: `dotnet test censeq-admin-api/framework/Censeq.Framework.SignalR.Tests/Censeq.Framework.SignalR.Tests.csproj`
Expected: PASS.

- [ ] **Step 2: Run backend build**

Run: `dotnet build censeq-admin-api/Censeq.Admin.sln`
Expected: PASS.

- [ ] **Step 3: Run frontend build**

Run: `cd censeq-admin-web && pnpm build`
Expected: PASS.

- [ ] **Step 4: Review diff for unrelated changes**

Confirm existing user changes such as `IdentityUser.cs` are not reverted or bundled into SignalR edits.

---

Self-review:

- Spec coverage: This plan covers framework module, Host integration, frontend SignalR wrappers, online-user store/page, notification foundation, and verification. Redis Backplane, Nginx, metrics, and full business notification events are intentionally deferred.
- Placeholder scan: No `TBD` or implementation placeholders remain.
- Type consistency: Backend DTO/client names match the design document and frontend model names.
