using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;

namespace ScpLockdown.Patches
{
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdInteract))]
    internal static class Scp079Patch
    {
        internal static ScpLockdown Plugin { get; set; }
        private static bool Prefix(string command, GameObject target)
        {
            bool scp079Lockdown = Plugin.LockdownService.LockedDownScps.TryGetValue(RoleType.Scp079, out bool locked) && locked;
            string action = command.Split(':')[0];
            bool attemptedUse = action == "DOORLOCK" || action == "ELEVATORUSE" || action == "TESLA" || action == "DOOR";
            //If in lockdown & attempting to use something - disallow
            if (scp079Lockdown && attemptedUse)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
