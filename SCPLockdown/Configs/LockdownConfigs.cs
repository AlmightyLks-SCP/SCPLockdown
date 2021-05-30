using Synapse.Config;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScpLockdown.Configs
{
    public sealed class LockdownConfigs : IConfigSection
    {
        [Description("The affected SCPs and their duration in seconds of lockdown")]
        public Dictionary<RoleType, int> AffectedScps { get; set; } = new Dictionary<RoleType, int>()
        {
            { RoleType.Scp173, 60 },
            { RoleType.Scp079, 60 },
            { RoleType.Scp096, 60 },
            { RoleType.Scp106, 60 },
            { RoleType.Scp049, 60 },
            { RoleType.Scp93989, 60 },
            { RoleType.Scp93953, 60 }
        };
    }
}
