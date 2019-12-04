using System;
using System.Threading;
using robotManager.Helpful;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using Timer = robotManager.Helpful.Timer;
using System.Collections.Generic;
using System.Linq;
using wManager.Events;


public static class HunterLevel
{
    private static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static Timer PetFeedTimer = new Timer();
    public static Timer PetHealTimer = new Timer();
    public static Timer PetPullTimer = new Timer();
    public static Timer MultiShotFeigndeath = new Timer();

    //range Combat Spells
    public static Spell HuntersMark = new Spell("Hunter's Mark"); //added
    public static Spell SerpentSting = new Spell("Serpent Sting"); //added
    public static Spell ArcaneShot = new Spell("Arcane Shot"); //added
    public static Spell ConcussiveShot = new Spell("Concussive Shot");
    public static Spell MultiShot = new Spell("Multi-Shot");
    public static Spell SteadyShot = new Spell("Steady Shot");
    public static Spell KillShot = new Spell("Kill Shot");

    //close  Combat Spells
    public static Spell RaptorStrike = new Spell("Raptor Strike"); //added

    //Buffs Spells
    public static Spell AspecoftheMonkey = new Spell("Aspect of the Monkey"); //CloseCombat
    public static Spell AspecoftheHawk = new Spell("Aspect of the Hawk"); //Dmg Buff
    public static Spell AspecoftheCheetah = new Spell("Aspect of the Cheetah"); //Speedboost
    public static Spell AspecoftheViper = new Spell("Aspect of the Viper"); //Manareg
    public static Spell AspecoftheDragonhawk = new Spell("Aspect of the Dragonhawk"); //Highlevel Aspect
    public static Spell RapidFire = new Spell("Rapid Fire"); //for Multimobs
    public static Spell KillCommand = new Spell("Kill Command");
    
    //Pet Management
    public static Spell RevivePet = new Spell("Revive Pet");
    public static Spell CallPet = new Spell("Call Pet");
    public static Spell MendPet = new Spell("Mend Pet");
    public static Spell Intimidation = new Spell("Intimidation");
    public static Spell BestialWrath = new Spell("Bestial Wrath");

    //custom Spells
    public static Spell Disengage = new Spell("Disengage");
    public static Spell FrostTrap = new Spell("Frost Trap");
    public static Spell FeignDeath = new Spell("Feign Death");
    
    public static void Initialize() // When product started, initialize and launch Fightclass
    {
        if (ObjectManager.Me.WowClass == WoWClass.Hunter)
        {
            HunterLevelSettings.Load();
            Logging.Write("Hunter Low Level  Class...loading...");
            RangeManager();
            _isLaunched = true;
            Rotation();
        }
        else
        {
            Logging.Write("No  Hunter....unloading...");
        }
    }

    public static void Dispose() // When product stopped
    {
        _isLaunched = false;
    }

    public static void ShowConfiguration() // When a configuration is declared
    {
        HunterLevelSettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(HunterLevelSettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        HunterLevelSettings.CurrentSetting.Save();
    }


    internal static void Rotation() // Rotation itself
    {
        while (_isLaunched)
        {
            try
            {
                Main.settingRange = 29f;

                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !(Fight.InFight))
                {
                    if (!Me.InCombatFlagOnly)
                        BuffRotation(); // Out of Combat buffing

                    PetRevivehandler(); //Handles Petrezz

                    if (HunterLevelSettings.CurrentSetting.Petfeed)
                        Feed(); //Sub for Petfeeding
                }
                else
                {
                    if (HunterLevelSettings.CurrentSetting.Framelock)
                        Extension.Framelock();

                    if (Fight.InFight && Me.Target > 0UL && ObjectManager.Target.IsAttackable)
                        CombatRotation();

                    if (HunterLevelSettings.CurrentSetting.Framelock)
                        Extension.Frameunlock();
                }
            }
            catch (Exception e)
            {
                Logging.WriteError("error" + e);
            }
            Thread.Sleep(HunterLevelSettings.CurrentSetting.Delay);
        }
        Logging.Write("STOPPED");
    }

    private static void CombatRotation()
    {
        // Pet attack
        if (Fight.InFight && Me.Target > 0UL && ObjectManager.Target.IsAttackable
            && !ObjectManager.Pet.HaveBuff("Feed Pet Effect") && ObjectManager.Pet.Target != Me.Target)
            Lua.LuaDoString("PetAttack();", false);

        //Pethandle in Fight
        if (ObjectManager.Pet.HealthPercent < HunterLevelSettings.CurrentSetting.PetmendInFight
            && PetHealTimer.IsReady)
        {
            Extension.PetSpell(MendPet);
            PetHealTimer = new Timer(1000 * 15);
            return;
        }

        if (Extension.InterruptableUnit(20f) != null && Intimidation.KnownSpell && Intimidation.IsSpellUsable)
        {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.Target = Extension.InterruptableUnit(20f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(20f).Guid);
            Extension.FightSpell(Intimidation);
        }

        if (ObjectManager.Target.IsElite || Extension.GetAttackingUnits(20).Count() > 1)
        {
            Extension.FightSpell(BestialWrath);
        }

        if (ObjectManager.Target.IsElite || Extension.GetAttackingUnits(20).Count() > 2)
        {
            Extension.BuffSpell(RapidFire);
        }

        //Ranged Attacks
        if (MyTarget.GetDistance >= 7)
        {
            Extension.FightSpell(HuntersMark);

            if (!Me.HaveBuff("Kill Command"))
                Extension.FightSpell(KillCommand);

            if (MyTarget.HealthPercent > 40)
            {
                Extension.FightSpell(SerpentSting);
            }
                Extension.FightSpell(ArcaneShot);
            
            if (MultiShotFeigndeath.IsReady && HunterLevelSettings.CurrentSetting.MultiS)
                Extension.FightSpell(MultiShot);

            Extension.FightSpell(SteadyShot, false, false, true);

            if (MyTarget.HealthPercent < 20)
                Extension.FightSpell(KillShot);

            if (Me.ManaPercentage < HunterLevelSettings.CurrentSetting.AspecofViper)
                Extension.BuffSpell(AspecoftheViper);

            if (Me.ManaPercentage > 30)
                Extension.BuffSpell(AspecoftheDragonhawk);

            if (!AspecoftheDragonhawk.KnownSpell && Me.ManaPercentage > 30)
                Extension.BuffSpell(AspecoftheHawk);
        }

        //Close  Combat  Attacks
        if (MyTarget.GetDistance <= 6)
        {
            Extension.FightSpell(RaptorStrike);

            if (!MyTarget.IsTargetingMe && ObjectManager.Pet.IsAlive && HunterLevelSettings.CurrentSetting.Dis)
                Extension.BuffSpell(Disengage);

            if (MyTarget.IsTargetingMe && ObjectManager.Pet.IsAlive)
            {
                Extension.BuffSpell(FeignDeath);
                Thread.Sleep(1500);
                MultiShotFeigndeath = new Timer(1000 * 5);
                return;
            }

            if (AspecoftheDragonhawk.KnownSpell)
                Extension.BuffSpell(AspecoftheDragonhawk);
        }

    }

    private static void Checkhostile()
    {
        if (ObjectManager.GetWoWUnitHostile().Any(x => x.Position.DistanceTo(ObjectManager.Me.Position) < 20) && !ObjectManager.Me.IsMounted)
        {
            MovementManager.StopMove();
            Fight.StartFight(ObjectManager.GetWoWUnitAttackables().OrderBy(x => x.Position.DistanceTo(ObjectManager.Me.Position)).FirstOrDefault().Guid);
        }
    }

    private static void RangeManager()
    {
        if (HunterLevelSettings.CurrentSetting.Backpaddle)
        { 
            wManager.Events.FightEvents.OnFightLoop += (unit, cancelable) =>
            {
                if (ObjectManager.Target.GetDistance < 7 && ObjectManager.Target.IsTargetingMyPet)
                {

                    var xvector = (ObjectManager.Me.Position.X) - (ObjectManager.Target.Position.X);
                    var yvector = (ObjectManager.Me.Position.Y) - (ObjectManager.Target.Position.Y);

                    Vector3 newpos = new Vector3()
                    {
                        X = ObjectManager.Me.Position.X + (float)((xvector * (10 / ObjectManager.Target.GetDistance) - xvector)),
                        Y = ObjectManager.Me.Position.Y + (float)((yvector * (10 / ObjectManager.Target.GetDistance) - yvector)),
                        Z = ObjectManager.Me.Position.Z
                    };
                    MovementManager.Go(PathFinder.FindPath(newpos), false);
                    Thread.Sleep(1500);
                }
            };
        }
    }

    #region Multitarget
    public static void Multitarget()
    {
        FightEvents.OnFightLoop += (unit, cancelable) => { // this code will loop everytime you are fighting
            var unitsAffectingMyCombat = ObjectManager.GetUnitAttackPlayer();
            var unitsAttackMe = unitsAffectingMyCombat.Where(u => u != null && u.IsValid && u.IsTargetingMe).ToList();

            if (unitsAttackMe.Count > 1 && ObjectManager.Pet.IsAlive && ObjectManager.Pet.IsValid)
            {
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

            //IF ALL THE MOBS ARE ATTACKING THE PET FOCUS THE LOWER HP ONE
            else
            {
                var unitsAttackPet = unitsAffectingMyCombat.Where(u => u != null && u.IsValid && u.IsTargetingMyPet).ToList();
                var lowerHpUnit = unitsAttackPet.OrderBy(uu => uu.HealthPercent).FirstOrDefault();
                if (lowerHpUnit != null && lowerHpUnit.IsValid && lowerHpUnit.IsAlive && !lowerHpUnit.IsMyPetTarget)
                {
                    if (!lowerHpUnit.IsMyTarget)
                        Interact.InteractGameObject(lowerHpUnit.GetBaseAddress, !ObjectManager.Me.GetMove);
                    if (lowerHpUnit.IsMyTarget)
                        Lua.LuaDoString("PetAttack();");
                    Logging.Write("PET ATTACKING LOWER HP: " + lowerHpUnit);
                }
            }
        };
    }
    #endregion

    private static void PetRevivehandler()
    {
        if (RevivePet.IsSpellUsable
            && RevivePet.KnownSpell
            && ObjectManager.Pet.IsDead
            && !ObjectManager.Me.IsMounted)
        {
            RevivePet.Launch();
            Usefuls.WaitIsCasting();
        }

        if (CallPet.IsSpellUsable
            && CallPet.KnownSpell
            && !ObjectManager.Pet.IsValid
            && !ObjectManager.Me.IsMounted)
        {
            CallPet.Launch();
            Usefuls.WaitIsCasting();
        }

    }

    private static void BuffRotation()
    {
        if (Me.ManaPercentage < HunterLevelSettings.CurrentSetting.AspecofViper)
            Extension.BuffSpell(AspecoftheViper);
        
        if (AspecoftheCheetah.KnownSpell && HunterLevelSettings.CurrentSetting.Cheetah)
            Extension.BuffSpell(AspecoftheCheetah);

        if (!AspecoftheDragonhawk.KnownSpell && !AspecoftheCheetah.HaveBuff
            && (Me.ManaPercentage > 90 || !AspecoftheViper.KnownSpell))
            Extension.BuffSpell(AspecoftheHawk);

        if (Me.ManaPercentage > 90 && !AspecoftheCheetah.HaveBuff)
            Extension.BuffSpell(AspecoftheDragonhawk);

        if (HunterLevelSettings.CurrentSetting.Checkpet)
        {
            if (ObjectManager.Pet.IsAlive && ObjectManager.Pet.IsValid 
                && ObjectManager.Pet.HealthPercent < HunterLevelSettings.CurrentSetting.PetHealth)
                if (PetHealTimer.IsReady)
                {
                    Extension.PetSpell(MendPet);
                    PetHealTimer = new Timer(1000 * 15);
                }
            Thread.Sleep(1000);
            return;
        }
    }

    //FoodPart for Pet
    private static void Feed()
    {
        if (ObjectManager.Pet.IsAlive && PetFeedTimer.IsReady && Lua.LuaDoString<int>("happiness, damagePercentage, loyaltyRate = GetPetHappiness() return happiness", "") < 3)
            FeedPet();
        PetFeedTimer = new Timer(1000 * 15);
        return;
    }

    private static string PetFoodType()
    {
        return (string)Lua.LuaDoString<string>("return GetPetFoodTypes();", "");
    }

    private static List<string> FoodList()
    {
        return new List<string>()
    {
      "Tough Jerky",
      "Haunch of Meat",
      "Mutton Chop",
      "Wild Hog Shank",
      "Cured Ham Steak",
      "Roasted Quail",
      "Smoked Talbuk Venison",
      "Clefthoof Ribs",
      "Salted Venison",
      "Mead Basted Caribou",
      "Mystery Meat",
      "R\xFEFFed Wolf Mea\xFEFF\xFEFFt"
    };
    }

    private static List<string> Fungus()
    {
        return new List<string>() { "Raw Black Truffle" };
    }

    private static List<string> FishList()
    {
        return new List<string>()
    {
      "Slitherskin Mackerel",
      "Longjaw Mud Snapper",
      "Bristle Whisker Catfish",
      "Rockscale Cod",
      "Striped Yellowtail",
      "Spinefin Halibut",
      "Sunspring Carp",
      "Zangar Trout",
      "Fillet of Icefin",
      "Poached Emperor Salmon"
    };
    }

    private static List<string> FruitList()
    {
        return new List<string>()
    {
      "Shiny Red Apple",
      "Tel'Abim Banana",
      "Snapvine Watermelon",
      "Goldenbark Apple",
      "Heaven Peach",
      "Moon Harvest Pumpkin",
      "Deep Fried Plantains",
      "Skethyl Berries",
      "Telaari Grapes",
      "Tundra Berries",
      "Savory Snowplum"
    };
    }

    private static List<string> BreadList()
    {
        return new List<string>()
    {
      "Tough Hunk of Bread",
      "Freshly Baked Bread",
      "Moist Cornbread",
      "Mulgore Spice Bread",
      "Soft Banana Bread",
      "Homemade Cherry Pie",
      "Mag'har Grainbread",
      "Crusty Flatbread"
    };
    }

    private static void FeedByType(List<string> list)
    {
        foreach (string str in list)
        {
            if (ItemsManager.GetItemCountByNameLUA(str) > 0)
            {
                Lua.LuaDoString("CastSpellByName('Feed Pet')", false);
                Lua.LuaDoString("UseItemByName('" + str + "')", false);
                Thread.Sleep(5000);
            }
        }
    }
    private static void FeedPet()
    {
        if (PetFoodType().Contains("Meat"))
            FeedByType(FoodList());
        if (PetFoodType().Contains("Fungus"))
            FeedByType(Fungus());
        if (PetFoodType().Contains("Fish"))
            FeedByType(FishList());
        if (PetFoodType().Contains("Fruit"))
            FeedByType(FruitList());
        if (!PetFoodType().Contains("Bread"))
            return;
        FeedByType(BreadList());
    }
}