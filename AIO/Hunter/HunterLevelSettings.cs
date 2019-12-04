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
public class HunterLevelSettings : Settings
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
    [DefaultValue(20)]
    [Category("Fight")]
    [DisplayName("Aspect of the Viper")]
    [Description("Set the your  Mana  Treshold when to use AotV")]
    [Percentage(true)]
    public int AspecofViper { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fight")]
    [DisplayName("Disengage")]
    [Description("Use  Disengage?")]
    public bool Dis { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fight")]
    [DisplayName("Multishot")]
    [Description("Use  Multishot?")]
    public bool MultiS { get; set; }

    [Setting]
    [DefaultValue(99)]
    [Category("Fight")]
    [DisplayName("Pet Health in Fight")]
    [Description("Define when to heal Pet Infight!")]
    [Percentage(true)]
    public int PetmendInFight { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fight")]
    [DisplayName("Backpaddle")]
    [Description("Auto Backpaddle?")]
    public bool Backpaddle { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Pet")]
    [DisplayName("Pet Feeding")]
    [Description("Want the Pet get Autofeeded?")]
    public bool Petfeed { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Pet")]
    [DisplayName("Pet Health OOC")]
    [Description("Should Check Pet Health before attack?")]
    public bool Checkpet { get; set; }

    [Setting]
    [DefaultValue(80)]
    [Category("Pet")]
    [DisplayName("Pet Health OOC")]
    [Description("Set Treshhold for Petattack?")]
    [Percentage(true)]
    public int PetHealth { get; set; }

    public static HunterLevelSettings CurrentSetting { get; set; }

    private HunterLevelSettings()
    {

        Framelock = false;
        PetHealth = 80;
        AspecofViper = 20;
        PetmendInFight = 99;
        MultiS = false;
        Backpaddle = true;
        Checkpet = true;
        Cheetah = true;
        Petfeed = true;
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
            Logging.WriteError("HunterLevelSettings > Save(): " + e);
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
                    Load<HunterLevelSettings>(AdviserFilePathAndName("Hunter Settings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new HunterLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("HunterLevelSettings > Load(): " + e);
        }
        return false;
    }
}

