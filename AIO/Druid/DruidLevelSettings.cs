﻿using robotManager.Helpful;
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
public class DruidLevelSettings : Settings
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
    [Category("Fighting")]
    [DisplayName("Prowl")]
    [Description("Use Prowl?")]
    public bool Prowl { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fighting")]
    [DisplayName("Tigers Fury")]
    [Description("Use Tigers Fury on Cooldown?")]
    public bool TF { get; set; }

    [Setting]
    [DefaultValue(1)]
    [Category("Fighting")]
    [DisplayName("Ferocious Bite")]
    [Description("Set the Combopoint, when to use FB?")]
    public int FBC { get; set; }

    [Setting]
    [DefaultValue(20)]
    [Category("Fighting")]
    [DisplayName("Ferocious Bite")]
    [Description("Set the Enemyhealth, when to use FB?")]
    [Percentage(true)]
    public int FBH { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fighting")]
    [DisplayName("Faerie Fire Feral")]
    [Description("Use FF in the Rotation?")]
    public bool FFF { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fighting")]
    [DisplayName("Dash")]
    [Description("Use Dash while stealthed?")]
    public bool Dash { get; set; }

    public static DruidLevelSettings CurrentSetting { get; set; }

    private DruidLevelSettings()
    {

        Framelock = false;
        Prowl = true;
        TF = false;
        FBC = 1;
        FBH = 30;
        FFF = true;
        Dash = true;
        Delay = 50;
    }

    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("DruidLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("DruidLevelSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("DruidLevelSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<DruidLevelSettings>(AdviserFilePathAndName("DruidLevelSettings",
                                                                 ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new DruidLevelSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("DruidLevelSettings > Load(): " + e);
        }
        return false;
    }
}
