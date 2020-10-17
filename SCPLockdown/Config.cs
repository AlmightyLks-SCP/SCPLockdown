using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScpLockdown
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("The affected SCPs and their duration [seconds] of lockdown.")]
        public Dictionary<RoleType, int> AffectedScps { get; set; } = new Dictionary<RoleType, int>()
        {
            { RoleType.Scp173,60 },
            { RoleType.Scp079,60 },
            { RoleType.Scp096,60 },
            { RoleType.Scp106,60 },
            { RoleType.Scp049,60 },
            { RoleType.Scp93989,60 },
            { RoleType.Scp93953,60 }
        };
    }
}
