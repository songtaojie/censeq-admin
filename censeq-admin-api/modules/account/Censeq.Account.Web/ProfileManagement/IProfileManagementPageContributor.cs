﻿using System.Threading.Tasks;

namespace Censeq.Account.Web.ProfileManagement;

public interface IProfileManagementPageContributor
{
    Task ConfigureAsync(ProfileManagementPageCreationContext context);
}
