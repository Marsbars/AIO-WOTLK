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
    private static bool _isLaunched;

    public static Spell IcyTouch = new Spell("Icy Touch");
    public static Spell ChainsOfIce = new Spell("Chains of Ice");
    public static Spell PlagueStrike = new Spell("Plague Strike");
    public static Spell BloodStrike = new Spell("Blood Strike");
    public static Spell HeartSTrike = new Spell("Heartstrike");
    public static Spell DeathCoil = new Spell("Death Coil");
    public static Spell DeathGrip = new Spell("Death Grip");
    public static Spell BloodBoil = new Spell("Blood Boil");
    public static Spell Pestilence = new Spell("Pestilence");
    public static Spell Strangulate = new Spell("Strangulate");
    public static Spell MindFreeze = new Spell("Mind Freeze");
    public static Spell DeathAndDecay = new Spell("Death and Decay");
    public static Spell DeathStrike = new Spell("Death Strike");
    public static Spell Obliterate = new Spell("Obliterate");
    public static Spell BloodPresence = new Spell("Blood Presence");
    public static Spell FrostPresence = new Spell("Frost Presence");
    public static Spell IceBoundFortitude = new Spell("Icebound Fortitude");
    public static Spell RuneStrike = new Spell("Rune Strike");
    public static Spell HornofWinter = new Spell("Horn of Winter");
    public static Timer Pestilencetimer = new Timer();




    public static void Initialize()
    {

        DKSettings.Load();
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

    }


    private static void BuffRotation()
    {


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
