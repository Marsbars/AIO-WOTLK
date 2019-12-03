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


    private MageLevelSettings()
    {
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

