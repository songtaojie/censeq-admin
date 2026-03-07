﻿using System.Threading.Tasks;

namespace Censeq.Admin.Web.ProfileManagement;

public interface IProfileManagementPageContributor
{
    Task ConfigureAsync(ProfileManagementPageCreationContext context);
}
