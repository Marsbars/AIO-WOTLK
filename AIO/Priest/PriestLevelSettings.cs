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
    [Category("Fight")]
    [DisplayName("Use Wand?")]
    [Description("Use Wand in General?")]
    public bool UseWand { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("Fight")]
    [DisplayName("Use Wand Treshold?")]
    [Description("Enemy Life Treshold for Wandusage?")]
    [Percentage(true)]
    public int UseWandTresh { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("Use Devouring Plague?")]
    [Description("Use DP up to level 80?")]
    public bool DPUse { get; set; }

    [Setting]
    [DefaultValue(75)]
    [Category("Fight")]
    [DisplayName("Use Shield Treshold?")]
    [Description("Own life for Shield Usage?")]
    [Percentage(true)]
    public int UseShieldTresh { get; set; }

    [Setting]
    [DefaultValue(90)]
    [Category("Fight")]
    [DisplayName("Use Renew Treshold?")]
    [Description("Own life for Renew Usage?")]
    [Percentage(true)]
    public int UseRenewTresh { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("Fight")]
    [DisplayName("Use Lesser Heal/Flash Heal Treshold?")]
    [Description("Own life for Lesser Heal/Flash Heal Usage?")]
    [Percentage(true)]
    public int UseLessFlashTresh { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("Use Mindflay?")]
    [Description("Use Mindflay in General?")]
    public bool UseMindflay { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("General")]
    [DisplayName("Delay")]
    [Description("Set your Delay in MS (for bad PC´s) ")]
    public int Delay { get; set; }

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

    public static PriestLevelSettings CurrentSetting { get; set; }

    private PriestLevelSettings()
    {
        AssignTalents = true;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        UseWand = true;
        UseWandTresh = 20;
        UseShieldTresh = 75;
        UseRenewTresh = 90;
        UseLessFlashTresh = 20;
        UseMindflay = true;
        DPUse = true;
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

