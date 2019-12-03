using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager.Events;
using System.Collections.Generic;
using System.Linq;

public static class Paladin
{
    private static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }

    //Damage Spells
    public static Spell JudgementofLight = new Spell("Judgement of Light");
    public static Spell JudgementofWisdom = new Spell("Judgement of Wisdom");
    public static Spell HandofReckoning = new Spell("Hand of Reckoning");
    public static Spell AvengersShield = new Spell("Avenger's Shield");
    public static Spell HammerofWrath = new Spell("Hammer of Wrath");
    public static Spell Exorcism = new Spell("Exorcism");
    public static Spell Consecration = new Spell("Consecration");
    public static Spell DivineStorm = new Spell("Divine Storm");
    public static Spell CrusaderStrike = new Spell("Crusader Strike");
    public static Spell ShieldofRighteousness = new Spell("Shield of Righteousness");
    public static Spell HolyShield = new Spell("Holy Shield");
    public static Spell HolyWrath = new Spell("Holy Wrath");
    //Buff Blessings
    public static Spell BlessingofMight = new Spell("Blessing of Might");
    public static Spell BlessingofKings = new Spell("Blessing of Kings");
    public static Spell BlessingofWisdom = new Spell("Blessing of WisdHaom");
    public static Spell BlessingofSanctuary = new Spell("Blessing of Sanctuary");
    //Buff Aura
    public static Spell DevotionAura = new Spell("Devotion Aura");
    public static Spell CrusaderAura = new Spell("Crusader Aura");
    public static Spell RetributionAura = new Spell("Retribution Aura");
    public static Spell ConcentrationAura = new Spell("Concentration Aura");
    //Buff Seals	
    public static Spell SealofRighteousness = new Spell("Seal of Righteousness");
    public static Spell SealofCommand = new Spell("Seal of Command");
    public static Spell SealofWisdom = new Spell("Seal of Wisdom");
    public static Spell SealofCorruption = new Spell("Seal of Corruption");
    //Buff General
    public static Spell DivinePlea = new Spell("Divine Plea");
    public static Spell AvengingWrath = new Spell("Avenging Wrath");
    public static Spell DivineProtection = new Spell("Divine Protection");
    public static Spell SacredShield = new Spell("Sacred Shield");
    public static Spell RighteousFury = new Spell("Righteous Fury");
    public static Spell HandofFreedom = new Spell("Hand of Freedom");
    public static Spell GreaterBlessingOfMight = new Spell("Greater Blessing of Might");
    //Healing Spells
    public static Spell HolyLight = new Spell("Holy Light");
    public static Spell FlashofLight = new Spell("Flash of Light");
    public static Spell LayonHands = new Spell("Lay on Hands");
    public static Spell HandofProtection = new Spell("Hand of Protection");
    //Buff Rest
    public static Spell BloodCorruption = new Spell("Blood Corruption");
    public static Spell TheartofWar = new Spell("The Art of War");
    //stun
    public static Spell HammerofJustice = new Spell("Hammer of Justice");
    // dispell
    public static Spell Purify = new Spell("Purify");

    public static void Initialize()
    {
        {
            //Radar3D.Pulse();
            _isLaunched = true;
            Paladinsettings.Load();
            Logging.Write("Settings Loaded");
            Rotation();
        }
    }

    public static void Dispose() // When product stopped
    {
        {
            //Radar3D.Stop();
            _isLaunched = false;
        }
    }

    public static void ShowConfiguration() // When use click on Fight class settings
    {
        Paladinsettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(Paladinsettings.CurrentSetting, Me.WowClass.ToString());
        settingWindow.ShowDialog();
        Paladinsettings.CurrentSetting.Save();
    }

    public static void Rotation()
    {
        Logging.Write("Rotation Loaded");
        while (_isLaunched)
        {
            try
            {
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
                {
                    Main.settingRange = 5f;
                    if (!(Fight.InFight))
                    {
                        Healing();
                        BuffRotation();
                    }
                    else
                     if (Fight.InFight)
                    {
                        //if (Paladinsettings.CurrentSetting.Draw) //new
                        //{
                        //    foreach (W﻿oWUnit Mob in ObjectManager﻿.GetObjectWoWPlayer().Where(x => x.IsAlliance && ObjectManager.Target.TargetObject != null))
                        //    {
                        //        Radar3D.DrawCircle(ObjectManager.Target.Position, 1f, System.Drawing.Color.Red, true);
                        //        Radar3D.DrawLine(Me.Position, Mob.TargetObject.Position, System.Drawing.Color.Red);
                        //        Radar3D.DrawCircle(Mob.TargetObject.Position, 0.5f, System.Drawing.Color.LightBlue, false);
                        //    }
                        //}
                        Healing();
                        BuffRotation();
                        if (Paladinsettings.CurrentSetting.Framelock)
                        {
                            Extension.Framelock();
                        }
                        CombatRotation();
                        if (Paladinsettings.CurrentSetting.Framelock)
                        {
                            Extension.Frameunlock();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(Paladinsettings.CurrentSetting.Delay);
        }
    }

    public static void CombatRotation()
    {

        if (Extension.GetAttackingUnits(5).Count() > 1 && !Me.IsStunned && Paladinsettings.CurrentSetting.HammerofJustice)
        {
            WoWUnit mainTarget = Extension.GetAttackingUnits(5).Where(u => u.HealthPercent == Extension.GetAttackingUnits(5).Min(x => x.HealthPercent)).FirstOrDefault();
            WoWUnit hammerTarget = Extension.GetAttackingUnits(5).Where(u => u.HealthPercent == Extension.GetAttackingUnits(5).Max(x => x.HealthPercent)).FirstOrDefault();
            if (hammerTarget != mainTarget)
            {
                ObjectManager.Me.FocusGuid = hammerTarget.Guid;
                Extension.FightSpell(HammerofJustice, true, true);
                Logging.Write("Cast Hammer on " + hammerTarget);
                Thread.Sleep(500);
            }
        }
        if (MyTarget.GetDistance <= 25 && MyTarget.GetDistance >= 7 && Paladinsettings.CurrentSetting.HOR)
        {
            Extension.FightSpell(HandofReckoning, false);
        }
        if(Extension.GetAttackingUnits(20).Count() >= 3)
        {
        Extension.BuffSpell(AvengingWrath);
        }
        Extension.FightSpell(HammerofWrath, false);
        if (!JudgementofWisdom.KnownSpell)
        {
            Extension.FightSpell(JudgementofLight, false);
        }
        Extension.FightSpell(JudgementofWisdom, false);        
        Extension.FightSpell(CrusaderStrike, false);
        Extension.FightSpell(DivineStorm, false);
        if (Me.Level < 43 && MyTarget.HealthPercent > 25) //new
        {
            Extension.FightSpell(Consecration, false);
        }
        if (Me.Level > 42 && Extension.GetAttackingUnits(20).Count() > 1) //new
        {
            Extension.FightSpell(Consecration, false);
        }
        if (Me.HaveBuff(TheartofWar.Id))
        {
            Extension.FightSpell(Exorcism, false);
        }
        if (Me.Level < 43 && MyTarget.HealthPercent > 20)
        {
            Extension.FightSpell(Exorcism, false);
        }
        Extension.FightSpell(HolyWrath, false);
    }

    private static void BuffRotation()
    {
        if (Paladinsettings.CurrentSetting.Buffing)
        {
            if (Me.IsMounted && Paladinsettings.CurrentSetting.Crusader)
            {
                Extension.BuffSpell(CrusaderAura, true);
            }
            if (!Me.HaveBuff(GreaterBlessingOfMight.Id))
            {
                Extension.BuffSpell(BlessingofMight, false);
            }
            if (Paladinsettings.CurrentSetting.Aura == "Retribution Aura")
            {
                Extension.BuffSpell(RetributionAura, false);
            }
            if (Paladinsettings.CurrentSetting.Aura == "Devotion Aura")
            {
                Extension.BuffSpell(DevotionAura, false);
            }
            if (Extension.GetAttackingUnits(20).Count() >= 3 && Paladinsettings.CurrentSetting.SOC)
            {
                Extension.BuffSpell(SealofCommand, false);
            }
            if (Extension.GetAttackingUnits(20).Count() <= 2)
            {
                Extension.BuffSpell(SealofRighteousness, false);
            }
            if (Me.ManaPercentage < 80 && !Fight.InFight)
            {
                Extension.BuffSpell(DivinePlea, false);
            }
            if (Extension.GetAttackingUnits(20).Count() >2 && Paladinsettings.CurrentSetting.SShield)
            {
                Extension.BuffSpell(SacredShield, false);
                return;
            }
            if(Me.HealthPercent < 15 && Paladinsettings.CurrentSetting.LayOnHands && !Me.HaveBuff("Forbearance"))
            {

            }
            if (Me.HealthPercent < 20 && Paladinsettings.CurrentSetting.HoProtection && !Me.HaveBuff("Forbearance"))
            {
                Extension.BuffSpell(HandofProtection, false);
                return;
            }
            if (Me.HealthPercent < 40 && !Me.HaveBuff("Forbearance") && Extension.GetAttackingUnits(5).Count() >= 2 && Paladinsettings.CurrentSetting.DivProtection)
            {
                Extension.BuffSpell(DivineProtection, false);
                return;
            }
            if (Me.Rooted)
            {
                Extension.BuffSpell(HandofFreedom);
            }
        }

    }

    private static void Healing()
    {
        bool Poison = Extension.HasPoisonDebuff();
        bool Disease = Extension.HasDiseaseDebuff();
        if (Poison && Paladinsettings.CurrentSetting.Purify)
        {
            Extension.BuffSpell(Purify);
        }
        if (Disease && Paladinsettings.CurrentSetting.Purify)
        {
            Extension.BuffSpell(Purify);
        }
        if (Me.HaveBuff(TheartofWar.Id) && Me.HealthPercent <= 75)
        {
            Extension.HealSpell(FlashofLight, false, true);
        }

        if (Me.HealthPercent <= Paladinsettings.CurrentSetting.HL)
        {
            Extension.HealSpell(HolyLight, false, true);
        }
        if (Me.HealthPercent <= Paladinsettings.CurrentSetting.FL)
        {
            Extension.HealSpell(FlashofLight, false, true);
        }

    }


}
