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
public class WarlockLevelSettings : Settings
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
    [DefaultValue(false)]
    [Category("Pet")]
    [DisplayName("Felguard")]
    [Description("Set Combat Aura")]
    [DropdownList(new string[] { "Felguard", "VoidWalker" })]
    public string Pet { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fight")]
    [DisplayName("Fear")]
    [Description("Uses Fear if 2 Targets attacking")]
    public bool Fear { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("Fight")]
    [DisplayName("Lifetap")]
    [Description("Tells on which Mana % to use Lifetap")]
    [Percentage(true)]
    public int Lifetap { get; set; }

    [Setting]
    [DefaultValue(40)]
    [Category("Fight")]
    [DisplayName("Drain Life")]
    [Description("Tells on which Health % to use Drain Life")]
    [Percentage(true)]
    public int Drainlife { get; set; }

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
    [DisplayName("Use Wand")]
    [Description("Use Wand?")]
    public bool UseWand { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("Fight")]
    [DisplayName("Health Funnel")]
    [Description("Tells on which PetHealth % to use Health Funnel")]
    [Percentage(true)]
    public int Healthfunnel { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("Unstable Affliction")]
    [Description("Use Unstable Affliction in Fight?")]
    public bool unstableaffl { get; set; }

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
    [DropdownList(new string[] { "WarlockAffliction", "WarlockDestruction", "WarlockDemonology" })]
    public string ChooseTalent { get; set; }

    private WarlockLevelSettings()
    {
        AssignTalents = true;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        ChooseTalent = "WarlockAffliction";
        Framelock = false;
        Delay = 50;
        Pet = "VoidWalker";
        Fear = false;
        Lifetap = 20;
        Drainlife = 40;
        Healthfunnel = 50;
        UseWandTresh = 20;
        UseWand = true;
        unstableaffl = true;
    }


    public static WarlockLevelSettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("WarlockLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("WarlockLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("WarlockLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<WarlockLevelSettings>(AdviserFilePathAndName("WarlockLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new WarlockLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("WarlockLevelSettings > Load(): " + e);
        }
        return false;
    }
}

