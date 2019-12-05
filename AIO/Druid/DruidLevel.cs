using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager.Events;
using System.Collections.Generic;
using System.Linq;

public static class DruidLevel
{

    public static bool lowlevel;
    private static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static string Groundmount;

    //Damage Spells
    public static Spell Wrath = new Spell("Wrath"); //lvl1
    public static Spell Moonfire = new Spell("Moonfire");//lvl4
    public static Spell InsectSwarm = new Spell("Insect Swarm");// 20
    public static Spell Starfire = new Spell("Star Fire"); //20
    public static Spell ForceofNature = new Spell("Force of Nature"); //50
    public static Spell Starfall = new Spell("Starfall"); //60

    //Damage Bear and Cat
    public static Spell Maul = new Spell("Maul"); //lvl10
    public static Spell Bash = new Spell("Bash"); //lvl14
    public static Spell SwipeBear = new Spell("Swipe (Bear)"); //16
    public static Spell Claw = new Spell("Claw"); //20
    public static Spell FeralChargeBear = new Spell("Feral Charge - Bear"); //20
    public static Spell FeralChargeCat = new Spell("Feral Charge - Cat"); //20
    public static Spell Rip = new Spell("Rip"); //20
    public static Spell Rake = new Spell("Rake"); //24
    public static Spell FerociousBite = new Spell("Ferocious Bite"); //32
    public static Spell Ravage = new Spell("Ravage"); //32
    public static Spell Pounce = new Spell("Pounce"); //36
    public static Spell MangleBear = new Spell("Mangle (Bear)"); //50
    public static Spell MangleCat = new Spell("Mangle (Cat)"); //50	
    public static Spell Shred = new Spell("Shred"); //54
    public static Spell Maim = new Spell("Maim"); //62
    public static Spell Lacerate = new Spell("Lacerate"); //66

    //Buff General
    public static Spell MarkoftheWild = new Spell("Mark of the Wild"); //lvl1
    public static Spell Thorns = new Spell("Thorns"); //lvl6
    public static Spell Enrage = new Spell("Enrage"); //lvl12
    public static Spell TigersFury = new Spell("Tiger's Fury"); //24
    public static Spell Dash = new Spell("Dash"); //26
    public static Spell NaturesSwiftness = new Spell("Nature's Swiftness"); //30
    public static Spell FrenziedRegeneration = new Spell("Frenzied Regeneration"); //36 
    public static Spell LeaderofthePack = new Spell("Leader of the Pack"); //40
    public static Spell Barkskin = new Spell("Barkskin"); //44
    public static Spell Berserk = new Spell("Berserk"); //60

    //Buff Forms
    public static Spell BearForm = new Spell("Bear Form"); //10
    public static Spell AquaticForm = new Spell("Aquatic Form"); //16
    public static Spell TravelForm = new Spell("Travel Form"); //16
    public static Spell CatForm = new Spell("Cat Form"); //20
    public static Spell DireBearForm = new Spell("Dire Bear Form"); //40
    public static Spell MoonkinForm = new Spell("Moonkin Form"); //40
    public static Spell TreeofLife = new Spell("Tree of Life"); //50

    //Healing Spells
    public static Spell HealingTouch = new Spell("Healing Touch"); //lvl1
    public static Spell Rejuvenation = new Spell("Rejuvenation"); //lvl4
    public static Spell Regrowth = new Spell("Regrowth"); //lvl12
    public static Spell Rebirth = new Spell("Rebirth"); //20
    public static Spell Tranquility = new Spell("Tranquility"); //30
    public static Spell Swiftmend = new Spell("Swiftmend"); //40
    public static Spell WildGrowth = new Spell("Wild Growth");//60
    public static Spell Lifebloom = new Spell("Lifebloom"); //64
    public static Spell Nourish = new Spell("Nourish"); //80

    //Usefull Spells
    public static Spell Growl = new Spell("Growl"); //lvl10
    public static Spell FaerieFireFeral = new Spell("Faerie Fire (Feral)"); //18
    public static Spell FaerieFire = new Spell("Faerie Fire"); //18
    public static Spell Prowl = new Spell("Prowl"); //20
    public static Spell Cower = new Spell("Cower"); //28
    public static Spell Innervate = new Spell("Innvervate"); //40

    //CC
    public static Spell EntanglingRoots = new Spell("Entangling Roots"); //lvl8
    public static Spell NaturesGrasp = new Spell("Nature's Grasp");//lvl10
    public static Spell Hibernate = new Spell("Hibernate"); //18
    public static Spell Typhoon = new Spell("Typhoon"); //50

    //Debuffs
    public static Spell DemoralizingRoar = new Spell("Demoralizing Roar"); //lvl10
    public static Spell ChallengingRoar = new Spell("Challenging Roar"); //28

    //Cure
    public static Spell CurePoison = new Spell("Cure Poison"); //lvl14
    public static Spell RemoveCurse = new Spell("Remove Curse"); //24
    public static Spell AbolishPoison = new Spell("Abolish Poison"); //26



    public static void Initialize()
    {
        {
            _isLaunched = true;
            DruidLevelSettings.Load();
            Talents.InitTalents(DruidLevelSettings.CurrentSetting.AssignTalents,
                                DruidLevelSettings.CurrentSetting.UseDefaultTalents,
                                DruidLevelSettings.CurrentSetting.TalentCodes.ToArray());
            Logging.Write("Druid Low Level Class...loading...");
            Groundmount = wManager.wManagerSetting.CurrentSetting.GroundMountName;
            if (wManager.wManagerSetting.CurrentSetting.GroundMountName == string.Empty)
            {
                wManager.wManagerSetting.CurrentSetting.GroundMountName = "Travel Form";
            }
            Rotation();
        }
    }

    public static void Dispose() // When product stopped
    {
        {
            wManager.wManagerSetting.CurrentSetting.GroundMountName = Groundmount;
            _isLaunched = false;
        }
    }

    public static void ShowConfiguration() // When use click on Fight class settings
    {
        DruidLevelSettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(DruidLevelSettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        DruidLevelSettings.CurrentSetting.Save();
    }

    public static void Rotation()
    {
        Logging.Write("Rotation Loaded");
        while (_isLaunched)
        {
            try
            {
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
                {
                    if (Me.Level < 20 && !BearForm.KnownSpell)
                    {
                        Main.settingRange = 29f;
                    }
                    if (BearForm.KnownSpell)
                    {
                        Main.settingRange = 5f;
                    }

                    if (!(Fight.InFight))
                    {
                        Healing();
                        BuffRotation();
                        Pull();
                    }
                    else
                     if (Me.Target > 0)
                    {
                        if (DruidLevelSettings.CurrentSetting.Framelock)
                        {
                            Extension.Framelock();
                        }
                        CombatRotation();
                        if (DruidLevelSettings.CurrentSetting.Framelock)
                        {
                            Extension.Frameunlock();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(DruidLevelSettings.CurrentSetting.Delay);
        }
    }

    public static void CombatRotation()
    {
        if (Me.Level < 10)
        {
            Extension.FightSpell(Wrath);
            Extension.FightSpell(Moonfire);
        }
        if (Me.Level > 9 && Me.Level < 20)
        {
            if (BearForm.KnownSpell)
            {
                Extension.BuffSpell(BearForm);
            }
            if (Me.HaveBuff(BearForm.Id))
            {
                Extension.FightSpell(DemoralizingRoar);
                Extension.FightSpell(Maul);
                Extension.FightSpell(SwipeBear);
            }

            if (Me.HaveBuff(BearForm.Id) && MyTarget.GetDistance > 10)
            {
                Extension.FightSpell(Growl);
            }
            if (Me.HaveBuff(BearForm.Id) && MyTarget.GetDistance > 10)
            {
                Extension.FightSpell(FaerieFireFeral);
            }
            if (!Me.HaveBuff(BearForm.Id))
            {
                Extension.FightSpell(Moonfire);
            }
            if (!Me.HaveBuff(BearForm.Id))
            {
                Extension.FightSpell(Wrath);
            }
        }
        if (Me.Level > 19 && Me.Level < 80)
        {
            // Heal in Combat
            if (Me.HealthPercent < 36 && Me.ManaPercentage > 30 && MyTarget.HealthPercent > 13)
            {
                Extension.BuffSpell(Barkskin);
                Extension.HealSpell(Regrowth);
                if (!Regrowth.KnownSpell)
                {
                    Extension.HealSpell(HealingTouch);
                }
            }
            if (Me.HealthPercent < 92 && Me.ManaPercentage > 40 && Me.HaveBuff("Predator's Swiftness"))
            {
                Extension.HealSpell(Regrowth);
            }
            if (Me.Energy < 40 && Me.ComboPoint > 1 && MyTarget.HealthPercent > 40)
            {
                Extension.BuffSpell(TigersFury);
            }
            if (Extension.GetAttackingUnits(20).Count() > 1 && MyTarget.HealthPercent > 20)
            {
                if (Extension.GetAttackingUnits(20).Count() == 1 && Me.ManaPercentage > 40) //for Rejuve after bearform and one target died
                {
                    Extension.HealSpell(Rejuvenation);
                }
                if (!Me.HaveBuff(BearForm.Id) && !DireBearForm.KnownSpell)
                {
                    Extension.BuffSpell(BearForm);
                }
                if (!Me.HaveBuff(DireBearForm.Id))
                {
                    Extension.BuffSpell(DireBearForm);
                }
                if (Me.Rage > 16)
                {
                    Extension.FightSpell(Maul);
                }
                Extension.FightSpell(MangleBear, false, false, false, false);
                Extension.FightSpell(FaerieFireFeral);
                if (Extension.GetAttackingUnits(10).Count() > 3)
                {
                    Extension.FightSpell(DemoralizingRoar);
                }

            }
            if (Extension.GetAttackingUnits(20).Count() < 2)
            {
                if (!Me.HaveBuff(CatForm.Id))
                {
                    Extension.BuffSpell(CatForm);
                }
                if (Me.HaveBuff(CatForm.Id) && !Me.HaveBuff(Prowl.Id) && DruidLevelSettings.CurrentSetting.Prowl)
                {
                    Extension.BuffSpell(Prowl);
                }
                if (Me.HaveBuff(Prowl.Id))
                {
                    if (DruidLevelSettings.CurrentSetting.Dash)
                    {
                        Extension.BuffSpell(Dash);
                    }
                    Extension.FightSpell(Ravage);
                    Extension.FightSpell(Pounce);
                    Extension.FightSpell(Shred);
                }
                if (DruidLevelSettings.CurrentSetting.FFF && !Me.HaveBuff(Prowl.Id))
                {
                    Extension.FightSpell(FaerieFireFeral);
                }
                if (Extension.GetAttackingUnits(10).Count() > 1 && !DruidLevelSettings.CurrentSetting.TF)
                {
                    Extension.BuffSpell(TigersFury);
                }
                if (DruidLevelSettings.CurrentSetting.TF && !Me.HaveBuff(Prowl.Id))
                {
                    Extension.BuffSpell(TigersFury);
                }
                if (Me.ComboPoint <= 4 && MyTarget.HealthPercent >= 40)
                {
                    Extension.FightSpell(Rake);
                }
                if (!MangleCat.KnownSpell && Me.ComboPoint <= 4)
                {
                    Extension.FightSpell(Claw);
                }
                if (MangleCat.KnownSpell && Me.ComboPoint <= 4)
                {
                    Extension.FightSpell(MangleCat, false, false, false, false);
                }
                if (MyTarget.HealthPercent < DruidLevelSettings.CurrentSetting.FBH && Me.ComboPoint >= DruidLevelSettings.CurrentSetting.FBC)
                {
                    Extension.FightSpell(FerociousBite);
                }
                if (Me.ComboPoint >= 4 && MyTarget.HealthPercent > 50)
                {
                    Extension.FightSpell(Rip);
                }
            }

        }

    }

    private static void Pull()
    {
        if (Me.Level > 9 && Me.Level < 20)
        {
            if (Me.HasTarget && MyTarget.IsAttackable)
            {
                if (Me.HaveBuff(BearForm.Id))
                {
                    Extension.FightSpell(Growl);
                    Extension.FightSpell(FaerieFireFeral);
                }
            }
        }
        if (Me.Level > 19 && Me.Level < 81)
        {
            if (Me.HasTarget && MyTarget.IsAttackable)
            {
                if (!Me.HaveBuff(CatForm.Id))
                {
                    Extension.BuffSpell(CatForm);

                    if (!Me.HaveBuff(Prowl.Id))
                    {
                        Extension.BuffSpell(Prowl);

                        if (Me.HaveBuff(Prowl.Id))
                        {
                            Extension.FightSpell(Ravage);
                            Extension.FightSpell(Pounce);
                            Extension.FightSpell(Shred);
                        }
                        if (!Prowl.IsSpellUsable)
                        {
                            Extension.FightSpell(FaerieFireFeral);
                        }

                    }
                }
            }
        }
    }

    private static void BuffRotation()
    {
        Extension.BuffSpell(MarkoftheWild);
        if (Me.Rage < 20)
        {
            Extension.BuffSpell(Thorns);
        }
        if (Me.Rage < 20)
        {
            Extension.BuffSpell(Thorns);
        }
        if (!Me.HaveBuff(CatForm.Id) && Me.HaveBuff(Thorns.Id) && Me.HaveBuff(MarkoftheWild.Id) && !Me.IsMounted && wManager.wManagerSetting.CurrentSetting.GroundMountName == string.Empty)
        {
            Logging.Write("Using  Catform, traveltime");
            Extension.BuffSpell(CatForm);
        }
    }

    private static void Healing()
    {
        if (Me.HealthPercent < 30 && Me.ManaPercentage > 30)
        {
            Extension.HealSpell(Regrowth);
            if (!Regrowth.KnownSpell)
            {
                Extension.HealSpell(HealingTouch);
            }
        }

        if (Me.HealthPercent < 60 && Me.ManaPercentage > 10)
        {
            Extension.HealSpell(Rejuvenation);
            Extension.HealSpell(Lifebloom);
        }

    }
}
