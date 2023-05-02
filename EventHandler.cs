using PluginAPI.Core;
using PluginAPI;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;
using PlayerStatsSystem;
using PlayerRoles;
using Scp914;
using UnityEngine;
using PlayerRoles.FirstPersonControl;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp330;
using InventorySystem.Items.Usables;
using System.Collections.Generic;
using CustomPlayerEffects;
using InventorySystem.Disarming;

namespace SLRealism
{
    public class EventHandler
    {
        public Dictionary<string, PlayerData> Data = new Dictionary<string, PlayerData> ();

        public PlayerData GetPlayerData(string id)
        {
            if (Data.TryGetValue(id, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                Data.Add(id, new PlayerData());
                return Data[id];
            }
        }

        public void SetPlayerData(string id, PlayerData playerData)
        {
            if (Data.ContainsKey(id))
            {
                Data[id] = playerData;
            }
            else
            {
                Data.Add(id, playerData);
            }
        }

        public PlayerData MakePlayerData(string id)
        {
            if (!Data.TryGetValue(id, out PlayerData data))
            {
                Data.Add(id, new PlayerData());
                return new PlayerData();
            }
            else
            {
                return new PlayerData();
            }
        }

        public void RemovePlayerData(string id)
        {
            Data.Remove(id);
        }


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
                            player.Damage(-1, "Bullet wound to the head");
                            break;
                        }
                        if (player.RoleBase.RoleTypeId == RoleTypeId.Scp0492 && _config.OneShotHeadshotAppliesToZombies)
                        {
                            player.Damage(-1, "Bullet wound to the head");
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
                    float amount = _config.AdrenalineDamageMultiplier * handler.Damage;
                    Log.Debug("Trying to give AHP...");
                    var ahpStat = player.ReferenceHub.playerStats.GetModule<AhpStat>();
                    var ahpProcess = ahpStat.ServerAddProcess(amount);
                    if (ahpProcess != null)
                        ahpProcess = new AhpStat.AhpProcess(amount, 75f, 1.2f, 0.7f, 0f, persistant: false);
                    else
                    {
                        ahpStat.ServerAddProcess(amount);
                    }
                }
                
                //attacker is null!
                //attacker.ReferenceHub.playerStats.GetModule<AhpStat>().ServerAddProcess(amount);
                /*
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
                */
            }
        }

        [PluginEvent(ServerEventType.PlayerGameConsoleCommandExecuted)]
        public void OnCommand(Player player, string command, string[] arguments, bool result, string response)
        {
            Log.Info("Command executed: " + command);
            switch(command)
            {
                case "suicide":
                    player.Damage(-1, "Committed suicide");
                    player.SendConsoleMessage("You killed yourself via console.", "yellow");
                    break;
                case "cuffme":
                    if (player.IsDisarmed)
                    {
                        player.SendConsoleMessage("You are already handcuffed, silly!", "red");
                    }
                    else
                    {
                        player.ReferenceHub.inventory.ServerDropEverything();
                        Server.RunCommand("disarm " + player.PlayerId);
                        player.SendConsoleMessage("You are now handcuffed, enjoy! ;)");
                    }
                    break;
            }
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

        [PluginEvent(ServerEventType.PlayerUsedItem)]
        public void OnUseItem(Player player, ItemBase item)
        {
            if(item is Painkillers && _config.EnablePainkillerOverdose)
            {
                PlayerData data = GetPlayerData(player.UserId);
                Log.Debug("the item is painkillers, and overdose is enabled!");
                Log.Debug("Amount of painkillers chugged: " + data.PainkillersUsed.ToString());
                if(data.PainkillersUsed >= _config.PainkillerOverdoseAmount)
                {
                    Log.Debug("Giving poison!");
                    player.EffectsManager.ChangeState("poisoned", 3, _config.PainKillerOverdoseLength, true);
                    data.PainkillersUsed = 0;
                    SetPlayerData(player.UserId, data);
                }
                else
                {
                    Log.Debug("Increased amount of painkiller chugs!");
                    data.PainkillersUsed++;
                    SetPlayerData(player.UserId, data);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerLeft)]
        public void OnUserLoad(Player player)
        {
            RemovePlayerData(player.UserId);
        }

        [PluginEvent(ServerEventType.Scp914ProcessPlayer)]
        public void On914Course(Player player, Scp914KnobSetting knobSetting, Vector3 outPosition)
        {
            if(knobSetting == Scp914KnobSetting.Coarse && _config.Scp914AfflictsEffectsOnCourse)
            {
                if (player.IsHuman)
                {
                    player.EffectsManager.ChangeState("cardiacarrest", 1, _config.Scp914EffectLength, true);
                    player.EffectsManager.ChangeState("disabled", 1, _config.Scp914EffectLength, true);
                }
            }
        }

        [PluginEvent(ServerEventType.PlayerReceiveEffect)]
        public void OnEffectGiven(Player player, StatusEffectBase effect, byte intensity, float duration)
        {
            if (_config.FixBleedingDamage)
            {
                if(effect is Bleeding)
                {
                    player.Heal(20f);
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
                        player.Damage(-1, "Body is chopped up and horribly mutilated");
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

    public struct PlayerData
    {
        public int PainkillersUsed;

        public PlayerData(int painkillersUsed = 0)
        {
            PainkillersUsed = painkillersUsed;
        }
    }
}
