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
using Scp914;
using UnityEngine;
using PlayerRoles.FirstPersonControl;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp330;
using MapGeneration;
using System.Linq;
using PluginAPI.Core.Zones;
using PluginAPI.Core.Doors;
using InventorySystem.Items.Jailbird;
using Mirror;
using System.Collections.Generic;

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

        [PluginEvent(ServerEventType.Scp049ResurrectBody)]
        public void OnZombieSpawn(Player player, Player target, BasicRagdoll ragdoll)
        {
            if(_config.AprilFoolsEnabled)
            {
                if(target.RoleBase.RoleTypeId == RoleTypeId.Scp0492)
                {
                    target.EffectsManager.ChangeState("MovementBoost", (byte)_config.ZombieSpeedBoostAmount, 0, false);
                }
            }
        }

        [PluginEvent(ServerEventType.Scp914ProcessPlayer)]
        public void On914Rough(Player player, Scp914KnobSetting knobSetting, Vector3 outPosition)
        {
            if (knobSetting == Scp914KnobSetting.Rough)
            {
                if (_config.Scp914RoughMakesZombies)
                {
                    //SCPs cannot die to 914
                    if (player.Role.IsHuman() && _config.AprilFoolsEnabled)
                    {
                        player.ReferenceHub.inventory.ServerDropEverything();
                        player.SetRole(RoleTypeId.Scp0492);
                        player.EffectsManager.ChangeState("MovementBoost", (byte)_config.ZombieSpeedBoostAmount, 0, false);
                        player.ReferenceHub.TryOverridePosition(outPosition, new Vector3());
                        return;
                    }
                }
                //april fools does not have to be enabled for this one to work, however making zombies overrides it.
                if (_config.Scp914KillsOnRough)
                {
                    if (player.Role.IsHuman())
                    {
                        player.ReferenceHub.TryOverridePosition(outPosition, new Vector3());
                        player.Kill();
                        return;
                    }
                }
            }
        }


        [PluginEvent(ServerEventType.PlayerPickupScp330)]
        public void Scp330Interact(Player player, ItemPickupBase item)
        {
            if (!_config.AprilFoolsEnabled)
            {
                return;
            }
            if (Random.Range(0f, 1.0f) <= _config.AprilFoolsPinkCandyChance)
            {
                var bag = player.ReferenceHub.inventory.ServerAddItem(ItemType.SCP330) as Scp330Bag;
                bag.TryAddSpecific(CandyKindID.Pink);
                bag.ServerRefreshBag();
            }
        }
    }
}
