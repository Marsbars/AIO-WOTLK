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


public static class Hunter
{
    private static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static Timer PetFeedTimer = new Timer();
    public static Timer PetHealTimer = new Timer();
    public static Timer PetPullTimer = new Timer();

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

    //custom Spells
    public static Spell Disengage = new Spell("Disengage");
    public static Spell FrostTrap = new Spell("Frost Trap");
    public static Spell FeignDeath = new Spell("Feign Death");


    public static void Initialize() // When product started, initialize and launch Fightclass
    {
        Multitarget();
        RangeManager();

        //HunterSettings.Load(); //to add  Settings later
        if (ObjectManager.Me.WowClass == WoWClass.Hunter)
        {
            Huntersettings.Load();
            Logging.Write("Hunter Low Level  Class...loading...");
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
        Huntersettings.Load();
        Huntersettings.CurrentSetting.ToForm();
        Huntersettings.CurrentSetting.Save();
    }


    internal static void Rotation() // Rotation itself
    {
        while (_isLaunched)
        {
            try
            {
                Main.settingRange = 25f;
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !(Fight.InFight))
                {
                    BuffRotation(); // Out of Combat buffing
                    PetRevivehandler(); //Handles Petrezz
                    Feed(); //Sub for Petfeeding
                    Pull();
                }
                else
                {
                    if (Huntersettings.CurrentSetting.Framelock)
                    {
                        Extension.Framelock();
                    }
                    CombatRotation(); //call of  Combatroutine
                    if (Huntersettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                }


            }
            catch (Exception e)
            {
                Logging.WriteError("error" + e);
            }

            Thread.Sleep(10); // little sleep for 10ms
        }
        Logging.Write("STOPPED");
    }

    /*
     * CreateStatusFrame()
     * InGame Status frame to see which spells casting next
     */



    private static void CombatRotation()
    {
        //Ranged Attacks
        if (MyTarget.GetDistance >= 7)
        {
            Extension.FightSpell(HuntersMark);
            if (MyTarget.IsTargetingMe)
            {
                Extension.FightSpell(ConcussiveShot);
            }
            if (!Me.HaveBuff("Kill Command"))
            {
                Extension.FightSpell(KillCommand);
            }
            Extension.FightSpell(SerpentSting);
            Extension.FightSpell(ArcaneShot);
            Extension.FightSpell(MultiShot);
            Extension.FightSpell(SteadyShot, false, false, true);

            if (MyTarget.HealthPercent < 20)
            {
                Extension.FightSpell(KillShot);
            }

            if (Me.ManaPercentage < 10)
            {
                Extension.BuffSpell(AspecoftheViper);
            }

            if (Me.ManaPercentage > 90)
            {
                Extension.BuffSpell(AspecoftheDragonhawk);
            }

            if (!AspecoftheDragonhawk.KnownSpell
                && Me.ManaPercentage > 90)
            {
                Extension.BuffSpell(AspecoftheHawk);
            }
        }

        //Close  Combat  Attacks
        if (MyTarget.GetDistance <= 6)
        {
            Extension.FightSpell(RaptorStrike);

            if (!MyTarget.IsTargetingMe
                && ObjectManager.Pet.IsAlive)
            {
                Extension.BuffSpell(Disengage);
            }
            if (MyTarget.IsTargetingMe
                && ObjectManager.Pet.IsAlive)
            {
                Extension.BuffSpell(FeignDeath);
                Thread.Sleep(1500);
            }
            if (!AspecoftheDragonhawk.KnownSpell)
            {
                Extension.BuffSpell(AspecoftheMonkey);
            }
            if (AspecoftheDragonhawk.KnownSpell)
            {
                Extension.BuffSpell(AspecoftheDragonhawk);
            }


        }

        //Pethandle in Fight
        if (ObjectManager.Pet.HealthPercent < 90
            && PetHealTimer.IsReady)
        {
            Extension.PetSpell(MendPet);
            PetHealTimer = new Timer(1000 * 15);
            return;
        }
        //Backpaddle
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
        wManager.Events.FightEvents.OnFightLoop += (unit, cancelable) => {
            if (ObjectManager.Target.GetDistance < 11 && ObjectManager.Target.IsTargetingMyPet)
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
            return;
        }
        if (CallPet.IsSpellUsable
            && CallPet.KnownSpell
            && !ObjectManager.Pet.IsValid
            && !ObjectManager.Me.IsMounted)
        {
            CallPet.Launch();
            Usefuls.WaitIsCasting();
            return;
        }
    }

    #region Pull
    private static void Pull()
    {
        if (MyTarget.IsAttackable
            && ObjectManager.Me.Target > 0
            && ObjectManager.Pet.IsValid
            && ObjectManager.Pet.IsAlive
            && PetPullTimer.IsReady)
        {
            Lua.LuaDoString("PetAttack();");
            PetPullTimer = new Timer(500);
        }
    }

    #endregion

    private static void BuffRotation()
    {
        if (!AspecoftheHawk.KnownSpell)
        {
            Extension.BuffSpell(AspecoftheMonkey);
        }

        if (Me.ManaPercentage < 25)
        {
            Extension.BuffSpell(AspecoftheViper);
        }

        if (Me.ManaPercentage > 90 && Me.Level < 20 && ObjectManager.Me.Level > 4) //def for lowlevel
        {
            Extension.BuffSpell(AspecoftheCheetah);
        }

        if (!AspecoftheDragonhawk.KnownSpell && !AspecoftheCheetah.HaveBuff && Me.ManaPercentage > 90)
        {
            Extension.BuffSpell(AspecoftheHawk);
        }

        if (Me.ManaPercentage > 90 && !AspecoftheCheetah.HaveBuff)
        {
            Extension.BuffSpell(AspecoftheDragonhawk);
        }
    }

    //FoodPart for Pet
    public static void Feed()
    {
        if (ObjectManager.Pet.IsAlive && PetFeedTimer.IsReady && Lua.LuaDoString<int>("happiness, damagePercentage, loyaltyRate = GetPetHappiness() return happiness", "") < 3)
            FeedPet();
        PetFeedTimer = new Timer(1000 * 15);
        return;
    }

    public static string PetFoodType()
    {
        return (string)Lua.LuaDoString<string>("return GetPetFoodTypes();", "");
    }

    public static List<string> FoodList()
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

    public static List<string> Fungus()
    {
        return new List<string>() { "Raw Black Truffle" };
    }

    public static List<string> FishList()
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

    public static List<string> FruitList()
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

    public static List<string> BreadList()
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

    public static void FeedByType(List<string> list)
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
    public static void FeedPet()
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