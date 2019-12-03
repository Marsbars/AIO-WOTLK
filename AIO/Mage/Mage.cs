using System;
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


public static class Mage
{

    private static MageFoodManager _foodManager = new MageFoodManager();
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static bool _isLaunched;
    //Damage SPells
    public static Spell IceLance = new Spell("Ice Lance");
    public static Spell ConeOfCold = new Spell("Cone of Cold");
    public static Spell UseWand = new Spell("Shoot");
    public static Spell IcyVeins = new Spell("Icy Veins");
    public static Spell Fireball = new Spell("Fireball");
    public static Spell Frostbolt = new Spell("Frostbolt");
    public static Spell FireBlast = new Spell("Fire Blast");
    public static Spell DeepFreeze = new Spell("Deep Freeze");
    public static Spell FrostFirebolt = new Spell("Frostfire Bolt");
    //Buffs
    public static Spell ArcaneIntellect = new Spell("Arcane Intellect");
    public static Spell Evocation = new Spell("Evocation");
    public static Spell FrostArmor = new Spell("Frost Armor");
    public static Spell IceBarrier = new Spell("Ice Barrier");
    public static Spell ColdSnap = new Spell("Cold Snap");
    public static Spell IceArmor = new Spell("Ice Armor");
    //CC
    public static Spell Polymorph = new Spell("Polymorph");
    public static Spell FrostNova = new Spell("Frost Nova");
    //Pet
    public static Spell SummonWaterElemental = new Spell("Summon Water Elemental");
    //Usefuls
    public static Spell RemoveCurse = new Spell("Remove Curse");
    public static Spell CounterSpell = new Spell("Counterspell");
    public static Spell MirrorImage = new Spell("Mirror Image");
    public static Spell IceBlock = new Spell("Ice Block");
    public static Timer PetPullTimer = new Timer();

    public static void Initialize()
    {
        if (ObjectManager.Me.WowClass == WoWClass.Mage && ObjectManager.Me.Level <= 80)
        {
            #region Loggin Settings
            Logging.Write("Mage Low Level  Class...loading...");
            #endregion
            Magesettings.Load();
            Logging.Write("Magesettings Loaded");
            RangeManager();
            //TargetSwitcher();
            Logging.Write("Targetswitcher Activated");
            _isLaunched = true;
            Rotation();
        }
        else
        {
            Logging.Write("No  Mage....unloading...");
        }
    }

    public static void Dispose()
    {
        _isLaunched = false;
    }


    internal static void Rotation()
    {
        while (_isLaunched)
        {
            try
            {
                Main.settingRange = 25f;
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Fight.InFight)
                {
                    if (Magesettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                    BuffRotation();
                    _foodManager.CheckIfEnoughFoodAndDrinks();
                    _foodManager.CheckIfThrowFoodAndDrinks();
                    _foodManager.CheckIfHaveManaStone();
                    Pull();
                }
                else
                {
                    if (Magesettings.CurrentSetting.Framelock)
                    {
                        Extension.Framelock();
                    }
                    CombatRotation();
                    if (Magesettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                }

            }
            catch (Exception e)
            {
                Logging.WriteError("error" + e);
            }

            Thread.Sleep(Magesettings.CurrentSetting.Delay);
        }
        Logging.Write("STOPPED");
        wManagerSetting.CurrentSetting.UseLuaToMove = false;
        Logging.Write("Movement Lua disabled");
    }

    public static void ShowConfiguration() // When a configuration is declared
    {
        Magesettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(Magesettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        Magesettings.CurrentSetting.Save();
    }


    private static void CombatRotation()
    {
        if(Me.ManaPercentage < Magesettings.CurrentSetting.Manastone)
        {
            _foodManager.UseManaStone();
        }

        if (Magesettings.CurrentSetting.Sheep)
        {
            //Poly Management
            if (Extension.GetAttackingUnits(5).Count() > 1 && Polymorph.KnownSpell)
            {
                Extension.Frameunlock();
                Logging.Write("2 Attackers, one will get Feared");
                WoWUnit mainTarget = Extension.GetAttackingUnits(5).Where(u => u.HealthPercent == Extension.GetAttackingUnits(5).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit polyTarget = Extension.GetAttackingUnits(5).Where(u => u.HealthPercent == Extension.GetAttackingUnits(5).Max(x => x.HealthPercent)).FirstOrDefault();
                if (polyTarget != mainTarget && !polyTarget.HaveBuff("Polymorph"))
                {
                    ObjectManager.Me.FocusGuid = polyTarget.Guid;
                    Extension.FightSpell(Polymorph, true);
                    Logging.Write("Cast Poly on " + polyTarget);
                    Thread.Sleep(1000);

                }
            }
        }
        if (Extension.InterruptableUnit(20f) != null && CounterSpell.KnownSpell && CounterSpell.IsSpellUsable)
        {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.FocusGuid = Extension.InterruptableUnit(20f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(20f).Guid);
            Extension.FightSpell(CounterSpell, true);
        }
        if(Me.HaveBuff("Fireball!"))
            {
            Extension.FightSpell(FrostFirebolt);
            }
        if(!IceBarrier.IsSpellUsable && !IceBlock.IsSpellUsable)
        {
            Extension.BuffSpell(ColdSnap);
        }
        if(Me.HealthPercent < 15 && Extension.GetAttackingUnits(20).Count() >=2)
        {
            Extension.BuffSpell(Evocation);
        }
        if(Me.HealthPercent < 14 && !IceBarrier.IsSpellUsable)
        {
            Extension.BuffSpell(IceBlock);
        }
        if (Me.HealthPercent < 95)
        {
            Extension.BuffSpell(IceBarrier);
        }
        if(Extension.GetAttackingUnits(25).Count() >= 3 || MyTarget.IsElite)
        {
            Extension.FightSpell(MirrorImage);
            Extension.FightSpell(SummonWaterElemental);
        }
        if (Extension.GetAttackingUnits(25).Count() >= 2 || MyTarget.IsElite)
        {
            Extension.BuffSpell(IcyVeins);
            Extension.FightSpell(SummonWaterElemental);
        }
        Extension.FightSpell(DeepFreeze);
        if(Me.HaveBuff("Fingers of Frost") || MyTarget.HaveBuff(FrostNova.Id))
        {
            Extension.FightSpell(IceLance);
        }
        if(!Frostbolt.KnownSpell)
        {
            Extension.FightSpell(Fireball, false, false, false, false);
        }
        Extension.FightSpell(Frostbolt, false,false,false,false);
        if(MyTarget.HealthPercent < 10)
        {
            Extension.FightSpell(FireBlast);
        }
        if(MyTarget.GetDistance <7)
        {
            Extension.FightSpell(FrostNova);
        }
        Extension.Frameunlock();
    }


    private static void BuffRotation()
    {
        Extension.BuffSpell(ArcaneIntellect);
        if(!IceArmor.KnownSpell)
        {
            Extension.BuffSpell(FrostArmor);
        }
        Extension.BuffSpell(IceArmor);    
        if(Me.HealthPercent < 37 || Me.ManaPercentage <37)
        {
            Extension.BuffSpell(Evocation);
        }
    }

    #region Pethandler
    private static void Pethandler()
    {
        if (!ObjectManager.Pet.IsAlive && SummonWaterElemental.KnownSpell)
        {
            Thread.Sleep(500); //workaround for recast after dismount
            Extension.BuffSpell(SummonWaterElemental);
        }
    }
    #endregion


    #region Pull
    private static void Pull()
    {
        if (MyTarget.IsAttackable && Me.Target > 0 && ObjectManager.Pet.IsValid && PetPullTimer.IsReady)
        {
            Extension.Frameunlock();
            Lua.LuaDoString("PetAttack();", false);
            PetPullTimer = new Timer(500);
        }
    }

    #endregion


    #region Multitarget	
    public static void TargetSwitcher()
    {
        FightEvents.OnFightLoop += (unit, cancelable) =>
        {
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
                        Lua.LuaDoString("PetAttack();", false);
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
                        Lua.LuaDoString("PetAttack();", false);
                    //Fight.StartFight();
                    Logging.Write("PET ATTACKING LOWER HP: " + lowerHpUnit);
                }
            }

        };
    }
    #endregion

    private static void RangeManager()
    {
        wManager.Events.FightEvents.OnFightLoop += (unit, cancelable) => {
            if (MyTarget.GetDistance < 6 && MyTarget.HaveBuff(FrostNova.Id))
                {

                    var xvector = (ObjectManager.Me.Position.X) - (ObjectManager.Target.Position.X);
                    var yvector = (ObjectManager.Me.Position.Y) - (ObjectManager.Target.Position.Y);

                    Vector3 newpos = new Vector3()
                    {
                        X = ObjectManager.Me.Position.X + (float)((xvector * (20 / ObjectManager.Target.GetDistance) - xvector)),
                        Y = ObjectManager.Me.Position.Y + (float)((yvector * (20 / ObjectManager.Target.GetDistance) - yvector)),
                        Z = ObjectManager.Me.Position.Z
                    };
                    MovementManager.Go(PathFinder.FindPath(newpos), false);
                    Thread.Sleep(1500);
                }
        };
    }
}