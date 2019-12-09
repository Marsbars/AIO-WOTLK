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
public class MageLevelSettings : Settings
{
    [Setting]
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Framelock")]
    [Description("switch Framelock if the fightingclass misses Spells")]
    public bool Framelock { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("General")]
    [DisplayName("Delay")]
    [Description("Set your Delay in MS (for bad PC´s) ")]
    public int Delay { get; set; }

    [Setting]
    [DefaultValue(10)]
    [Category("Fight")]
    [DisplayName("Manstone")]
    [Description("Treshhold for Manastone")]
    [Percentage(true)]
    public int Manastone { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fight")]
    [DisplayName("Sheep")]
    [Description("Uses Sheep if 2 Targets attacking")]
    public bool Sheep { get; set; }

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
    [DropdownList(new string[] { "MageFrost", "MageFire", "MageArcane" })]
    public string ChooseTalent { get; set; }

    private MageLevelSettings()
    {
        AssignTalents = true;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        ChooseTalent = "MageFrost";
        Framelock = false;
        Sheep = false;
        Manastone = 10;
        Delay = 50;
    }


    public static MageLevelSettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("MageLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("MageLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("MageLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<MageLevelSettings>(AdviserFilePathAndName("MageLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new MageLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("MageLevelSettings > Load(): " + e);
        }
        return false;
    }
}

