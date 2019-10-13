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

[Serializable]
public class Priestsettings : Settings
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
    public int UseShieldTresh { get; set; }

    [Setting]
    [DefaultValue(90)]
    [Category("Fight")]
    [DisplayName("Use Renew Treshold?")]
    [Description("Own life for Renew Usage?")]
    public int UseRenewTresh { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("Fight")]
    [DisplayName("Use Lesser Heal/Flash Heal Treshold?")]
    [Description("Own life for Lesser Heal/Flash Heal Usage?")]
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

    public static Priestsettings CurrentSetting { get; set; }

    private Priestsettings()
    {
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
            return Save(AdviserFilePathAndName("Priestsettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("Priestsettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("Priestsettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<Priestsettings>(AdviserFilePathAndName("Priestsettings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new Priestsettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("Priestsettings > Load(): " + e);
        }
        return false;
    }
}

