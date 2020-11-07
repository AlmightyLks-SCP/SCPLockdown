using Exiled.API.Features;
using HarmonyLib;
using ScpLockdown.DirtyWorkaround;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ScpLockdown.Patch
{
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdInteract))]
    internal static class Scp079Patch
    {
        static bool Prefix(string command, GameObject target)
             => (String.Equals(command.Split(':')[0], "DOORLOCK") || String.Equals(command.Split(':')[0], "ELEVATORUSE")) && LockdownStates.Scp079LockedUp ? false : true;

    }
}
