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


public static class HunterBeastMastery
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
        //HunterBeastMasterySettings.Load(); //to add  Settings later
        if (ObjectManager.Me.WowClass == WoWClass.Hunter)
        {
            HunterBeastMasterySettings.Load();
            Logging.Write("Hunter Beastmaster Class...loading...");
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
        HunterBeastMasterySettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(HunterBeastMasterySettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        HunterBeastMasterySettings.CurrentSetting.Save();
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
                    if (HunterBeastMasterySettings.CurrentSetting.Petfeed)
                        Feed(); //Sub for Petfeeding
                }
                else
                {
                    if (HunterBeastMasterySettings.CurrentSetting.Framelock)
                        Extension.Framelock();

                    if (Fight.InFight && Me.Target > 0UL && ObjectManager.Target.IsAttackable)
                        CombatRotation();

                    if (HunterBeastMasterySettings.CurrentSetting.Framelock)
                        Extension.Frameunlock();
                }
            }
            catch (Exception e)
            {
                Logging.WriteError("error" + e);
            }
            Thread.Sleep(HunterBeastMasterySettings.CurrentSetting.Delay);
        }
        Logging.Write("STOPPED");
    }

    private static void CombatRotation()
    {
        // Pet attack
        if (Fight.InFight && Me.Target > 0UL 
            && ObjectManager.Target.IsAttackable
            && ObjectManager.Pet.Target != Me.Target)
            Lua.LuaDoString("PetAttack();", false);

        if (Extension.InterruptableUnit(20f) != null 
            && Intimidation.KnownSpell 
            && Intimidation.IsSpellUsable)
            {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.Target = Extension.InterruptableUnit(20f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(20f).Guid);
            Extension.FightSpell(Intimidation);
            }
        Extension.FightSpell(BestialWrath);
        //Ranged Attacks
        if (MyTarget.GetDistance >= 7)
        {
            if(MyTarget.HealthPercent < 20)
            {
            Extension.FightSpell(KillShot);
            }
            Extension.FightSpell(HuntersMark);
            Extension.BuffSpell(RapidFire);
            if (!Me.HaveBuff("Kill Command"))
            {
                Extension.FightSpell(KillCommand);
            }
            Extension.FightSpell(SerpentSting);         
            Extension.FightSpell(ArcaneShot);
            Extension.FightSpell(MultiShot);
            Extension.FightSpell(SteadyShot, false, false, true);

            if (Me.ManaPercentage < HunterBeastMasterySettings.CurrentSetting.AspecofViper)
            {
                Extension.BuffSpell(AspecoftheViper);
            }
            if (Me.ManaPercentage > 30)
            {
                Extension.BuffSpell(AspecoftheDragonhawk);
            }
            if (!AspecoftheDragonhawk.KnownSpell && Me.ManaPercentage > 30)
            {
                Extension.BuffSpell(AspecoftheHawk);
            }
        }
        //Close  Combat  Attacks
        if (MyTarget.GetDistance <= 6)
        {
            Extension.FightSpell(RaptorStrike);
        }

    }

    private static void BuffRotation()
    {
        if (Me.ManaPercentage < HunterBeastMasterySettings.CurrentSetting.AspecofViper)
        {
            Extension.BuffSpell(AspecoftheViper);
        }
        if (!AspecoftheDragonhawk.KnownSpell && !AspecoftheCheetah.HaveBuff
            && (Me.ManaPercentage > 90 || !AspecoftheViper.KnownSpell))
        {
            Extension.BuffSpell(AspecoftheHawk);
        }
        if (Me.ManaPercentage > 90 && !AspecoftheCheetah.HaveBuff)
        {
            Extension.BuffSpell(AspecoftheDragonhawk);
        }
        if (HunterBeastMasterySettings.CurrentSetting.Checkpet)
        {
            if (ObjectManager.Pet.IsAlive && ObjectManager.Pet.IsValid
                && ObjectManager.Pet.HealthPercent < HunterBeastMasterySettings.CurrentSetting.PetHealth)
                if (PetHealTimer.IsReady)
                {
                    Extension.PetSpell(MendPet);
                    PetHealTimer = new Timer(1000 * 15);
                }
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
