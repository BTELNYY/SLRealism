using System;
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

        /**
        [Description("Mimics how humans have adrenaline when injured, meant to somewhat balance out the absurd amount of damage guns can do with bleeding.")]
        public bool ApplyAdrenalineOnDamage { get; set; } = true;

        [Description("Mulitplier for how much adrenaline to give depending on damage. Formula is: damage x value = adrenaline hp")]
        public float AdrenalineDamageMultiplier { get; set; } = 0.2f;
        **/
    }
}
