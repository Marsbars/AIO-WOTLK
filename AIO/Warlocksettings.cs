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
public class Warlocksettings : Settings
{
    [Setting]
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Framelock")]
    [Description("switch Framelock if the fightingclass misses Spells")]
    public bool Framelock { get; set; }

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
    public int Lifetap { get; set; }

    [Setting]
    [DefaultValue(40)]
    [Category("Fight")]
    [DisplayName("Drain Life")]
    [Description("Tells on which Health % to use Drain Life")]
    public int Drainlife { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("Fight")]
    [DisplayName("Health Funnel")]
    [Description("Tells on which PetHealth % to use Health Funnel")]
    public int Healthfunnel { get; set; }

    private Warlocksettings()
    {
        Framelock = false;
        Fear = false;
        Lifetap = 20;
        Drainlife = 40;
        Healthfunnel = 50;
    }


    public static Warlocksettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("Warlocksettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("Warlocksettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("Warlocksettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<Warlocksettings>(AdviserFilePathAndName("Warlocksettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new Warlocksettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("Warlocksettings > Load(): " + e);
        }
        return false;
    }
}

