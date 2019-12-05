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
public class ShamanLevelSettings : Settings
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
    [DefaultValue(true)]
    [Category("Combat")]
    [DisplayName("Ghostwolf")]
    [Description("Use Ghostwolfform?")]
    public bool Ghostwolf { get; set; }

    [Setting]
    [DefaultValue(10)]
    [Category("Combat")]
    [DisplayName("Selfheal")]
    [Description("Set the Enemytreshold in % when to heal?")]
    [Percentage(true)]
    public int Enemylife { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Combat")]
    [DisplayName("Lightning Bolt")]
    [Description("Use LNB for Pull?")]
    public bool LNB { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Totem")]
    [DisplayName("Totemic Revall")]
    [Description("Use Totemic Recall?")]
    public bool UseTotemicCall { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Totem")]
    [DisplayName("Earthtotem")]
    [Description("Use Earthtotem?")]
    public bool UseEarthTotems { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Totem")]
    [DisplayName("Stone Skin Totem")]
    [Description("Use Stone Skin Totem?")]
    public bool UseStoneSkinTotem { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Totem")]
    [DisplayName("Fire Totems")]
    [Description("Use Fire Totems?")]
    public bool UseFireTotems { get; set; }

    [Category("Totem")]
    [DefaultValue(false)]
    [DisplayName("Use Magma Totem")]
    [Description("Use Magma Totem on multi aggro")]
    public bool UseMagmaTotem { get; set; }

    [Category("Totem")]
    [DefaultValue(true)]
    [DisplayName("Use Air totems")]
    [Description("Use Air totems")]
    public bool UseAirTotems { get; set; }

    [Category("Totem")]
    [DefaultValue(true)]
    [DisplayName("Use Water totems")]
    [Description("Use Water totems")]
    public bool UseWaterTotems { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Totem")]
    [DisplayName("Fire Nova")]
    [Description("Use Fire Nova?")]
    public bool UseFireNova { get; set; }

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

    private ShamanLevelSettings()
    {
        AssignTalents = true;
        TalentCodes = new List<string> { };
        UseDefaultTalents = true;
        Framelock = false;
        Delay = 50;
        Ghostwolf = true;
        Enemylife = 10;
        LNB = false;
        UseTotemicCall = true;
        UseEarthTotems = true;
        UseStoneSkinTotem = true;
        UseFireTotems = true;
        UseMagmaTotem = false;
        UseAirTotems = true;
        UseWaterTotems = true;
        UseFireNova = true;
    }


    public static ShamanLevelSettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("ShamanLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("ShamanLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("ShamanLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<ShamanLevelSettings>(AdviserFilePathAndName("ShamanLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new ShamanLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("ShamanLevelSettings > Load(): " + e);
        }
        return false;
    }
}