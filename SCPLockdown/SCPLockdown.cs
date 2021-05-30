using ScpLockdown.Handlers;
using System;
using HarmonyLib;
using Synapse.Api.Plugin;
using System.Collections.Generic;
using ScpLockdown.Configs;
using System.Linq;
using ScpLockdown.Services;
using ScpLockdown.Patches;

namespace ScpLockdown
{
    [PluginInformation(
        Author = "AlmightyLks",
        Description = "Lock down all the Scps at the beginning of the round",
        Name = "ScpLockdown",
        SynapseMajor = 2,
        SynapseMinor = 6,
        SynapsePatch = 0,
        Version = "2.0.0"
        )]
    public sealed class ScpLockdown : AbstractPlugin
    {
        [Config(section = "ScpLockdown")]
        public LockdownConfigs Config { get; set; }
        public LockdownService LockdownService { get; private set; }

        private Harmony _harmony;
        private LockdownEventHandler _lockdownHandler;

        public override void Load()
        {
            Scp079Patch.Plugin = this;
            LockdownService = new LockdownService(this);
            _harmony = new Harmony("ScpLockdown.Patches");
            LegitimateConfig();
            RegisterEvents();
            Patch();
        }
        public override void ReloadConfigs()
        {
            LegitimateConfig();
        }

        private void LegitimateConfig()
        {
            var configScpListLegit = new Dictionary<RoleType, int>();

            //Filter unique RoleTypes
            foreach (var entry in Config.AffectedScps)
            {
                if (!configScpListLegit.Select((e) => e.Key).Contains(entry.Key))
                    configScpListLegit.Add(entry.Key, entry.Value);
            }

            Config.AffectedScps = configScpListLegit;
        }
        private void RegisterEvents()
        {
            _lockdownHandler = new LockdownEventHandler(this);

            Synapse.Api.Events.EventHandler.Get.Round.RoundStartEvent += _lockdownHandler.OnRoundStart;
            Synapse.Api.Events.EventHandler.Get.Player.PlayerSetClassEvent += _lockdownHandler.OnSetClass;
            Synapse.Api.Events.EventHandler.Get.Scp.Scp106.PocketDimensionLeaveEvent += _lockdownHandler.OnEscapingPocketDimension;
            Synapse.Api.Events.EventHandler.Get.Map.TriggerTeslaEvent += _lockdownHandler.OnInteractingTesla;
            Synapse.Api.Events.EventHandler.Get.Round.RoundEndEvent += _lockdownHandler.OnRoundEnd;
        }
        private void Patch()
        {
            try
            {
                var lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
                _harmony.PatchAll();
                Harmony.DEBUG = lastDebugStatus;

                Synapse.Api.Logger.Get.Info("Patches applied successfully");
            }
            catch (Exception e)
            {
                Synapse.Api.Logger.Get.Error($"Patching failed {Environment.NewLine}{e}");
            }
        }
    }
}
