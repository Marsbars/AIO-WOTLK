﻿using System;
using System.Threading;
using robotManager.MemoryClass;
using robotManager.Helpful;
using wManager;
using wManager.Wow;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using Timer = robotManager.Helpful.Timer;
using System.Collections.Generic;
using System.Linq;
using wManager.Events;


public static class WarlockLevel
{


    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    private static bool _icanusewand = Extension.HaveRangedWeaponEquipped();

    public static float Range { get { return 25f; } }
    private static bool _isLaunched;
    public static Spell demonskin = new Spell("Demon Skin");
    public static Spell demonarmor = new Spell("Demon Armor");
    public static Spell felarmor = new Spell("Fel Armor");
    public static Spell unendingbreath = new Spell("Unending Breath");
    public static Spell shadowbolt = new Spell("Shadow Bolt");
    public static Spell immolate = new Spell("Immolate");
    public static Spell corruption = new Spell("Corruption");
    public static Spell summonimp = new Spell("Summon Imp");
    public static Spell summonvoid = new Spell("Summon Voidwalker");
    public static Spell summonfelguard = new Spell("Summon Felguard");
    public static Spell healthfunnel = new Spell("Health Funnel");
    public static Spell lifetap = new Spell("Life Tap");
    public static Spell createhealthstone = new Spell("Create Healthstone");
    public static Spell curseofagony = new Spell("Curse of Agony");
    public static Spell deathcoil = new Spell("Death Coil");
    public static Spell drainlife = new Spell("Drain Life");
    public static Spell fear = new Spell("Fear");
    public static Spell drainsoul = new Spell("Drain Soul");
    public static Spell haunt = new Spell("Haunt");
    public static Spell unstableaffliction = new Spell("Unstable Affliction");
    public static Spell shadowtrance = new Spell("Shadow Trance");
    public static Spell soullink = new Spell("Soul Link");
    private static Spell UseWand = new Spell("Shoot");
    public static Timer PetPullTimer = new Timer();
    private static int SaveDrink = wManager.wManagerSetting.CurrentSetting.DrinkPercent;

    public static void Initialize()
    {
        if (ObjectManager.Me.WowClass == WoWClass.Warlock && ObjectManager.Me.Level <= 80)
        {
            #region Loggin Settings
            Logging.Write("Warlock Low Level  Class...loading...");
            #endregion

            Logging.Write("Movement Lua enabled");
            WarlockLevelSettings.Load();
            Main.kindofclass = WarlockLevelSettings.CurrentSetting.ChooseTalent;
            Talents.InitTalents(WarlockLevelSettings.CurrentSetting.AssignTalents,
                                WarlockLevelSettings.CurrentSetting.UseDefaultTalents,
                                WarlockLevelSettings.CurrentSetting.TalentCodes.ToArray());
            Logging.Write("WarlockLevelSettings Loaded");
            TargetSwitcher();
            Logging.Write("Targetswitcher Activated");
            _isLaunched = true;
            Rotation();
        }
        else
        {
            Logging.Write("No  Warlock....unloading...");
        }
    }

    public static void Dispose()
    {
        _isLaunched = false;
        wManager.wManagerSetting.CurrentSetting.DrinkPercent = SaveDrink;
    }


    internal static void Rotation()
    {
        while (_isLaunched)
        {
            try
            {
                Main.settingRange = 29f;
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Fight.InFight)
                {
                    if (WarlockLevelSettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                    BuffRotation();
                    Pethandler();
                    Pull();
                    Soulshard();
                    Healthstone();
                }
                else
                {
                    if (WarlockLevelSettings.CurrentSetting.Framelock)
                    {
                        Extension.Framelock();
                    }
                    CombatRotation();
                    if (WarlockLevelSettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                    UseHealthstone();
                }

            }
            catch (Exception e)
            {
                Logging.WriteError("error" + e);
            }

            Thread.Sleep(WarlockLevelSettings.CurrentSetting.Delay);
        }
        Logging.Write("STOPPED");
    }

    public static void ShowConfiguration() // When a configuration is declared
    {
        WarlockLevelSettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(WarlockLevelSettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        WarlockLevelSettings.CurrentSetting.Save();
        Main.kindofclass = WarlockLevelSettings.CurrentSetting.ChooseTalent;
    }


    private static void CombatRotation()
    {

        if (WarlockLevelSettings.CurrentSetting.Fear)
        {
            //Fear Management
            if (Extension.GetAttackingUnits(5).Count() > 1)
            {
                Extension.Frameunlock();
                Logging.Write("2 Attackers, one will get Feared");
                WoWUnit mainTarget = Extension.GetAttackingUnits(10).Where(u => u.HealthPercent == Extension.GetAttackingUnits(10).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit fearTarget = Extension.GetAttackingUnits(10).Where(u => u.HealthPercent == Extension.GetAttackingUnits(10).Max(x => x.HealthPercent)).FirstOrDefault();
                if (fearTarget != mainTarget && !fearTarget.HaveBuff("Fear"))
                {
                    ObjectManager.Me.FocusGuid = fearTarget.Guid;
                    Extension.FightSpell(fear, true);
                    Logging.Write("Cast Fear on " + fearTarget);
                    Thread.Sleep(1000);

                }
            }
        }
        if (_icanusewand && Me.ManaPercentage < 5)
        {
            Extension.FightSpell(UseWand);
        }
        if (_icanusewand
            && WarlockLevelSettings.CurrentSetting.UseWand
            && MyTarget.HealthPercent < WarlockLevelSettings.CurrentSetting.UseWandTresh)
        {
            Extension.FightSpell(UseWand);
            return;
        }
        if (Me.HealthPercent > 50
            && Me.ManaPercentage < WarlockLevelSettings.CurrentSetting.Lifetap)
        {
            Extension.BuffSpell(lifetap);
        }
        if (Me.HaveBuff("Shadow Trance"))
        {
            Extension.FightSpell(shadowbolt);
        }
        if (Me.HealthPercent < WarlockLevelSettings.CurrentSetting.Drainlife)
        {
            Extension.FightSpell(drainlife);
        }
        if (ObjectManager.Pet.HealthPercent < 30
            && ObjectManager.Pet.IsAlive
            && Me.HealthPercent > 50)
        {
            Extension.FightSpell(healthfunnel);
        }
        Extension.FightSpell(corruption);
        Extension.FightSpell(curseofagony);
        if (WarlockLevelSettings.CurrentSetting.unstableaffl)
        {
            Extension.FightSpell(unstableaffliction);
        }
        if (!curseofagony.KnownSpell)
        {
            Extension.FightSpell(immolate);
        }
        if (MyTarget.HealthPercent <= 25)
        {
            Extension.FightSpell(drainsoul);
        }
        if (MyTarget.HealthPercent > 15)
        {
            Extension.FightSpell(shadowbolt);
        }

        Extension.Frameunlock();
    }


    private static void BuffRotation()
    {
        if (!demonarmor.KnownSpell && !felarmor.KnownSpell)
        {
            Extension.BuffSpell(demonskin, false);
        }
        if (!felarmor.KnownSpell)
        {
            Extension.BuffSpell(demonarmor, false);
        }
        Extension.BuffSpell(felarmor, false);
        Extension.BuffSpell(unendingbreath, false);
        Extension.BuffSpell(soullink);
    }

    #region Pethandler
    private static void Pethandler()
    {
        if (!ObjectManager.Pet.IsValid && !Me.IsMounted)
        {
            if (Me.ManaPercentage < 80 && !Me.HaveBuff("Drink"))
            {
                wManager.wManagerSetting.CurrentSetting.DrinkPercent = 95;
                Thread.Sleep(1000);
            }
            wManager.wManagerSetting.CurrentSetting.DrinkPercent = SaveDrink;
            Thread.Sleep(100); //workaround for recast after dismount

            if (!summonvoid.KnownSpell || !summonvoid.IsSpellUsable || !summonfelguard.IsSpellUsable || !summonfelguard.KnownSpell)
            {
                Extension.BuffSpell(summonimp);
            }
            if (WarlockLevelSettings.CurrentSetting.Pet == "VoidWalker")
            {
                Extension.BuffSpell(summonvoid);
            }
            if (WarlockLevelSettings.CurrentSetting.Pet == "Felguard")
            {
                Extension.BuffSpell(summonfelguard);
            }
            if (ObjectManager.Pet.HealthPercent < 30
                && ObjectManager.Pet.IsAlive
                && Me.HealthPercent > 50)
            {
                Extension.BuffSpell(healthfunnel);
            }
        }
    }
    #endregion


    #region Pull
    private static void Pull()
    {
        if (MyTarget.IsAttackable && Me.Target > 0 && ObjectManager.Pet.IsValid && PetPullTimer.IsReady)
        {
            Extension.Frameunlock();
            Lua.LuaDoString("PetAttack();");
            PetPullTimer = new Timer(500);
        }
    }

    #endregion

    private static void Healthstone()
    {
        if (!Consumables.HaveHealthstone())
        {
            Extension.Frameunlock();
            Extension.BuffSpell(createhealthstone);
        }
    }

    private static void UseHealthstone()
    {
        if (Me.HealthPercent < 20)
        {
            Extension.Frameunlock();
            Consumables.UseHealthstone();
        }
    }

    #region Multitarget	
    public static void TargetSwitcher()
    {
        FightEvents.OnFightLoop += (unit, cancelable) => {
            var unitsAffectingMyCombat = ObjectManager.GetUnitAttackPlayer();
            if (unitsAffectingMyCombat.Count <= 0)
                return;
            var unitsAttackMe = unitsAffectingMyCombat.Where(u => u != null && u.IsValid && u.IsTargetingMe).ToList();
            if (unitsAttackMe.Count > 1)
            {
                Extension.Frameunlock();
                var unitToAttack = unitsAttackMe.FirstOrDefault(u => u != null && u.IsValid && !u.IsMyPetTarget);
                if (unitToAttack != null && unitToAttack.IsValid && unitToAttack.IsAlive)
                {
                    if (!unitToAttack.IsMyTarget)
                        Interact.InteractGameObject(unitToAttack.GetBaseAddress, !ObjectManager.Me.GetMove);
                    if (unitToAttack.IsMyTarget)
                        Lua.LuaDoString("PetAttack();");
                    Logging.Write("PET ATTACKING: " + unitToAttack);
                }
            }

            else
            {
                var unitsAttackPet = unitsAffectingMyCombat.Where(u => u != null && u.IsValid && u.IsTargetingMyPet).ToList();
                var lowerHpUnit = unitsAttackPet.OrderBy(b => b.HealthPercent).FirstOrDefault();
                if (lowerHpUnit != null && lowerHpUnit.IsValid && lowerHpUnit.IsAlive && !lowerHpUnit.IsMyPetTarget)
                {
                    if (!lowerHpUnit.IsMyTarget)
                        Interact.InteractGameObject(lowerHpUnit.GetBaseAddress, !ObjectManager.Me.GetMove);
                    if (lowerHpUnit.IsMyTarget)
                        Lua.LuaDoString("PetAttack();");
                    //Fight.StartFight();
                    Logging.Write("PET ATTACKING LOWER HP: " + lowerHpUnit);
                }
            }

        };
    }
    #endregion
    #region Shardmanagement
    private static void Soulshard()
    {
        if (ItemsManager.GetItemCountByIdLUA(6265) >= 5)
        {
            Extension.DeleteItems("Soul Shard", 5);
        }
    }
    #endregion
}