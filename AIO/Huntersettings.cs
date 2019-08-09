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

[Serializable]
public class Huntersettings : Settings
{
    [Setting]
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Framelock")]
    [Description("activate Framelock")]
    public bool Framelock { get; set; }

    public static Huntersettings CurrentSetting { get; set; }

    private Huntersettings()
    {

        Framelock = false;

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
