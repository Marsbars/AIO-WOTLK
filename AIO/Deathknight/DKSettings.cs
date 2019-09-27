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
public class DKSettings : Settings
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

    private DKSettings()
    {
        DeathGrip = true;
        BloodPresence = true;
        Framelock = false;
        Delay = 50;
    }


    public static DKSettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("DKSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("DKSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("DKSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<DKSettings>(AdviserFilePathAndName("DKSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new DKSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("DKSettings > Load(): " + e);
        }
        return false;
    }
}