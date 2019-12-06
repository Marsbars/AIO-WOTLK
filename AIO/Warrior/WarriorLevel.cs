using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using robotManager.MemoryClass;
using wManager.Events;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using Timer = robotManager.Helpful.Timer;
using wManager.Wow;
using wManager;

public static class WarriorLevel
{
    public static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static Spell Attack = new Spell("Attack");
    public static Spell HeroicStrike = new Spell("Heroic Strike");
    public static Spell BattleShout = new Spell("Battle Shout");
    public static Spell Charge = new Spell("Charge");
    public static Spell Rend = new Spell("Rend");
    public static Spell Hamstring = new Spell("Hamstring");
    public static Spell BloodRage = new Spell("Bloodrage");
    public static Spell Overpower = new Spell("Overpower");
    public static Spell DemoralizingShout = new Spell("Demoralizing Shout");
    public static Spell Throw = new Spell("Throw");
    public static Spell Shoot = new Spell("Shoot");
    public static Spell Retaliation = new Spell("Retaliation");
    public static Spell Cleave = new Spell("Cleave");
    public static Spell Execute = new Spell("Execute");
    public static Spell SweepingStrikes = new Spell("Sweeping Strikes");
    public static Spell Bloodthirst = new Spell("Bloodthirst");
    public static Spell VictoryRush = new Spell("Victory Rush");
    public static Spell ThunderClap = new Spell("Thunder Clap");
    public static Spell Intercept = new Spell("Intercept");
    public static Spell Whirlwind = new Spell("Whirlwind");
    public static Spell Pummel = new Spell("Pummel");



    public static void Initialize()
    {

        WarriorLevelSettings.Load();
        Talents.InitTalents(WarriorLevelSettings.CurrentSetting.AssignTalents,
                            WarriorLevelSettings.CurrentSetting.UseDefaultTalents,
                            WarriorLevelSettings.CurrentSetting.TalentCodes.ToArray());
        {
            _isLaunched = true;
            Logging.Write("Warrior Low Level Class...loading...");
            Rotation();
        }



    }

    public static void Dispose()
    {

        {
            _isLaunched = false;
        }



    }

    public static void ShowConfiguration()
    {
        WarriorLevelSettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(WarriorLevelSettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        WarriorLevelSettings.CurrentSetting.Save();
    }

    internal static void Rotation()
    {
        while (_isLaunched)
        {
            try
            {
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
                {
                    Main.settingRange = 5f;
                    if (!Fight.InFight)
                    {
                        BuffRotation();
                        Pull();
                    }

                    else

                     if (Fight.InFight && ObjectManager.Me.HasTarget)
                    {
                        BuffRotation();
                        if (WarriorLevelSettings.CurrentSetting.Framelock)
                        {
                            Framelock();
                        }
                        CombatRotation();
                        if (WarriorLevelSettings.CurrentSetting.Framelock)
                        {
                            Frameunlock();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(WarriorLevelSettings.CurrentSetting.Delay);
        }

    }
    private static void CombatRotation()
    {

        if (Extension.InterruptableUnit(10f) != null && Pummel.KnownSpell && Pummel.IsSpellUsable)
        {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.FocusGuid = Extension.InterruptableUnit(5f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(5f).Guid);
            Extension.FightSpell(Pummel, true);
        }
        if (MyTarget.HealthPercent < 20)
        {
            Extension.FightSpell(Execute);
        }
        Extension.FightSpell(VictoryRush);
        Extension.FightSpell(Rend);
        if (MyTarget.GetDistance > 7)
        {
            Extension.FightSpell(Intercept);
            Extension.FightSpell(Charge);
        }
        if (Extension.GetAttackingUnits(5).Count() > 1)
        {
            Extension.FightSpell(ThunderClap);
            Extension.FightSpell(Cleave);
            Extension.FightSpell(Whirlwind);
            Extension.BuffSpell(Bloodthirst);
        }
        if (ObjectManager.Me.Rage >= 40)
        {
            Extension.FightSpell(HeroicStrike);
        }
    }


    private static void BuffRotation()
    {
        Extension.BuffSpell(BattleShout);

    }

    #region Pull
    private static void Pull()
    {

    }

    #endregion


    #region Check Interruptspell Casting
    private static bool InterruptSpell(Spell spell, bool CanBeMounted = false)
    {
        var resultLua = Lua.LuaDoString("ret = \"false\"; spell, rank, displayName, icon, startTime, endTime, isTradeSkill, ca﻿stID, interrupt = UnitCastingInfo(\"target\"); if interrupt then ret ﻿= \"true\" end", "ret");
        if (spell.KnownSpell
            && spell.IsSpellUsable
            && resultLua == "true")
        {
            if (ObjectManager.Me.IsMounted == CanBeMounted)
            {
                Frameunlock();
                spell.Launch();
                Usefuls.WaitIsCasting();
                return true;
            }
        }
        return false;
    }
    #endregion

    private static void Frameunlock()
    {
        if (Memory.WowMemory.FrameIsLocked && Hook.AllowFrameLock)
        {
            wManagerSetting.CurrentSetting.UseLuaToMove = false;
            Thread.Sleep(10);
            Memory.WowMemory.UnlockFrame(true);
            Thread.Sleep(10);
        }
    }

    private static void Framelock()
    {
        if (!Memory.WowMemory.FrameIsLocked && Hook.AllowFrameLock)
        {
            wManagerSetting.CurrentSetting.UseLuaToMove = true;
            Thread.Sleep(10);
            Memory.WowMemory.LockFrame();
            Thread.Sleep(10);
        }
    }

}
