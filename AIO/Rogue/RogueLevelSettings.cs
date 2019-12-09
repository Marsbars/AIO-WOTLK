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
public class RogueLevelSettings : Settings
{
    [Setting]
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Framelock")]
    [Description("activate Framelock")]
    public bool Framelock { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("General")]
    [DisplayName("Delay")]
    [Description("Set your Delay in MS (for bad PC´s) ")]
    public int Delay { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fighting")]
    [DisplayName("Stealth")]
    [Description("Use Stealth?")]
    public bool Stealth { get; set; }

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
    [DropdownList(new string[] { "RogueCombat", "RogueAssasination", "RogueSublety" })]
    public string ChooseTalent { get; set; }

    public static RogueLevelSettings CurrentSetting { get; set; }

    private RogueLevelSettings()
    {
        AssignTalents = true;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        ChooseTalent = "RogueCombat";
        Framelock = false;
        Stealth = true;
        Delay = 50;

    }

    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("RogueLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("RogueLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("RogueLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<RogueLevelSettings>(AdviserFilePathAndName("RogueLevelSettings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new RogueLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("RogueLevelSettings > Load(): " + e);
        }
        return false;
    }
}

