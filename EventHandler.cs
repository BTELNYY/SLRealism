using PluginAPI.Core;
using PluginAPI;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;
using PlayerStatsSystem;
using PlayerRoles;
using InventorySystem.Disarming;
using PlayerRoles.PlayableScps.Scp939;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using YamlDotNet.Core.Tokens;
using static Steamworks.InventoryItem;

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
                switch (handle.Hitbox)
                {
                    case HitboxType.Headshot:
                        if(_config.OneShotHeadshot && player.RoleBase.Team != Team.SCPs)
                        {
                            player.Kill();
                            break;
                        }
                        if (player.RoleBase.RoleTypeId == RoleTypeId.Scp0492 && _config.OneShotHeadshotAppliesToZombies)
                        {
                            player.Kill();
                            break;
                        }
                    break;
                    case HitboxType.Body:
                        if (player.RoleBase.Team != Team.SCPs && _config.ApplyBleedingOnBulletDamage)
                        {
                            player.EffectsManager.ChangeState("bleeding", 1, (handle.Damage * _config.BleedingMultiplierGunshots), true);
                        }
                    break;
                    case HitboxType.Limb:
                        if (player.RoleBase.Team != Team.SCPs && _config.ApplyBleedingOnBulletDamage)
                        {
                            player.EffectsManager.ChangeState("bleeding", 1, (handle.Damage * _config.BleedingMultiplierLimbshots), true);
                        }
                    break;
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

        [PluginEvent(ServerEventType.PlayerDamage)]
        public void AdrenalineDamage(Player player, Player attacker, DamageHandlerBase damageHanlder)
        {
            if(damageHanlder is UniversalDamageHandler handler)
            {
                if(_config.ApplyAdrenalineOnDamage)
                {   
                    ((AhpStat)player.ReferenceHub.playerStats.StatModules[1]).ServerTryGetProcess(0, out AhpStat.AhpProcess proc);
                    if (proc == null)
                    {
                        Log.Debug("Procc is null.");
                        AhpStat.AhpProcess  procc1 = ((AhpStat)player.ReferenceHub.playerStats.StatModules[1]).ServerAddProcess((float)(_config.AdrenalineDamageMultiplier * handler.Damage));
                        procc1.CurrentAmount = _config.AdrenalineDamageMultiplier * handler.Damage;
                        Log.Debug(procc1.KillCode + " " + procc1.CurrentAmount);
                    }
                    else
                    {
                        Log.Debug("Procc is not null.");
                        proc.CurrentAmount += (float)(_config.AdrenalineDamageMultiplier * handler.Damage);
                    }
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerGameConsoleCommandExecuted)]
        public void OnCommand(Player player, string command, string[] arguments, bool result, string response)
        {
            Log.Info("Command executed: " + command);
        }
    }
}
