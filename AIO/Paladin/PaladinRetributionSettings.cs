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
public class PaladinRetributionSettings : Settings
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
    [DefaultValue(false)]
    [Category("General")]
    [DisplayName("Crusader")]
    [Description("switch Crusader Aura")]
    public bool Crusader { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Fighting")]
    [DisplayName("Auto Buffing")]
    [Description("use Autobuffing?")]
    public bool Autobuffing { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fighting")]
    [DisplayName("Hammer of Justice")]
    [Description("Hammer of Justice when more then 1 Target")]
    public bool HammerofJustice { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Fighting")]
    [DisplayName("Hand of Reckoning")]
    [Description("Use Hand of Reckoning in Rotation?")]
    public bool HOR { get; set; }

    [Setting]
    [DefaultValue(50)]
    [Category("Healing")]
    [DisplayName("Holy Light")]
    [Description("Set your Treshhold when to use Holy Light")]
    [Percentage(true)]
    public int HL { get; set; }

    [Setting]
    [DefaultValue(30)]
    [Category("Healing")]
    [DisplayName("Flash of Light")]
    [Description("Set your Treshhold when to use Flash of Light")]
    [Percentage(true)]
    public int FL { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Healing")]
    [DisplayName("Purify")]
    [Description("Allow Purify on yourself")]
    public bool Purify { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Healing")]
    [DisplayName("Sacred Shield")]
    [Description("Allow the Use of Sacredshield")]
    public bool SShield { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Healing")]
    [DisplayName("Hand of Protection")]
    [Description("Allow the Use of Hand of Protection")]
    public bool HoProtection { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Healing")]
    [DisplayName("Lay on Hands")]
    [Description("Allow the Use of Lay on Hands")]
    public bool LayOnHands { get; set; }

    [Setting]
    [DefaultValue(true)]
    [Category("Healing")]
    [DisplayName("Divine Protection")]
    [Description("Allow the Use of Divine Protection")]
    public bool DivProtection { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Aura")]
    [DisplayName("Combat Aura")]
    [Description("Set Combat Aura")]
    [DropdownList(new string[] { "Devotion Aura", "Retribution Aura" })]
    public string Aura { get; set; }

    [Setting]
    [DefaultValue(false)]
    [Category("Seal")]
    [DisplayName("Seal of Command")]
    [Description("Set if you want Seal of command used by FC")]
    public bool SOC { get; set; }

    //[Setting]
    //[DefaultValue(false)]
    //[Category("Other")]
    //[DisplayName("Drawing on Screen")]
    //[Description("Allow the Use of Circles and Drawing Lines to Enemy Players")]
    //public bool Draw { get; set; }

    private PaladinRetributionSettings()
    {
        Framelock = false;
        Crusader = false;
        Autobuffing = true;
        HammerofJustice = true;
        HOR = true;
        Purify = true;
        SShield = true;
        HoProtection = true;
        DivProtection = true;
        //Draw = false;
        HL = 50;
        FL = 30;
        Aura = "Devotion Aura";
        SOC = false;
        Delay = 50;
    }


    public static PaladinRetributionSettings CurrentSetting { get; set; }


    public bool Save()
    {
        try
        {
            return Save(AdviserFilePathAndName("PaladinRetributionSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
        }
        catch (Exception e)
        {
            Logging.WriteError("PaladinRetributionSettings > Save(): " + e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            if (File.Exists(AdviserFilePathAndName("PaladinRetributionSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName)))
            {
                CurrentSetting =
                    Load<PaladinRetributionSettings>(AdviserFilePathAndName("PaladinRetributionSettings", ObjectManager.Me.Name + "." + Usefuls.RealmName));
                return true;
            }
            CurrentSetting = new PaladinRetributionSettings();
        }
        catch (Exception e)
        {
            Logging.WriteError("PaladinRetributionSettings > Load(): " + e);
        }
        return false;
    }
}

