using SynapseDoor = Synapse.Api.Door;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Synapse.Api;
using Synapse.Api.Enum;

namespace ScpLockdown.Helper
{
    public static class Helper
    {
        public static readonly Vector3 PocketDimensionPosition = new Vector3(0, -1998.67f, 2);
        public static void SendToPocketDimension(this Player player)
            => player.Position = PocketDimensionPosition;

        public static SynapseDoor GetClosestDoor(
            this IEnumerable<SynapseDoor> doors,
            SynapseDoor relativeDoor,
            bool onlyHeavyDoors = false,
            IEnumerable<SynapseDoor> ignoreDoors = null
            )
        {
            ignoreDoors = ignoreDoors ?? Enumerable.Empty<SynapseDoor>();

            SynapseDoor result = null;

            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = relativeDoor.Position;

            foreach (SynapseDoor potentialDoor in doors)
            {
                if (onlyHeavyDoors && !potentialDoor.DoorType.IsHeavy())
                    continue;
                if (ignoreDoors.Contains(potentialDoor))
                    continue;
                if (potentialDoor == relativeDoor)
                    continue;

                Vector3 directionToTarget = potentialDoor.Position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    result = potentialDoor;
                }
            }

            return result;
        }

        public static SynapseDoor GetClosestDoor(
            this IEnumerable<SynapseDoor> doors,
            Room relativeRoom,
            bool onlyHeavyDoors = false,
            IEnumerable<SynapseDoor> ignoreDoors = null
            )
        {
            ignoreDoors = ignoreDoors ?? Enumerable.Empty<SynapseDoor>();

            SynapseDoor result = null;

            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = relativeRoom.Position;

            foreach (SynapseDoor potentialDoor in doors)
            {
                if (onlyHeavyDoors && !potentialDoor.DoorType.IsHeavy())
                    continue;
                if (ignoreDoors.Contains(potentialDoor))
                    continue;

                Vector3 directionToTarget = potentialDoor.Position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    result = potentialDoor;
                }
            }

            return result;
        }

        public static bool IsHeavy(this DoorType type)
            => type == DoorType.Gate_A ||
            type == DoorType.Gate_B ||
            type == DoorType.HCZ_049_Gate ||
            type == DoorType.LCZ_173_Gate ||
            type == DoorType.Surface_Gate;
    }
}
