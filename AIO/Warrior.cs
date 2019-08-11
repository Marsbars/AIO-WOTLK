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

public static class Warrior
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
    
       

    public static void Initialize()
    {

        Warriorsettings.Load();
        {
            _isLaunched = true;

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
        Warriorsettings.Load();
        Warriorsettings.CurrentSetting.ToForm();
        Warriorsettings.CurrentSetting.Save();
    }

    internal static void Rotation()
    {
        while (_isLaunched)
        {
            try
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
                    if (Warriorsettings.CurrentSetting.Framelock)
                    {
                        Framelock();
                    }
                    CombatRotation();
                    if (Warriorsettings.CurrentSetting.Framelock)
                    {
                        Frameunlock();
                    }
                }

            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(Usefuls.Latency);
        }

    }
    private static void CombatRotation()
    {
        Extension.FightSpell(Rend);
        if(MyTarget.GetDistance >7)
        {
            Extension.FightSpell(Charge);
        }
        Extension.FightSpell(HeroicStrike);
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
