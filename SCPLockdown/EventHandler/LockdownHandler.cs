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

namespace ScpLockdown.EventHandlers
{
    public class RoundHandler
    {
        private List<KeyValuePair<Player, CoroutineHandle>> larryCoroutines;
        private List<CoroutineHandle> runningCoroutines;
        private bool scp079Interactable;
        private Config config;
        public RoundHandler(Config config)
        {
            this.config = config;
            runningCoroutines = new List<CoroutineHandle>();
            larryCoroutines = new List<KeyValuePair<Player, CoroutineHandle>>();
            scp079Interactable = true;
        }

        public void OnRoundStart()
        {
            var configScpList = new Dictionary<RoleType, int>();

            //Filter unique RoleTypes
            foreach (var entry in config.AffectedScps)
            {
                if (!configScpList.Select((e) => e.Key).Contains(entry.Key))
                    configScpList.Add(entry.Key, entry.Value);
            }

            runningCoroutines.Add(Timing.CallDelayed(1, () =>
            {
                foreach (var scp in configScpList)
                {
                    switch (scp.Key)
                    {
                        case RoleType.Scp173:
                            Door door173 = Map.Doors.First((e) => String.Equals(e.DoorName, "173"));
                            var heavyDoor173 = Map.Doors.GetClosestDoor(door173, true);
                            heavyDoor173.locked = true;
                            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () => { heavyDoor173.locked = false; }));
                            break;
                        case RoleType.Scp079:
                            scp079Interactable = false;
                            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () =>
                            {
                                scp079Interactable = true;
                            }));
                            break;
                        case RoleType.Scp106:
                            foreach (var player in Player.List.Where((e) => e.Role == RoleType.Scp106))
                            {
                                var prevPos = player.Position;
                                player.SendToPocketDimension();
                                player.IsInvisible = true;

                                larryCoroutines.Add(new KeyValuePair<Player, CoroutineHandle>(player, Timing.CallDelayed(scp.Value, () =>
                                {
                                    player.Position = prevPos;
                                    player.IsInvisible = false;
                                })));
                            }
                            break;
                        case RoleType.Scp049:
                            Door door049 = Map.Doors.First((e) => String.Equals(e.DoorName, "049_ARMORY"));
                            var heavyDoor049 = Map.Doors.GetClosestDoor(door049);
                            heavyDoor049.locked = true;
                            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () => { heavyDoor049.locked = false; }));
                            break;
                        case RoleType.Scp096:
                            Door door096 = Map.Doors.First((e) => e.DoorName == "096");
                            Door nearestDoor = Map.Doors.GetClosestDoor(door096);
                            nearestDoor.locked = true;
                            runningCoroutines.Add(Timing.CallDelayed(scp.Value, () => { nearestDoor.locked = false; }));
                            break;
                        case RoleType.Scp93953:
                        case RoleType.Scp93989:
                            if (Player.List.Any((e) => e.Role == RoleType.Scp93953 || e.Role == RoleType.Scp93989))
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
                                }));
                            }
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
            => ev.IsAllowed = scp079Interactable;
        public void OnInteractingDoor(InteractingDoorEventArgs ev)
            => ev.IsAllowed = scp079Interactable;
    }
}
