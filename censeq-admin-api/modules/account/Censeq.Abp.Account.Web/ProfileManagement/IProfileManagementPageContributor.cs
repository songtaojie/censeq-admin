﻿using System.Threading.Tasks;

namespace Censeq.Abp.Account.Web.ProfileManagement;

public interface IProfileManagementPageContributor
{
    Task ConfigureAsync(ProfileManagementPageCreationContext context);
}
