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
public class Huntersettings : Settings
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
    [Category("Fight")]
    [DisplayName("Aspect of the Cheetah")]
    [Description("Should use ASpect of Cheetah before level 20?")]
    public bool Cheetah { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fight")]
    [DisplayName("Disengage")]
    [Description("Use  Disengage?")]
    public bool Dis { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Pet")]
    [DisplayName("Pet Health")]
    [Description("Should Check Pet Health beore attack?")]
    public bool Checkpet { get; set; }

    [Setting]
    [DefaultValue(80)]
    [Category("Pet")]
    [DisplayName("Pet Health")]
    [Description("Set Treshhold for Petattack?")]
    public int PetHealth { get; set; }

    public static Huntersettings CurrentSetting { get; set; }

    private Huntersettings()
    {

        Framelock = false;
        PetHealth = 80;
        Checkpet = true;
        Cheetah = true;
        Dis = false;
        Delay = 50;

    }

    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("Hunter Settings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("Huntersettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("Hunter Settings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<Huntersettings>(AdviserFilePathAndName("Hunter Settings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new Huntersettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("HunterSettings > Load(): " + e);
        }
        return false;
    }
}

