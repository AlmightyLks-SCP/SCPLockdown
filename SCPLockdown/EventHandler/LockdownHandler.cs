using Exiled.API.Features;
using Exiled.Events;
using ScpLockdown.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MEC;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.Events.EventArgs;
using Exiled.API.Enums;
using ScpLockdown.DirtyWorkaround;

namespace ScpLockdown.EventHandlers
{
    public class RoundHandler
    {
        private LockdownStates _lockdownStates;
        private List<KeyValuePair<Player, CoroutineHandle>> larryCoroutines;
        private List<CoroutineHandle> runningCoroutines;
        private ScpLockdown plugin;
        public RoundHandler(ScpLockdown plugin)
        {
            this.plugin = plugin;
            _lockdownStates = new LockdownStates();
            runningCoroutines = new List<CoroutineHandle>();
            larryCoroutines = new List<KeyValuePair<Player, CoroutineHandle>>();
        }

        public void OnRoundStart()
        {
            var configScpList = new Dictionary<RoleType, int>();

            //Filter unique RoleTypes
            foreach (var entry in plugin.Config.AffectedScps)
            {
                if (!configScpList.Select((e) => e.Key).Contains(entry.Key))
                    configScpList.Add(entry.Key, entry.Value);
            }

            runningCoroutines.Add(Timing.CallDelayed(1, () =>
            {
                foreach (var scp in configScpList)
                {
                    _lockdownStates.ToggleLockedUpState(scp.Key);

                    switch (scp.Key)
                    {
                        case RoleType.Scp173:
                            Lockdown173(scp);
                            break;
                        case RoleType.Scp079:
                            Lockdown079(scp);
                            break;
                        case RoleType.Scp106:
                            Lockdown106(scp);
                            break;
                        case RoleType.Scp049:
                            Lockdown049(scp);
                            break;
                        case RoleType.Scp096:
                            Lockdown096(scp);
                            break;
                        case RoleType.Scp93953:
                        case RoleType.Scp93989:
                            Lockdown939(scp);
                            break;
                    }
                }
            }));
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (larryCoroutines.Select((e) => e.Key).Contains(ev.Player))
            {
                Timing.KillCoroutines(larryCoroutines.Where((e) => e.Key == ev.Player).Select((e) => e.Value));
                larryCoroutines.RemoveAt(larryCoroutines.FindIndex((e) => e.Key == ev.Player));
                ev.Player.IsGodModeEnabled = false;
            }
        }
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Timing.KillCoroutines(runningCoroutines);
            Timing.KillCoroutines(larryCoroutines.Select((e) => e.Value));
            runningCoroutines.Clear();
        }
        public void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Scp106)
            {
                ev.IsAllowed = false;
                ev.Player.SendToPocketDimension();
            }
        }
        public void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Scp106)
            {
                ev.IsAllowed = false;
                ev.Player.SendToPocketDimension();
            }
        }
        public void OnInteractingTesla(InteractingTeslaEventArgs ev)
            => ev.IsAllowed = !LockdownStates.Scp079LockedUp;
        public void OnInteractingDoor(InteractingDoorEventArgs ev)
            => ev.IsAllowed = !LockdownStates.Scp079LockedUp;

        public void ResetAllStates()
            => _lockdownStates.ResetAllStates();

        private Task Lockdown173(KeyValuePair<RoleType, int> scp)
        {
            Door door173 = Map.Doors.First((e) => String.Equals(e.DoorName, "173"));
            var heavyDoor173 = Map.Doors.GetClosestDoor(door173, true);
            heavyDoor173.locked = true;

            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                heavyDoor173.locked = false;
                _lockdownStates.ToggleLockedUpState(RoleType.Scp173);

                foreach (var player in Player.List.Where((player) => player.Role == RoleType.Scp173))
                {
                    player.ShowHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task Lockdown106(KeyValuePair<RoleType, int> scp)
        {
            foreach (var player in Player.List.Where((e) => e.Role == RoleType.Scp106))
            {
                var prevPos = player.Position;
                player.SendToPocketDimension();
                player.IsGodModeEnabled = true;

                larryCoroutines.Add(new KeyValuePair<Player, CoroutineHandle>(player, Timing.CallDelayed(scp.Value, () =>
                {
                    player.Position = prevPos;
                    player.IsGodModeEnabled = false;
                    _lockdownStates.ToggleLockedUpState(RoleType.Scp106);

                    player.ShowHint("You're good to go!");
                })));
            }
            return Task.CompletedTask;
        }
        private Task Lockdown079(KeyValuePair<RoleType, int> scp)
        {
            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                _lockdownStates.ToggleLockedUpState(RoleType.Scp079);
                foreach (var player in Player.List.Where((player) => player.Role == RoleType.Scp079))
                {
                    player.ShowHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task Lockdown049(KeyValuePair<RoleType, int> scp)
        {
            Door door049 = Map.Doors.First((e) => String.Equals(e.DoorName, "049_ARMORY"));
            var heavyDoor049 = Map.Doors.GetClosestDoor(door049);
            heavyDoor049.locked = true;
            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                heavyDoor049.locked = false;
                _lockdownStates.ToggleLockedUpState(RoleType.Scp049);

                foreach (var player in Player.List.Where((player) => player.Role == RoleType.Scp049))
                {
                    player.ShowHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task Lockdown096(KeyValuePair<RoleType, int> scp)
        {
            Door door096 = Map.Doors.First((e) => e.DoorName == "096");
            Door nearestDoor = Map.Doors.GetClosestDoor(door096);
            nearestDoor.locked = true;
            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                nearestDoor.locked = false;
                _lockdownStates.ToggleLockedUpState(RoleType.Scp096);

                foreach (var player in Player.List.Where((player) => player.Role == RoleType.Scp096))
                {
                    player.ShowHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task Lockdown939(KeyValuePair<RoleType, int> scp)
        {
            List<Door> targetDoors = new List<Door>();

            Room room939 = Map.Rooms.First((e) => e.Type == RoomType.Hcz939);

            targetDoors.Add(Map.Doors.GetClosestDoor(room939));
            targetDoors.Add(Map.Doors.GetClosestDoor(room939, false, targetDoors));

            foreach (var door in targetDoors)
                door.locked = true;

            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                foreach (var door in targetDoors)
                    door.locked = false;

                _lockdownStates.ToggleLockedUpState(RoleType.Scp93953);

                foreach (var player in Player.List.Where((player) => player.Role == RoleType.Scp93953 || player.Role == RoleType.Scp93989))
                {
                    player.ShowHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
    }
}
