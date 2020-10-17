using EXPlayerEvents = Exiled.Events.Handlers.Player;
using EXServerEvents = Exiled.Events.Handlers.Server;
using Exiled.API.Enums;
using Exiled.API.Features;
using ScpLockdown.EventHandlers;
using System;
using System.Linq;

namespace ScpLockdown
{
    public class ScpLockdown : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.High;

        private RoundHandler _roundHandler;

        public ScpLockdown() { }

        public override void OnEnabled()
        {
            Log.Info("<AlmightyLks> SCPLockdown enabled");
            RegisterEvents();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            UnRegisterEvents();
            base.OnDisabled();
        }
        private void RegisterEvents()
        {
            _roundHandler = new RoundHandler(Config);

            EXServerEvents.RoundStarted += _roundHandler.OnRoundStart;
            EXPlayerEvents.ChangingRole += _roundHandler.OnChangingRole;
            EXPlayerEvents.EscapingPocketDimension += _roundHandler.OnEscapingPocketDimension;
            EXPlayerEvents.FailingEscapePocketDimension += _roundHandler.OnFailingEscapePocketDimension;
            EXServerEvents.RoundEnded += _roundHandler.OnRoundEnded;
        }
        private void UnRegisterEvents()
        {
            EXServerEvents.RoundStarted -= _roundHandler.OnRoundStart;
            EXPlayerEvents.ChangingRole -= _roundHandler.OnChangingRole;
            EXPlayerEvents.EscapingPocketDimension -= _roundHandler.OnEscapingPocketDimension;
            EXPlayerEvents.FailingEscapePocketDimension -= _roundHandler.OnFailingEscapePocketDimension;
            EXServerEvents.RoundEnded -= _roundHandler.OnRoundEnded;

            _roundHandler = null;
        }
    }
}
