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

public static class DeathKnight
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
        DKSettings.Load();
        DKSettings.CurrentSetting.ToForm();
        DKSettings.CurrentSetting.Save();
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
                    if (DKSettings.CurrentSetting.Framelock)
                    {
                        Framelock();
                    }
                    CombatRotation();
                    if (DKSettings.CurrentSetting.Framelock)
                    {
                        Frameunlock();
                    }
                }

            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(DKSettings.CurrentSetting.Delay);
        }

    }
    private static void CombatRotation()
    {
        bool ffcheck = ObjectManager.Target.HaveBuff("Frost Fever");
        bool bpcheck = ObjectManager.Target.HaveBuff("Blood Plague");
        if (Extension.InterruptableUnit(5f) != null && MindFreeze.KnownSpell && MindFreeze.IsSpellUsable)
        {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.FocusGuid = Extension.InterruptableUnit(5f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(5f).Guid);
            Extension.FightSpell(MindFreeze, true);
        }
        if (Extension.InterruptableUnit(20f) != null && Strangulate.KnownSpell && Strangulate.IsSpellUsable)
        {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.FocusGuid = Extension.InterruptableUnit(20f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(20f).Guid);
            Extension.FightSpell(Strangulate, true);
        }
        if (DKSettings.CurrentSetting.DeathGrip && ObjectManager.Target.GetDistance > 10)
        {
            FightSpell(DeathGrip);
        }
        FightSpell(RuneStrike);
        if (!ffcheck)
        {
            FightSpell(IcyTouch);
        }
        if (!bpcheck)
        {
            FightSpell(PlagueStrike);
        }
        if (ffcheck && bpcheck
            && GetAttackingUnits(10).Count() > 1
            && Pestilencetimer.IsReady
            && Pestilence.IsSpellUsable
            && Pestilence.KnownSpell)
        {
            Pestilence.Launch();
            Pestilencetimer = new Timer(7000);
        }
        UseBloodSkill();
        FightSpell(DeathStrike);
    }

    public static void UseBloodSkill()
    {
        if (GetAttackingUnits(5).Count() == 1)
        {
            FightSpell(BloodStrike);
            return;
        }
        if (GetAttackingUnits(5).Count() == 2)
        {
            FightSpell(HeartSTrike);
            return;
        }
        if (GetAttackingUnits(5).Count() > 2)
        {
            FightSpell(BloodBoil);
            return;
        }
        if (GetAttackingUnits(10).Count() > 3)
        {
            ClickOnTerrain.Spell(DeathAndDecay.Id, ObjectManager.Me.Position);
        }
    }


    private static void BuffRotation()
    {
        List<WoWUnit> attackers = ObjectManager.GetUnitAttackPlayer();
        if (ObjectManager.Me.HealthPercent < 30 && attackers.Count > 1)
        {
            BuffSpell(IceBoundFortitude);
        }
        if(DKSettings.CurrentSetting.BloodPresence)
        {
        BuffSpell(BloodPresence);
        }
        BuffSpell(HornofWinter);

    }

    #region Pull
    private static void Pull()
    {
        if (ObjectManager.Target.IsAttackable
            && ObjectManager.Me.Target > 0)
        {
            FightSpell(DeathCoil);
            if (DKSettings.CurrentSetting.DeathGrip && ObjectManager.Target.GetDistance > 10)
            {
                FightSpell(DeathGrip);
            }
        }
    }

    #endregion

    #region Check Fightspell Casting
    private static bool FightSpell(Spell spell)
    {
        if (spell.KnownSpell && spell.IsSpellUsable
            && spell.IsDistanceGood
            && ObjectManager.Me.HasTarget
            && ObjectManager.Target.IsAttackable
            && !ObjectManager.Target.HaveBuff(spell.Name))
        {
            Frameunlock();
            spell.Launch();
            Usefuls.WaitIsCasting();
            return true;
        }
        return false;
    }
    #endregion

    #region Check Buffspell Casting
    private static bool BuffSpell(Spell spell, bool CanBeMounted = false)
    {
        if (spell.KnownSpell
            && spell.IsSpellUsable
            && !ObjectManager.Me.HaveBuff(spell.Name))
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

    #region attackers
    public static IEnumerable<WoWUnit> GetAttackingUnits(int range)
    {
        return ObjectManager.GetUnitAttackPlayer().Where(u => u.Position.DistanceTo(ObjectManager.Target.Position) <= range);
    }
    #endregion


}
