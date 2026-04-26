import re
import os
from pathlib import Path

# File list from the grep output
files = set([
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/OrganizationUnitAppService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/Integration/IdentityUserIntegrationService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/IdentityUserLookupAppService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/IdentityUserAppService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/IdentitySessionAppService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/IdentityRoleAppService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Application/Censeq/Identity/IdentityClaimTypeAppService.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreOrganizationUnitRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentityUserRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentityUserDelegationRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentitySessionRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentitySecurityLogRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentityRoleRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentityLinkUserRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.EntityFrameworkCore/Censeq/Identity/EntityFrameworkCore/EfCoreIdentityClaimTypeRepository.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/UserRoleFinder.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.AspNetCore/Volo/Abp/Identity/AspNetCore/CenseqSignInManager.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/OrganizationUnitManager.cs",
    "censeq-admin-api/modules/identity/Censeq.PermissionManagement.Domain.Identity/Censeq/PermissionManagement/UserPermissionManagerExtensions.cs",
    "censeq-admin-api/modules/identity/Censeq.PermissionManagement.Domain.Identity/Censeq/PermissionManagement/RolePermissionManagerExtensions.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityUserRepositoryExternalUserLookupServiceProvider.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityUserManager.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityUserDelegationManager.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityDataSeeder.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/ExternalLoginProviderBase.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/ExternalLoginProviderWithPasswordBase.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityClaimTypeManager.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityRoleManager.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityDynamicClaimsPrincipalContributorCache.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/IdentityLinkUserManager.cs",
    "censeq-admin-api/modules/identity/Censeq.Identity.Domain/Censeq/Identity/CenseqIdentityUserValidator.cs",
])

BAD_PATTERN = re.compile(
    r'^(\s*)///\s*<summary>\s*\n'
    r'\s*///\s*(?:Task<[^>]+>|bool|int|string|long|List<[^>]+>|IQueryable<[^>]+>|(?:\w*[一-鿿]\w*)+)\s*\n'
    r'\s*///\s*</summary>\s*\n',
    re.MULTILINE
)

METHOD_PATTERN = re.compile(
    r'(?:\s*\[.*?\]\s*)*\s*'
    r'(?:public|protected|internal|private)\s+'
    r'(?:virtual\s+|async\s+|static\s+|override\s+|abstract\s+)*'
    r'(?:[\w<>,\?\[\]\s]+?)\s+'
    r'(\w+)\s*\(',
    re.MULTILINE | re.DOTALL
)

name_map = {
    'GetAsync': '异步获取。',
    'GetAllListAsync': '异步获取全部列表。',
    'GetListAsync': '异步获取列表。',
    'CreateAsync': '异步创建。',
    'UpdateAsync': '异步更新。',
    'DeleteAsync': '异步删除。',
    'FindByIdAsync': '根据ID异步查找。',
    'FindByNameAsync': '根据名称异步查找。',
    'GetCountAsync': '异步获取总数。',
    'AddClaimAsync': '异步添加声明。',
    'RemoveClaimAsync': '异步移除声明。',
    'SetRoleNameAsync': '异步设置角色名称。',
    'GetRoleNameAsync': '异步获取角色名称。',
    'GetRoleIdAsync': '异步获取角色ID。',
    'GetNormalizedRoleNameAsync': '异步获取标准化角色名称。',
    'SetNormalizedRoleNameAsync': '异步设置标准化角色名称。',
    'GetClaimsAsync': '异步获取声明列表。',
    'GetUsersForClaimAsync': '异步获取具有指定声明的用户。',
    'GetUsersInRoleAsync': '异步获取指定角色中的用户。',
    'AddToRoleAsync': '异步添加到角色。',
    'RemoveFromRoleAsync': '异步从角色移除。',
    'GetRolesAsync': '异步获取角色列表。',
    'IsInRoleAsync': '异步检查是否在角色中。',
    'GetLoginsAsync': '异步获取登录信息。',
    'AddLoginAsync': '异步添加登录。',
    'RemoveLoginAsync': '异步移除登录。',
    'GetTokenAsync': '异步获取令牌。',
    'SetTokenAsync': '异步设置令牌。',
    'RemoveTokenAsync': '异步移除令牌。',
    'GetAuthenticatorKeyAsync': '异步获取身份验证器密钥。',
    'ResetAuthenticatorKeyAsync': '异步重置身份验证器密钥。',
    'GetTwoFactorEnabledAsync': '异步获取是否启用双重身份验证。',
    'SetTwoFactorEnabledAsync': '异步设置双重身份验证。',
    'GetPhoneNumberAsync': '异步获取电话号码。',
    'SetPhoneNumberAsync': '异步设置电话号码。',
    'GetPhoneNumberConfirmedAsync': '异步获取电话号码是否已确认。',
    'SetPhoneNumberConfirmedAsync': '异步设置电话号码确认状态。',
    'GetEmailAsync': '异步获取电子邮箱。',
    'SetEmailAsync': '异步设置电子邮箱。',
    'GetEmailConfirmedAsync': '异步获取邮箱是否已确认。',
    'SetEmailConfirmedAsync': '异步设置邮箱确认状态。',
    'GetNormalizedEmailAsync': '异步获取标准化电子邮箱。',
    'SetNormalizedEmailAsync': '异步设置标准化电子邮箱。',
    'GetNormalizedUserNameAsync': '异步获取标准化用户名。',
    'SetNormalizedUserNameAsync': '异步设置标准化用户名。',
    'GetPasswordHashAsync': '异步获取密码哈希。',
    'SetPasswordHashAsync': '异步设置密码哈希。',
    'HasPasswordAsync': '异步检查是否有密码。',
    'GetUserIdAsync': '异步获取用户ID。',
    'GetUserNameAsync': '异步获取用户名。',
    'SetUserNameAsync': '异步设置用户名。',
    'GetLockoutEnabledAsync': '异步获取是否启用锁定。',
    'SetLockoutEnabledAsync': '异步设置锁定。',
    'GetLockoutEndDateAsync': '异步获取锁定结束日期。',
    'SetLockoutEndDateAsync': '异步设置锁定结束日期。',
    'GetAccessFailedCountAsync': '异步获取访问失败次数。',
    'ResetAccessFailedCountAsync': '异步重置访问失败次数。',
    'IncrementAccessFailedCountAsync': '异步增加访问失败次数。',
    'GeneratePasswordResetTokenAsync': '异步生成密码重置令牌。',
    'GenerateEmailConfirmationTokenAsync': '异步生成邮箱确认令牌。',
    'ConfirmEmailAsync': '异步确认邮箱。',
    'ChangeEmailAsync': '异步更改邮箱。',
    'ChangePhoneNumberAsync': '异步更改电话号码。',
    'ChangePasswordAsync': '异步更改密码。',
    'CheckPasswordAsync': '异步检查密码。',
    'ResetPasswordAsync': '异步重置密码。',
    'ValidateAsync': '异步验证。',
    'Dispose': '释放资源。',
    'SignInOrTwoFactorAsync': '登录或执行双重身份验证。',
    'SignInAsync': '异步登录。',
    'PreSignInCheckAsync': '异步预登录检查。',
    'GetExternalLoginUserAsync': '异步获取外部登录用户。',
    'CreateExternalUserAsync': '异步创建外部用户。',
    'UpdateUserAsync': '异步更新用户。',
    'SeedAsync': '异步初始化种子数据。',
    'LinkAsync': '异步关联用户。',
    'UnlinkAsync': '异步取消关联用户。',
    'IsLinkedAsync': '异步检查是否已关联。',
    'GenerateLinkTokenAsync': '异步生成关联令牌。',
    'VerifyLinkTokenAsync': '异步验证关联令牌。',
    'GetSourceUserAsync': '异步获取源用户。',
    'GetTargetUserAsync': '异步获取目标用户。',
    'FindAsync': '异步查找。',
    'AnyAsync': '异步检查是否存在。',
    'GetAllAsync': '异步获取全部。',
    'InsertAsync': '异步插入。',
    'MoveAsync': '异步移动。',
    'GetChildrenAsync': '异步获取子项。',
    'GetAllChildrenWithParentCodeAsync': '异步获取所有具有父代码的子项。',
    'GetRolesAsync': '异步获取角色列表。',
    'GetRoleNamesAsync': '异步获取角色名称列表。',
    'GetMembersAsync': '异步获取成员。',
    'GetMemberCountAsync': '异步获取成员数量。',
    'IsMemberAsync': '异步检查是否为成员。',
    'AddRoleToOrganizationUnitAsync': '异步添加角色到组织单元。',
    'RemoveRoleFromOrganizationUnitAsync': '异步从组织单元移除角色。',
    'GetOrganizationUnitsAsync': '异步获取组织单元。',
    'GetRolesInOrganizationUnitAsync': '异步获取组织单元中的角色。',
    'SetOrganizationUnitsAsync': '异步设置组织单元。',
    'UpdateRoleAsync': '异步更新角色。',
    'CreateRoleAsync': '异步创建角色。',
    'DeleteRoleAsync': '异步删除角色。',
    'GetDeletedRecordsAsync': '异步获取已删除的记录。',
    'GetSecurityLogsAsync': '异步获取安全日志。',
    'SaveAsync': '异步保存。',
    'RevokeAsync': '异步撤销。',
    'GetSessionsAsync': '异步获取会话。',
    'GetActiveSessionsAsync': '异步获取活动会话。',
    'RevokeSessionAsync': '异步撤销会话。',
    'GetActiveUserSessionsAsync': '异步获取用户活动会话。',
    'GetDelegationActiveListAsync': '异步获取有效委托列表。',
    'GetDelegationListAsync': '异步获取委托列表。',
    'GetAvailableDelegationsAsync': '异步获取可用委托。',
    'FindByNormalizedNameAsync': '根据标准化名称异步查找。',
    'FindByNormalizedUserNameOrEmailAsync': '根据标准化用户名或邮箱异步查找。',
    'GetListByNormalizedRoleNameAsync': '根据标准化角色名称异步获取列表。',
    'GetListByClaimAsync': '根据声明异步获取列表。',
    'GetListByRoleIdAsync': '根据角色ID异步获取列表。',
    'GetListByOrganizationUnitIdAsync': '根据组织单元ID异步获取列表。',
    'GetListByIdsAsync': '根据ID列表异步获取。',
    'GetListByIdListAsync': '根据ID列表异步获取。',
    'GetCountByOrganizationUnitIdAsync': '异步获取组织单元中的用户数量。',
    'GetUserNameFromDatabaseAsync': '从数据库异步获取用户名。',
    'GetRoleNamesFromDatabaseAsync': '从数据库异步获取角色名称。',
    'UpdateSecurityStampAsync': '异步更新安全戳。',
    'ShouldPeriodicallyChangePasswordAsync': '异步检查是否应定期更改密码。',
    'GetUserTwoFactorEnabledAsync': '异步获取用户双重身份验证状态。',
    'CanLoginAsync': '异步检查是否可以登录。',
    'IsEmailConfirmedAsync': '异步检查邮箱是否已确认。',
    'GetUserTwoFactorProviderAsync': '异步获取用户双重身份验证提供程序。',
    'GetValidTwoFactorProvidersAsync': '异步获取有效的双重身份验证提供程序。',
    'VerifyTwoFactorTokenAsync': '异步验证双重身份验证令牌。',
    'VerifyChangePhoneNumberTokenAsync': '异步验证更改电话号码令牌。',
    'GenerateChangePhoneNumberTokenAsync': '异步生成更改电话号码令牌。',
    'AccessFailedAsync': '异步记录访问失败。',
    'GetNewSecurityStamp': '获取新的安全戳。',
    'NormalizeName': '标准化名称。',
    'NormalizeEmail': '标准化邮箱。',
    'RegisterTokenProvider': '注册令牌提供程序。',
    'FindProvider': '查找提供程序。',
    'GetUserStore': '获取用户存储。',
    'GetEntityByIdAsync': '根据ID异步获取实体。',
    'GetCurrentUserDelegationsAsync': '异步获取当前用户委托。',
    'GetActiveDelegationAsync': '异步获取有效委托。',
    'GetDelegatedUsersAsync': '异步获取被委托用户。',
    'GetMyDelegatedUsersAsync': '异步获取我的被委托用户。',
    'GetMyAuthorizedUsersAsync': '异步获取我的授权用户。',
    'IsDelegatedAsync': '异步检查是否已委托。',
    'GetMyDelegatedUserAsync': '异步获取我的被委托用户。',
    'GetCurrentUserAsync': '异步获取当前用户。',
    'GetCurrentUserRolesAsync': '异步获取当前用户角色。',
    'GetCurrentUserOrganitaionUnitsAsync': '异步获取当前用户组织单元。',
    'GetCurrentUserPermissionsAsync': '异步获取当前用户权限。',
    'GetCurrentUserProfileAsync': '异步获取当前用户资料。',
    'UpdateCurrentUserProfileAsync': '异步更新当前用户资料。',
    'ChangeCurrentUserPasswordAsync': '异步更改当前用户密码。',
    'SetCurrentUserPasswordAsync': '异步设置当前用户密码。',
    'GetAssignableRolesAsync': '异步获取可分配角色。',
    'GetAssignableUsersAsync': '异步获取可分配用户。',
    'ImportUsersAsync': '异步导入用户。',
    'ExportUsersAsync': '异步导出用户。',
    'UnlockAsync': '异步解锁。',
    'GetRolesByUserIdAsync': '异步根据用户ID获取角色。',
    'GetRoleListAsync': '异步获取角色列表。',
    'GetClaimListAsync': '异步获取声明列表。',
    'GetAvailableClaimsAsync': '异步获取可用声明。',
    'SetClaimsAsync': '异步设置声明。',
    'ResetClaimsAsync': '异步重置声明。',
    'AddClaimsAsync': '异步添加声明。',
    'RemoveClaimsAsync': '异步移除声明。',
    'GetOrganizationUnitListAsync': '异步获取组织单元列表。',
    'GetOrganizationUnitTreeAsync': '异步获取组织单元树。',
    'MoveOrganizationUnitAsync': '异步移动组织单元。',
    'GetIdentityClaimTypeListAsync': '异步获取身份声明类型列表。',
    'GetIdentityClaimTypeAsync': '异步获取身份声明类型。',
    'CreateIdentityClaimTypeAsync': '异步创建身份声明类型。',
    'UpdateIdentityClaimTypeAsync': '异步更新身份声明类型。',
    'DeleteIdentityClaimTypeAsync': '异步删除身份声明类型。',
}

def get_summary(method_name):
    if method_name in name_map:
        return name_map[method_name]
    # Heuristic fallback
    if method_name.endswith('Async'):
        base = method_name[:-5]
        return f'异步执行 {base} 操作。'
    return f'执行 {method_name} 操作。'

def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original = content
    changed = False

    # We use a function-based replacement to look ahead for method names
    def replacer(match):
        nonlocal changed
        indent = match.group(1)
        rest_after = content[match.end():]
        m = METHOD_PATTERN.match(rest_after)
        if m:
            method_name = m.group(1)
            summary = get_summary(method_name)
            changed = True
            return f'{indent}/// <summary>\n{indent}/// {summary}\n{indent}/// </summary>\n'
        # If we can't find a method name, just remove the bad block
        changed = True
        return ''

    new_content = BAD_PATTERN.sub(replacer, content)
    if changed:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f'Fixed: {filepath}')
    return changed

base = Path('e:/Code/dotnet/myproject/censeq-admin')
count = 0
for rel in files:
    fp = base / rel
    if fp.exists():
        if process_file(str(fp)):
            count += 1
    else:
        print(f'Missing: {fp}')

print(f'Done. Modified {count} files.')
