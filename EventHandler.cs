using PluginAPI.Core;
using PluginAPI;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;
using PlayerStatsSystem;
using PlayerRoles;

namespace SLRealism
{
    public class EventHandler
    {
        public Config _config = SLRealism.instance.config;

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void OnPlayerClawed(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            if (!_config.ApplyBleedingPer939Claw || attacker == null || attacker.RoleBase.RoleTypeId != RoleTypeId.Scp939)
            {
                return;
            }
            player.EffectsManager.ChangeState("bleeding", 1, 40 * _config.DamageToBleedingMultiplier939, false);
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void OnPlayerShot(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            if (damageHandler is FirearmDamageHandler handle)
            {
                if(handle.Hitbox == HitboxType.Headshot || (player.RoleBase.RoleTypeId == RoleTypeId.Scp0492 && !_config.OneShotHeadshotAppliesToZombies))
                {
                    if(player.RoleBase.RoleTypeId == RoleTypeId.Scp0492 && _config.OneShotHeadshotAppliesToZombies)
                    {
                        player.Kill();
                    }
                }else if(player.RoleBase.Team != Team.SCPs)
                {
                    player.EffectsManager.ChangeState("bleeding", 1, (handle.Damage * _config.BleedingMultiplierGunshots), true);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void OnFallDamage(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            if (damageHandler is UniversalDamageHandler handle)
            { 
                if(handle.TranslationId != 6)
                {
                    return;
                }
                player.EffectsManager.ChangeState("disabled", 1, (handle.Damage * _config.DisabledDurationMulitplier), true);
            }
        }
    }
}
