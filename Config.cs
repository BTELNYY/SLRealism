﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRealism
{
    public class Config
    {
        [Description("Is the plugin enabled?")]
        public bool PluginEnabled { get; set; } = true;

        [Description("Apply bleeding effect when player is hit by 939s claw?")]
        public bool ApplyBleedingPer939Claw { get; set; } = true;

        [Description("Nullify the extra 20 damage bleeding gives you.")]
        public bool FixBleedingDamage { get; set; } = true;

        [Description("A magic number used to get how long should the bleeding effect last based on the damage dealt, this specific value effects SCP 939s claw attack. Lower number here means lower amount of time of bleeding effect. Formula is 40 x value = seconds of bleed. On default settings, this is 24 seconds, or roughy 45-50 HP lost")]
        public float DamageToBleedingMultiplier939 { get; set; } = 0.60f;

        [Description("Should headshots instantly kill? Note, when turned on this is unfair, it makes the crossvec better then anything else.")]
        public bool OneShotHeadshot { get; set; } = false;

        [Description("Should one shot headshot be applied to zombies?")]
        public bool OneShotHeadshotAppliesToZombies { get; set; } = false;

        [Description("Apply the disabled debuff after taking fall damage? (halves movement speed)")]
        public bool ApplyDisabledOnFallDamage { get; set; } = true;

        [Description("Used to calculate how long the disabled status effect should last. Formula is: fall damage x value = seconds of disabled effect. (Thats the RA name of the effect.)")]
        public float DisabledDurationMulitplier { get; set; } = 0.6f;

        [Description("Apply bleeding status effect when a player is shot in the limbs or chest, does not apply to the head.")]
        public bool ApplyBleedingOnBulletDamage { get; set; } = true;

        [Description("Bleeding duration multiplier. Formula is: damage x value = duration. Note that duration stacks, meaning that getting shot many times results in a long bleed time.")]
        public float BleedingMultiplierGunshots { get; set; } = 0.3f;

        [Description("Multiplier for bleeding duration for limbshots. Formula is damage x value = duration")]
        public float BleedingMultiplierLimbshots { get; set; } = 0.2f;
        

        [Description("Mimics how humans have adrenaline when injured, meant to somewhat balance out the absurd amount of damage guns can do with bleeding. THIS SETTING DOES NOTHING, IT IS DISABLED AND DOES NOT WORK CORRECTLY!")]
        public bool ApplyAdrenalineOnDamage { get; set; } = false;

        [Description("Mulitplier for how much adrenaline to give depending on damage. Formula is: damage x value = adrenaline hp")]
        public float AdrenalineDamageMultiplier { get; set; } = 0.2f;
        
        [Description("Should SCP 914 kill players on rough? if the above flag is true, this setting does nothing.")]
        public bool Scp914KillsOnRough { get; set; } = true;

        [Description("Should 914 give status effects to players in the machine when its set to course?")]
        public bool Scp914AfflictsEffectsOnCourse { get; set; } = true;

        [Description("How long should 914 course given effects last?")]
        public float Scp914EffectLength { get; set; } = 12f;

        [Description("Should 914 apply the SCP:CB effect to players on very fine? (Very fast speed followed by instant death)")]
        public bool Scp914AppliesVeryFine { get; set; } = true;

        [Description("How long should players remain alive after using 914 on very fine?")]
        public float Scp914VeryFineLifeTime { get; set; } = 30f;


        [Description("Enable April fools event occurences?")]
        public bool AprilFoolsEnabled { get; set; } = false;

        [Description("April fools only, how much speed should zombies get when they are resurrected or the plugin forces them to change?")]
        public int ZombieSpeedBoostAmount { get; set; } = 11;

        [Description("April fools only, Should SCP 914 make humans become SCP 049-2 on rough?")]
        public bool Scp914RoughMakesZombies { get; set; } = true;

        [Description("April fools only, can you pull pink candy from SCP 330?")]
        public bool AprilFoolsPinkCandyExists { get; set; } = true;

        [Description("April fools only, What percent chance should pink candy have of being pulled from SCP 330? (Use decimal notation for percent, 1.0 = 100%, 0.5 = 50%, etc.)")]
        public float AprilFoolsPinkCandyChance { get; set; } = 0.3f;

        [Description("Should eating more then a certain amount of painkillers give you poison?")]
        public bool EnablePainkillerOverdose { get; set; } = true;

        [Description("Painkiller overdose amount, how many should you take before you die?")]
        public int PainkillerOverdoseAmount { get; set; } = 3;

        [Description("How long should you be given poison when overdosed?")]
        public float PainKillerOverdoseLength { get; set; } = 60;
    }
}
