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
    [Category("Pull")]
    [DisplayName("Deathgrip")]
    [Description("use Deathgrip for Pull?")]
    public bool DeathGrip { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Framelock")]
    [Description("switch Framelock if the fightingclass misses Spells")]
    public bool Framelock { get; set; }

    private DKSettings()
    {
        DeathGrip = true;
        Framelock = false;
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