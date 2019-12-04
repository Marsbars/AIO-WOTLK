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
public class HunterBeastMasterySettings : Settings
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
    [DefaultValue(10)]
    [Category("Fight")]
    [DisplayName("Aspect of the Viper")]
    [Description("Set the your  Mana  Treshold when to use AotV")]
    [Percentage(true)]
    public int AspecofViper { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Pet")]
    [DisplayName("Pet Feeding")]
    [Description("Want the Pet get Autofeeded?")]
    public bool Petfeed { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Pet")]
    [DisplayName("Pet Healing OOC")]
    [Description("Should Check Pet Health for Heal OOC?")]
    public bool Checkpet { get; set; }

    [Setting]
    [DefaultValue(80)]
    [Category("Pet")]
    [DisplayName("Pet Health OOC")]
    [Description("Set Treshhold for Pet Heal OOC?")]
    [Percentage(true)]
    public int PetHealth { get; set; }

    public static HunterBeastMasterySettings CurrentSetting { get; set; }

    private HunterBeastMasterySettings()
    {

        Framelock = false;
        PetHealth = 80;
        AspecofViper = 20;
        Checkpet = true;
        Petfeed = true;
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
            Logging.WriteError("HunterBeastMasterySettings > Save(): " + e);
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
                    Load<HunterBeastMasterySettings>(AdviserFilePathAndName("Hunter Settings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new HunterBeastMasterySettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("HunterBeastMasterySettings > Load(): " + e);
        }
        return false;
    }
}

