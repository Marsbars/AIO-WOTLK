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
using System.ComponentModel;
using wManager.Wow;
using wManager;

public static class DeathKnight
{
    private static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
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
    public static Spell MarkofBlood = new Spell("Mark of Blood");
    public static Spell DancingRuneWeapon = new Spell("Dancing Rune Weapon");
    public static Timer Pestilencetimer = new Timer();




    public static void Initialize()
    {

        DKSettings.Load();
        {
            _isLaunched = true;
            if(Extension.GetSpecialization() == 1)
            {
                //Blood
            }
            if (Extension.GetSpecialization() == 2)
            {
                //Frost
            }
            if (Extension.GetSpecialization() == 3)
            {
                //Unholy
            }
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
        if(ObjectManager.Target.IsElite)
        {
            Extension.FightSpell(MarkofBlood);
        }
        if (ObjectManager.Target.IsElite)
        {
            Extension.FightSpell(DancingRuneWeapon);
        }
        if (DKSettings.CurrentSetting.DeathGrip && ObjectManager.Target.GetDistance > 10)
        {
            Extension.FightSpell(DeathGrip);
        }
        Extension.FightSpell(RuneStrike);
        if (!ffcheck && Extension.CanBleed(MyTarget))
        {
            Extension.FightSpell(IcyTouch);
        }
        if (!bpcheck)
        {
            Extension.FightSpell(PlagueStrike);
        }
        if (ffcheck && bpcheck
            && Extension.GetAttackingUnits(10).Count() > 1
            && Pestilencetimer.IsReady
            && Pestilence.IsSpellUsable
            && Pestilence.KnownSpell)
        {
            Pestilence.Launch();
            Pestilencetimer = new Timer(7000);
        }
        UseBloodSkill();
        Extension.FightSpell(DeathStrike);
    }

    public static void UseBloodSkill()
    {
        if (Extension.GetAttackingUnits(5).Count() == DKSettings.CurrentSetting.bloodstrike)
        {
            Extension.FightSpell(BloodStrike);
            return;
        }
        if (Extension.GetAttackingUnits(5).Count() == DKSettings.CurrentSetting.hearthstrike)
        {
            Extension.FightSpell(HeartSTrike);
            return;
        }
        if (Extension.GetAttackingUnits(5).Count() > DKSettings.CurrentSetting.bloodboil)
        {
            Extension.FightSpell(BloodBoil);
            return;
        }
        if (Extension.GetAttackingUnits(10).Count() > DKSettings.CurrentSetting.dnd)
        {
            ClickOnTerrain.Spell(DeathAndDecay.Id, ObjectManager.Me.Position);
        }
    }


    private static void BuffRotation()
    {
        List<WoWUnit> attackers = ObjectManager.GetUnitAttackPlayer();
        if (ObjectManager.Me.HealthPercent < 30 && attackers.Count > 1)
        {
            Extension.BuffSpell(IceBoundFortitude);
        }
        if(DKSettings.CurrentSetting.BloodPresence)
        {
            Extension.BuffSpell(BloodPresence);
        }
        Extension.BuffSpell(HornofWinter);

    }

    #region Pull
    private static void Pull()
    {
        if (ObjectManager.Target.IsAttackable
            && ObjectManager.Me.Target > 0)
        {
            Extension.FightSpell(DeathCoil);
            if (DKSettings.CurrentSetting.DeathGrip && ObjectManager.Target.GetDistance > 10)
            {
                Extension.FightSpell(DeathGrip);
            }
        }
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
