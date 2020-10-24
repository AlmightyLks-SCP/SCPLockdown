using EXPlayerEvents = Exiled.Events.Handlers.Player;
using EXServerEvents = Exiled.Events.Handlers.Server;
using EX079Events = Exiled.Events.Handlers.Scp079;
using Exiled.API.Enums;
using Exiled.API.Features;
using ScpLockdown.EventHandlers;
using System;
using System.Linq;
using HarmonyLib;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace ScpLockdown
{
    public class ScpLockdown : Plugin<Config>
    {
        public Harmony Harmony { get; private set; } = new Harmony("ScpLockdown.1");
        public override PluginPriority Priority { get; } = PluginPriority.High;

        private RoundHandler _lockdownHandler;
        
        public override void OnEnabled()
        {
            Log.Info("<AlmightyLks> SCPLockdown enabled");

            RegisterEvents();
            Patch();

            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            UnRegisterEvents();
            Unpatch();

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            _lockdownHandler = new RoundHandler(this);

            EXServerEvents.RoundStarted += _lockdownHandler.OnRoundStart;
            EXPlayerEvents.ChangingRole += _lockdownHandler.OnChangingRole;
            EXPlayerEvents.EscapingPocketDimension += _lockdownHandler.OnEscapingPocketDimension;
            EXPlayerEvents.FailingEscapePocketDimension += _lockdownHandler.OnFailingEscapePocketDimension;
            EX079Events.InteractingDoor += _lockdownHandler.OnInteractingDoor;
            EX079Events.InteractingTesla += _lockdownHandler.OnInteractingTesla;
            EXServerEvents.RoundEnded += _lockdownHandler.OnRoundEnded;
        }
        private void UnRegisterEvents()
        {
            EXServerEvents.RoundStarted -= _lockdownHandler.OnRoundStart;
            EXPlayerEvents.ChangingRole -= _lockdownHandler.OnChangingRole;
            EXPlayerEvents.EscapingPocketDimension -= _lockdownHandler.OnEscapingPocketDimension;
            EXPlayerEvents.FailingEscapePocketDimension -= _lockdownHandler.OnFailingEscapePocketDimension;
            EX079Events.InteractingDoor -= _lockdownHandler.OnInteractingDoor;
            EX079Events.InteractingTesla -= _lockdownHandler.OnInteractingTesla;
            EXServerEvents.RoundEnded -= _lockdownHandler.OnRoundEnded;

            _lockdownHandler = null;
        }

        private void Patch()
        {
            try
            {
                var lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;

                Harmony.PatchAll();

                Harmony.DEBUG = lastDebugStatus;

                Log.Debug("Patches applied successfully", Loader.ShouldDebugBeShown);
            }
            catch (Exception e)
            {
                Log.Error($"Patching failed {e}");
            }
        }
        private void Unpatch()
        {
            Harmony.UnpatchAll();
        }
    }
}
