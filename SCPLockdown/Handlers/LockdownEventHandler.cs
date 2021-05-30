using ScpLockdown.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using System.Threading.Tasks;
using Synapse.Api.Events.SynapseEventArguments;
using Synapse.Api;
using ScpLockdown.Services;

namespace ScpLockdown.Handlers
{
    public sealed class LockdownEventHandler
    {
        private ScpLockdown _plugin;

        public LockdownEventHandler(ScpLockdown plugin)
        {
            _plugin = plugin;
        }

        public void OnRoundStart()
        {
            _plugin.LockdownService.StartLockdown();
        }
        public void OnSetClass(PlayerSetClassEventArgs ev)
        {
            if (ev.Player.RealTeam != Team.SCP)
            {
                return;
            }
            if (_plugin.LockdownService.TryGetCoroutines(ev.Player, out IEnumerable<CoroutineHandle> coroutines))
            {
                Timing.KillCoroutines(coroutines.ToArray());
                ev.Player.GodMode = false;
            }
        }
        public void OnRoundEnd()
        {
            _plugin.LockdownService.ResetCoroutines();
        }
        public void OnEscapingPocketDimension(PocketDimensionLeaveEventArgs ev)
        {
            if (ev.Player.RoleType != RoleType.Scp106)
            {
                return;
            }
            if (_plugin.LockdownService.LockedDownScps.TryGetValue(RoleType.Scp106, out bool locked) && locked)
            {
                ev.Allow = false;
                ev.Player.SendToPocketDimension();
            }
        }

        public void OnInteractingTesla(TriggerTeslaEventArgs ev)
        {
            Synapse.Api.Logger.Get.Warn("Hey!");
            if (ev.Player.RoleType != RoleType.Scp079)
            {
                return;
            }
            if (_plugin.LockdownService.LockedDownScps.TryGetValue(RoleType.Scp079, out bool locked) && locked)
            {
                ev.Trigger = false;
            }
        }
    }
}
