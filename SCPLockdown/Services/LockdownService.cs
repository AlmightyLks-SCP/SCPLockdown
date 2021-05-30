using SynapseDoor = Synapse.Api.Door;
using MEC;
using ScpLockdown.Configs;
using ScpLockdown.Helper;
using Synapse.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synapse;
using static RoomInformation;

namespace ScpLockdown.Services
{
    public sealed class LockdownService
    {
        public IReadOnlyDictionary<RoleType, bool> LockedDownScps { get; }

        private Dictionary<RoleType, bool> _lockedDownScps { get; }
        private Dictionary<Player, CoroutineHandle> _larryCoroutines;
        private List<CoroutineHandle> _runningCoroutines;
        private ScpLockdown _plugin;

        public LockdownService(ScpLockdown plugin)
        {
            _plugin = plugin;

            var dict = new Dictionary<RoleType, bool>();
            foreach (var entry in _plugin.Config.AffectedScps)
            {
                dict.Add(entry.Key, false);
            }
            _lockedDownScps = dict;
            LockedDownScps = _lockedDownScps;

            _runningCoroutines = new List<CoroutineHandle>();
            _larryCoroutines = new Dictionary<Player, CoroutineHandle>();
        }

        public void StartLockdown()
        {
            Synapse.Api.Logger.Get.Warn("3");
            _runningCoroutines.Add(Timing.CallDelayed(5, () =>
            {
                Synapse.Api.Logger.Get.Warn("2");
                foreach (var scp in _plugin.Config.AffectedScps)
                {
                    ToggleLockdownState(scp.Key);
                    Synapse.Api.Logger.Get.Warn($"{scp.Key} | {scp.Value}");
                    switch (scp.Key)
                    {
                        case RoleType.Scp173:
                            LockDown173(scp);
                            break;
                        case RoleType.Scp079:
                            LockDown079(scp);
                            break;
                        case RoleType.Scp106:
                            LockDown106(scp);
                            break;
                        case RoleType.Scp049:
                            LockDown049(scp);
                            break;
                        case RoleType.Scp096:
                            LockDown096(scp);
                            break;
                        case RoleType.Scp93953:
                        case RoleType.Scp93989:
                            LockDown939(scp);
                            break;
                    }
                }
            }));
        }

        public void ResetCoroutines()
        {
            _runningCoroutines.ForEach((_) => Timing.KillCoroutines(_));
            _larryCoroutines.Select((_) => _.Value).ToList().ForEach((_) => Timing.KillCoroutines(_));
            _runningCoroutines.Clear();
        }
        public void ResetAllStates()
        {
            foreach (var entry in _lockedDownScps)
                _lockedDownScps[entry.Key] = false;
        }
        public bool TryGetCoroutines(Player player, out IEnumerable<CoroutineHandle> coroutines)
        {
            if (_larryCoroutines.ContainsKey(player))
            {
                coroutines = _larryCoroutines
                    .Where(e => e.Key == player)
                    .Select(e => e.Value);
                return true;
            }
            else
            {
                coroutines = Array.Empty<CoroutineHandle>();
                return false;
            }
        }

        private Task LockDown173(KeyValuePair<RoleType, int> scp)
        {
            Synapse.Api.Door connectorDoor173 = Synapse.Api.Map.Get.Doors.First((e) => e.DoorType == Synapse.Api.Enum.DoorType.LCZ_173_Connector);
            connectorDoor173.Locked = true;

            _runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                connectorDoor173.Locked = false;
                ToggleLockdownState(RoleType.Scp173);

                foreach (var player in Synapse.Server.Get.Players.Where((player) => player.RoleType == RoleType.Scp173))
                {
                    player.GiveTextHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task LockDown106(KeyValuePair<RoleType, int> scp)
        {
            foreach (var player in Synapse.Server.Get.Players.Where((e) => e.RoleType == RoleType.Scp106))
            {
                var prevPos = player.Position;
                player.SendToPocketDimension();
                player.GodMode = true;

                _larryCoroutines.Add(player, Timing.CallDelayed(scp.Value, () =>
                {
                    player.Position = prevPos;
                    player.GodMode = false;
                    ToggleLockdownState(RoleType.Scp106);

                    player.GiveTextHint("You're good to go!");
                }));
            }
            return Task.CompletedTask;
        }
        private Task LockDown079(KeyValuePair<RoleType, int> scp)
        {
            _runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                ToggleLockdownState(RoleType.Scp079);
                foreach (var player in Synapse.Server.Get.Players.Where((player) => player.RoleType == RoleType.Scp079))
                {
                    player.GiveTextHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task LockDown049(KeyValuePair<RoleType, int> scp)
        {
            SynapseDoor door049 = Server.Get.Map.Doors.First((e) => e.Name == "049_ARMORY");
            var heavyDoor049 = Server.Get.Map.Doors.GetClosestDoor(door049);
            heavyDoor049.Locked = true;
            _runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                heavyDoor049.Locked = false;
                ToggleLockdownState(RoleType.Scp049);

                foreach (var player in Server.Get.Players.Where((player) => player.RoleType == RoleType.Scp049))
                {
                    player.GiveTextHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task LockDown096(KeyValuePair<RoleType, int> scp)
        {
            SynapseDoor door096 = Server.Get.Map.Doors.First((e) => e.Name == "096");
            SynapseDoor nearestDoor = Server.Get.Map.Doors.GetClosestDoor(door096);
            nearestDoor.Locked = true;
            _runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                nearestDoor.Locked = false;
                ToggleLockdownState(RoleType.Scp096);

                foreach (var player in Server.Get.Players.Where((player) => player.RoleType == RoleType.Scp096))
                {
                    player.GiveTextHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private Task LockDown939(KeyValuePair<RoleType, int> scp)
        {
            Room room939 = Server.Get.Map.Rooms.First((e) => e.RoomType == RoomType.HCZ_939);

            foreach (var door in room939.Doors)
                door.Locked = true;

            _runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
            {
                foreach (var door in room939.Doors)
                    door.Locked = false;

                ToggleLockdownState(RoleType.Scp93953);

                foreach (var player in Server.Get.Players.Where((player) => player.RoleType == RoleType.Scp93953 || player.RoleType == RoleType.Scp93989))
                {
                    player.GiveTextHint("You're good to go!");
                }
            }));
            return Task.CompletedTask;
        }
        private void ToggleLockdownState(RoleType role)
        {
            switch (role)
            {
                case RoleType.Scp173:
                    _lockedDownScps[RoleType.Scp173] = !_lockedDownScps[RoleType.Scp173];
                    break;
                case RoleType.Scp079:
                    _lockedDownScps[RoleType.Scp079] = !_lockedDownScps[RoleType.Scp079];
                    break;
                case RoleType.Scp096:
                    _lockedDownScps[RoleType.Scp096] = !_lockedDownScps[RoleType.Scp096];
                    break;
                case RoleType.Scp106:
                    _lockedDownScps[RoleType.Scp106] = !_lockedDownScps[RoleType.Scp106];
                    break;
                case RoleType.Scp049:
                    _lockedDownScps[RoleType.Scp049] = !_lockedDownScps[RoleType.Scp049];
                    break;
                case RoleType.Scp93953:
                case RoleType.Scp93989:
                    _lockedDownScps[RoleType.Scp93953] = !_lockedDownScps[RoleType.Scp93953];
                    _lockedDownScps[RoleType.Scp93989] = !_lockedDownScps[RoleType.Scp93989];
                    break;
            }
        }
    }
}
