﻿using System.Collections.Generic;

namespace Censeq.Account.Web.ProfileManagement;

public class ProfileManagementPageOptions
{
    public List<IProfileManagementPageContributor> Contributors { get; }

    public ProfileManagementPageOptions()
    {
        Contributors = new List<IProfileManagementPageContributor>();
    }
}
