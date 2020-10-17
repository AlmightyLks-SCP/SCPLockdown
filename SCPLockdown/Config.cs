using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace SCPLockdown
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; }

        [Description("The affected SCPs and their duration of lockdown.")]
        public Dictionary<RoleType, int> AffectedSCPs { get; set; } = new Dictionary<RoleType, int>();
    }
}
