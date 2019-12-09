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
public class DeathKnightLevelSettings : Settings
{
    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("Deathgrip")]
    [Description("use Deathgrip for Pull?")]
    public bool DeathGrip { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("BloodPresence")]
    [Description("Autobuff BloodPresence?")]
    public bool BloodPresence { get; set; }

    [Setting]
    [DefaultValue(1)]
    [Category("General")]
    [DisplayName("Bloodstrike")]
    [Description("Set Enemy Count Equal X enemy to use Bloodstrike")]
    public int bloodstrike { get; set; }

    [Setting]
    [DefaultValue(2)]
    [Category("General")]
    [DisplayName("Hearthstrike")]
    [Description("Set Enemy Count Equal X enemy to use Hearthstrike")]
    public int hearthstrike { get; set; }

    [Setting]
    [DefaultValue(2)]
    [Category("General")]
    [DisplayName("BloodBoil")]
    [Description("Set Enemy Count larger X enemy to use Bloodstrike")]
    public int bloodboil { get; set; }

    [Setting]
    [DefaultValue(3)]
    [Category("General")]
    [DisplayName("Death and Decay")]
    [Description("Set Enemy Count larger X enemy to use DnD")]
    public int dnd { get; set; }

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
    [DefaultValue(true)]
    [DisplayName("Auto assign talents")]
    [Description("Will automatically assign your talent points.")]
    public bool AssignTalents { get; set; }
    private DeathKnightLevelSettings()
    {
        AssignTalents = true;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        DeathGrip = true;
        BloodPresence = true;
        bloodstrike = 1;
        hearthstrike = 2;
        bloodboil = 2;
        dnd = 3;
        Framelock = false;
        Delay = 50;
    }


    public static DeathKnightLevelSettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("DeathKnightLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("DeathKnightLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("DeathKnightLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<DeathKnightLevelSettings>(AdviserFilePathAndName("DeathKnightLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new DeathKnightLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("DeathKnightLevelSettings > Load(): " + e);
        }
        return false;
    }
}