﻿using System.Threading.Tasks;

namespace Starshine.Admin.Web.ProfileManagement;

public interface IProfileManagementPageContributor
{
    Task ConfigureAsync(ProfileManagementPageCreationContext context);
}
