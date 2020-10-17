using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPLockdown.Helper
{
    public static class Helper
    {
        public static readonly Vector3 PocketDimensionPosition = new Vector3(0, -1998.67f, 2);
        public static void SendToPocketDimension(this Player player)
            => player.Position = PocketDimensionPosition;
        public static Door GetClosestDoor(this IEnumerable<Door> doors, Door relativeDoor, bool onlyHeavyDoors = false, IEnumerable<Door> ignoreDoors = null)
        {
            ignoreDoors = ignoreDoors ?? Enumerable.Empty<Door>();

            Door result = null;

            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = relativeDoor.gameObject.transform.position;

            foreach (Door potentialDoor in doors)
            {
                if (onlyHeavyDoors && potentialDoor.doorType != Door.DoorTypes.HeavyGate)
                    continue;
                if (ignoreDoors.Contains(potentialDoor))
                    continue;
                if (potentialDoor == relativeDoor)
                    continue;

                Vector3 directionToTarget = potentialDoor.gameObject.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    result = potentialDoor;
                }
            }

            return result;
        }
        public static Door GetClosestDoor(this IEnumerable<Door> doors, Room relativeRoom, bool onlyHeavyDoors = false, IEnumerable<Door> ignoreDoors = null)
        {
            ignoreDoors = ignoreDoors ?? Enumerable.Empty<Door>();

            Door result = null;

            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = relativeRoom.Transform.position;

            foreach (Door potentialDoor in doors)
            {
                if (onlyHeavyDoors && potentialDoor.doorType != Door.DoorTypes.HeavyGate)
                    continue;
                if (ignoreDoors.Contains(potentialDoor))
                    continue;

                Vector3 directionToTarget = potentialDoor.gameObject.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    result = potentialDoor;
                }
            }

            return result;
        }
    }
}
