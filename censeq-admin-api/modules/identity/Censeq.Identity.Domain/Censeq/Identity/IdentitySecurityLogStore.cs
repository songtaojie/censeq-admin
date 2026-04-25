using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.SecurityLog;
using Volo.Abp.Uow;

namespace Censeq.Identity;

[Dependency(ReplaceServices = true)]
/// <summary>
/// 身份安全日志存储
/// </summary>
public class IdentitySecurityLogStore : ISecurityLogStore, ITransientDependency
{
    /// <summary>
    /// ILogger<Identity安全日志Store>
    /// </summary>
    public ILogger<IdentitySecurityLogStore> Logger { get; set; }

    /// <summary>
    /// Abp安全日志选项
    /// </summary>
    protected AbpSecurityLogOptions SecurityLogOptions { get; }
    /// <summary>
    /// I身份安全日志仓储
    /// </summary>
    protected IIdentitySecurityLogRepository IdentitySecurityLogRepository { get; }
    /// <summary>
    /// IGuidGenerator
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// I单元OfWork管理器
    /// </summary>
    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    public IdentitySecurityLogStore(
        ILogger<IdentitySecurityLogStore> logger,
        IOptions<AbpSecurityLogOptions> securityLogOptions,
        IIdentitySecurityLogRepository identitySecurityLogRepository,
        IGuidGenerator guidGenerator,
        IUnitOfWorkManager unitOfWorkManager)
    {
        Logger = logger;
        SecurityLogOptions = securityLogOptions.Value;
        IdentitySecurityLogRepository = identitySecurityLogRepository;
        GuidGenerator = guidGenerator;
        UnitOfWorkManager = unitOfWorkManager;
    }

    public async Task SaveAsync(SecurityLogInfo securityLogInfo)
    {
        if (!SecurityLogOptions.IsEnabled)
        {
            return;
        }

        using (var uow = UnitOfWorkManager.Begin(requiresNew: true))
        {
            await IdentitySecurityLogRepository.InsertAsync(new IdentitySecurityLog(GuidGenerator, securityLogInfo));
            await uow.CompleteAsync();
        }
    }
}
