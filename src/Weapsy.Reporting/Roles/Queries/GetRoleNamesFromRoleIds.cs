﻿using System;
using System.Collections.Generic;
using Weapsy.Infrastructure.Queries;

namespace Weapsy.Reporting.Roles.Queries
{
    public class GetRoleNamesFromRoleIds : IQuery
    {
        public IEnumerable<Guid> RoleIds { get; set; }
    }
}
