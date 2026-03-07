﻿using System.Collections.Generic;

namespace Starshine.Admin.Web.ProfileManagement;

public class ProfileManagementPageOptions
{
    public List<IProfileManagementPageContributor> Contributors { get; }

    public ProfileManagementPageOptions()
    {
        Contributors = new List<IProfileManagementPageContributor>();
    }
}
