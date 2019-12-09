using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using MarsSettingsGUI;

[Serializable]
public class PriestLevelSettings : Settings
{
    [Setting]
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Framelock")]
    [Description("activate Framelock")]
    public bool Framelock { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("General")]
    [DisplayName("Use Wand?")]
    [Description("Use Wand in General?")]
    public bool UseWand { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("General")]
    [DisplayName("Use Wand Treshold?")]
    [Description("Enemy Life Treshold for Wandusage?")]
    [Percentage(true)]
    public int UseWandTresh { get; set; }
    
    [Setting]
    [DefaultValue(50)]
    [Category("General")]
    [DisplayName("Delay")]
    [Description("Set your Delay in MS (for bad PC´s) ")]
    public int Delay { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("Use Devouring Plague?")]
    [Description("Use DP up to level 80?")]
    public bool ShadowDPUse { get; set; }

    [Setting]
    [DefaultValue(75)]
    [Category("Shadow")]
    [DisplayName("Use Shield Treshold?")]
    [Description("Own life for Shield Usage?")]
    [Percentage(true)]
    public int ShadowUseShieldTresh { get; set; }

    [Setting]
    [DefaultValue(90)]
    [Category("Shadow")]
    [DisplayName("Use Renew Treshold?")]
    [Description("Own life for Renew Usage?")]
    [Percentage(true)]
    public int ShadowUseRenewTresh { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("Shadow")]
    [DisplayName("Use Lesser Heal/Flash Heal Treshold?")]
    [Description("Own life for Lesser Heal/Flash Heal Usage?")]
    [Percentage(true)]
    public int ShadowUseLessFlashTresh { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Shadow")]
    [DisplayName("Use Mindflay?")]
    [Description("Use Mindflay in General?")]
    public bool ShadowUseMindflay { get; set; }

    [Setting]
    [DefaultValue(95)]
    [Category("Holy")]
    [DisplayName("Tank - Use Prayer of Mending")]
    [Description("Set the Treshhold for Prayer of Mending?")]
    [Percentage(true)]
    public int HolyPrayerofMendingTreshTank { get; set; }

    [Setting]
    [DefaultValue(95)]
    [Category("Holy")]
    [DisplayName("Tank - Use Renew Treshold")]
    [Description("Set the Treshhold for renew?")]
    [Percentage(true)]
    public int HolyUseRenewTreshTank { get; set; }

    [Setting]
    [DefaultValue(70)]
    [Category("Holy")]
    [DisplayName("Tank - Use Binding Heal")]
    [Description("Set the Treshhold for Binding Heal?")]
    [Percentage(true)]
    public int HolyBindingHealTreshTank { get; set; }

    [Setting]
    [DefaultValue(70)]
    [Category("Holy")]
    [DisplayName("Tank - Use Greater Heal when Serendipity")]
    [Description("Set the Treshhold for Greater Heal?")]
    [Percentage(true)]
    public int HolyGreaterHealTreshTank { get; set; }

    [Setting]
    [DefaultValue(80)]
    [Category("Holy")]
    [DisplayName("Group - Use Circle of Heal")]
    [Description("Set the Treshhold for Circle of Healing?")]
    [Percentage(true)]
    public int HolyCircleofHealingTreshGroup { get; set; }

    [Setting]
    [DefaultValue(80)]
    [Category("Holy")]
    [DisplayName("Group - Use Prayer of Mending")]
    [Description("Set the Treshhold for Prayer of Mending?")]
    [Percentage(true)]
    public int HolyPrayerofMendingTreshGroup { get; set; }

    [Setting]
    [DefaultValue(70)]
    [Category("Holy")]
    [DisplayName("Group - Use Prayer of Healing")]
    [Description("Set the Treshhold for Prayer of Healing?")]
    [Percentage(true)]
    public int HolyPrayerofHealingTreshGroup { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("Holy")]
    [DisplayName("Group - Use Guardian Spirit")]
    [Description("Set the Treshhold for Guardian Spirit?")]
    [Percentage(true)]
    public int HolyGuardianSpiritTreshGroup { get; set; }

    [Setting]
    [DefaultValue(3)]
    [Category("Holy")]
    [DisplayName("Group - Use Guardian Spirit")]
    [Description("Set the Count of Player for Guardian Spirit?")]
    [Percentage(false)]
    public int HolyGuardianSpiritCountTreshGroup { get; set; }

    [Category("Holy")]
    [DefaultValue(false)]
    [DisplayName("Damage Spells")]
    [Description("If True, do Damage mostly for afk Botbases")]
    public bool HolyDamage { get; set; }

    [Category("Talents")]
    [DisplayName("Talents Codes")]
    [Description("Use a talent calculator to generate your own codes: https://talentcalculator.org/wotlk/. " +
        "Do not modify if you are not sure.")]
    public List<string> TalentCodes { get; set; }

    [Category("Talents")]
    [DefaultValue(true)]
    [DisplayName("Use default talents")]
    [Description("If True, Make sure your talents match the default talents, or reset your talents.")]
    public bool UseDefaultTalents { get; set; }

    [Category("Talents")]
    [DefaultValue(false)]
    [DisplayName("Auto assign talents")]
    [Description("Will automatically assign your talent points.")]
    public bool AssignTalents { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Talents")]
    [DisplayName("Talent Tree")]
    [Description("Choose which Talent Tree you want for leveling")]
    [DropdownList(new string[] { "PriestShadow", "PriestHoly", "PriestDiscipline" })]
    public string ChooseTalent { get; set; }

    public static PriestLevelSettings CurrentSetting { get; set; }

    private PriestLevelSettings()
    {
        AssignTalents = false;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        ChooseTalent = "PriestShadow";
        UseWand = true;
        UseWandTresh = 20;
        ShadowUseShieldTresh = 75;
        ShadowUseRenewTresh = 90;
        ShadowUseLessFlashTresh = 20;
        ShadowUseMindflay = true;
        ShadowDPUse = true;
        HolyPrayerofMendingTreshTank = 95;
        HolyUseRenewTreshTank = 95;
        HolyBindingHealTreshTank = 70;
        HolyGreaterHealTreshTank = 50;
        HolyGuardianSpiritTreshGroup = 50;
        HolyCircleofHealingTreshGroup = 80;
        HolyPrayerofMendingTreshGroup = 80;
        HolyPrayerofHealingTreshGroup = 70;
        HolyDamage = false;
        Framelock = false;
        Delay = 50;

    }

    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("PriestLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("PriestLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("PriestLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<PriestLevelSettings>(AdviserFilePathAndName("PriestLevelSettings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new PriestLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("PriestLevelSettings > Load(): " + e);
        }
        return false;
    }
}

