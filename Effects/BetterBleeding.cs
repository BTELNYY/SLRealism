using CustomPlayerEffects;
using Mirror;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI;
using PluginAPI.Core;

namespace SLRealism.Effects
{
    public class BetterBleeding : TickingEffectBase, IHealablePlayerEffect
    {
        public float Damage = 3f;

        public override bool AllowEnabling => true;


        public bool IsHealable(ItemType item)
        {
            if (item == ItemType.SCP500)
            {
                return true;
            }
            return item == ItemType.Medkit;
        }

        protected override void OnTick()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            Log.Debug("Tick!");
            Hub.playerStats.DealDamage(new UniversalDamageHandler(Damage * Intensity, DeathTranslations.Bleeding, null));
            Hub.playerEffectsController.ServerSendPulse<Bleeding>();
        }

        protected override void Enabled()
        {
            Log.Debug("Enabled!");
            if (!NetworkServer.active)
            {
                return;
            }
            IsEnabled = true;
            Hub.playerStats.DealDamage(new UniversalDamageHandler(Damage * base.Intensity, DeathTranslations.Bleeding, null));
        }
    }
}
