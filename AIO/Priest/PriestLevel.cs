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


public static class PriestLevel
{

    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    private static bool _isLaunched;
    private static bool _icanusewand = Extension.HaveRangedWeaponEquipped();
    //Damage Spells
    public static Spell Smite = new Spell("Smite");//lvl 1
    public static Spell ShadowWordPain = new Spell("Shadow Word: Pain");//lvl 4
    public static Spell MindBlast = new Spell("Mind Blast"); //lvl 10
    public static Spell DevouringPlague = new Spell("Devouring Plague"); //lvl 20
    public static Spell HolyFire = new Spell("Holy Fire"); //lvl20
    public static Spell HolyNova = new Spell("Holy Nova"); //lvl20
    public static Spell MindFlay = new Spell("Mind Flay"); //lvl20
    public static Spell ManaBurn = new Spell("Mana Burn"); //lvl24
    public static Spell VampiricTouch = new Spell("Vampiric Touch"); //lvl50
    public static Spell ShadowWordDeath = new Spell("Shadow Word: Death");//lvl 62
    //Buff Spells
    public static Spell Fortitude = new Spell("Power Word: Fortitude");//lvl 1
    public static Spell Shield = new Spell("Power Word: Shield");//lvl 6
    public static Spell InnerFire = new Spell("Inner Fire");//lvl12
    public static Spell DivineSpirit = new Spell("Divine Spirit"); //lvl30
    public static Spell ShadowProtection = new Spell("Shadow Protection"); //lvl30
    public static Spell SoulWarding = new Spell("Soul Warding"); //lvl30
    public static Spell VampiricEmbrace = new Spell("Vampiric Embrace"); //lvl30
    public static Spell PowerInfusion = new Spell("Power Infusion"); //lvl40
    public static Spell ShadowForm = new Spell("Shadowform"); //lvl40
    //Heal Spells
    public static Spell LesserHeal = new Spell("Lesser Heal");//lvl 1
    public static Spell Renew = new Spell("Renew");//lvl 8
    public static Spell Resurrection = new Spell("Resurrection");//lvl 10
    public static Spell CureDisease = new Spell("Cure Disease");//lvl14
    public static Spell Heal = new Spell("Heal");//lvl 16
    public static Spell BlessedHealing = new Spell("Blessed Healing");//lvl 16	
    public static Spell DesperatePrayer = new Spell("Desperate Prayer");//lvl 20
    public static Spell FlashHeal = new Spell("Flash Heal");//lvl 20		
    public static Spell PrayerofMending = new Spell("Prayer of Mending"); //30
    public static Spell PrayerofHealing = new Spell("Prayer of Healing");
    public static Spell AbolishDisease = new Spell("Abolish Disease"); //30
    public static Spell GreaterHEal = new Spell("Greater Heal"); //lvl40
    public static Spell CircleofHealing = new Spell("Circle of Healing"); //lvl50
    public static Spell GuardianSpirit = new Spell("Guardian Spirit"); //lvl60
    public static Spell Penance = new Spell("Penance"); //lvl60
    public static Spell BindingHeal = new Spell("Binding Heal"); //lvl64
    public static Spell DivineHymn = new Spell("Divine Hymn"); //lvl80
    public static Spell HymnofHope = new Spell("Hymn of Hope"); //lvl80
                                                                //Usefull Spells
    public static Spell PsychicScream = new Spell("Psychic Scream");//lvl14
    public static Spell DispelMagic = new Spell("Dispel Magic");//lvl18	
    public static Spell FearWard = new Spell("Fear Ward");//lvl20	
    public static Spell InnerFocus = new Spell("Inner Focus");// lvl20
    public static Spell MindSoothe = new Spell("Mind Soothe"); //lvl20
    public static Spell ShackleUndead = new Spell("Shackle Undead"); //lvl20
    public static Spell Silence = new Spell("Silence"); //lvl 30
    public static Spell PainSuppression = new Spell("Pain Suppression"); //lvl50
    public static Spell PsychicHorror = new Spell("Psychic Horror"); //lvl50
    public static Spell Dispersion = new Spell("Dispersion"); //lvl60
    public static Spell ShadowFiend = new Spell("Shadow Fiend"); //lvl66
    public static Spell MassDispel = new Spell("Mass Dispel"); //lvl70
    private static Spell UseWand = new Spell("Shoot");
    private static String Spec;



    public static void Initialize()
    {
        if (ObjectManager.Me.WowClass == WoWClass.Priest && ObjectManager.Me.Level < 80)
        {
            #region Loggin Settings
            Logging.Write("Priest Low Level Class...loading...");
            #endregion
            PriestLevelSettings.Load();
            Main.kindofclass = PriestLevelSettings.CurrentSetting.ChooseTalent;
            Talents.InitTalents(PriestLevelSettings.CurrentSetting.AssignTalents,
                                PriestLevelSettings.CurrentSetting.UseDefaultTalents,
                                PriestLevelSettings.CurrentSetting.TalentCodes.ToArray());
            Logging.Write("PriestLevelSettings Loaded");

            _isLaunched = true;
            if (Me.Level < 10)
            {
                Spec = "Under10";
            }
            if (Extension.GetSpec() == "Shadow")
            {
                Spec = "Shadow";
            }
            if (Extension.GetSpec() == "Disciple")
            {
                Spec = "Disci";
            }
            if (Extension.GetSpec() == "Holy")
            {
                Spec  = "Holy";
            }
            Rotation();
        }
        else
        {
            Logging.Write("No Priest....unloading...");
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
                Main.settingRange = 29f;
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause && !Fight.InFight)
                {
                    if(Spec == "Under10")
                        {
                        Under10Healrotation();
                        }
                    if(Spec == "Holy")
                        {
                            HolyHealRotation();
                        }
                    BuffRotation();
                }
                else
                {
                    if (Spec == "Under10")
                    {
                        Under10Rotation();
                        Under10Healrotation();
                    }
                    if (Spec == "Shadow")
                    {
                        ShadowRotation();
                        ShadowHealRotation();
                    }
                    if (Spec == "Disci")
                    {
                        DisciRotation();
                        DisciHealRotation();
                    }
                    if(Spec == "Holy")
                    {
                        HolyRotation();
                        HolyHealRotation();
                    }
                }

            }
            catch (Exception e)
            {
                Logging.WriteError("error" + e);
            }

            Thread.Sleep(PriestLevelSettings.CurrentSetting.Delay);
        }
        Logging.Write("STOPPED");
    }

    public static void ShowConfiguration() // When a configuration is declared
    {
        PriestLevelSettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(PriestLevelSettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        PriestLevelSettings.CurrentSetting.Save();
        Main.kindofclass = PriestLevelSettings.CurrentSetting.ChooseTalent;
    }



    private static void Under10Rotation()
    {
        Extension.FightSpell(ShadowWordPain);
        Extension.FightSpell(Smite);
    }
    private static void ShadowRotation()
    {
        Extension.BuffSpell(ShadowForm);
        if (Extension.GetAttackingUnits(6).Count() > 1)
        {
            Extension.FightSpell(PsychicScream);
        }
        if (Extension.GetAttackingUnits(6).Count() > 1)
        {
            Extension.BuffSpell(InnerFire);
        }
        if (_icanusewand && Me.ManaPercentage < 5)
        {
            Extension.FightSpell(UseWand);
        }
        if (_icanusewand
            && PriestLevelSettings.CurrentSetting.UseWand
            && MyTarget.HealthPercent < PriestLevelSettings.CurrentSetting.UseWandTresh)
        {
            Extension.FightSpell(UseWand);
            return;
        }
        if (Me.Level >= 10 && Me.Level < 20)
        {
            Extension.FightSpell(MindBlast);
            if (!MindBlast.IsSpellUsable)
            {
                Extension.FightSpell(ShadowWordPain);
            }
            if (!MindBlast.IsSpellUsable)
            {
                Extension.FightSpell(Smite);
            }
        }
        if (Me.Level > 19 && Me.Level < 30)
        {
            if (Extension.GetAttackingUnits(20).Count() > 1 && !Me.IsStunned)
            {
                WoWUnit MainTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit DotTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Max(x => x.HealthPercent)).FirstOrDefault();
                if (DotTarget != MainTarget)
                {
                    ObjectManager.Me.FocusGuid = DotTarget.Guid;
                    Extension.FightSpell(ShadowWordPain, true, true);
                    Logging.Write("Cast Dot on " + DotTarget);
                    Thread.Sleep(50);
                }
            }
            Extension.FightSpell(HolyFire);
            Extension.FightSpell(DevouringPlague);
            Extension.FightSpell(ShadowWordPain);
            if (MyTarget.HaveBuff(DevouringPlague.Id) && MyTarget.HaveBuff(ShadowWordPain.Id) && PriestLevelSettings.CurrentSetting.ShadowUseMindflay)
            {
                Extension.FightSpell(MindFlay);
            }
            Extension.FightSpell(MindBlast);
        }
        if (Me.Level > 29 && Me.Level < 40)
        {
            //Vampiric Touch > Vampiric Embrace > Mind Blast > Mind Flay
            if (Extension.GetAttackingUnits(20).Count() > 1 && !Me.IsStunned)
            {
                WoWUnit MainTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit DotTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Max(x => x.HealthPercent)).FirstOrDefault();
                if (DotTarget != MainTarget)
                {
                    ObjectManager.Me.FocusGuid = DotTarget.Guid;
                    Extension.FightSpell(ShadowWordPain, true, true);
                    Logging.Write("Cast Dot on " + DotTarget);
                    Thread.Sleep(50);
                }
            }
            Extension.FightSpell(HolyFire);
            Extension.FightSpell(DevouringPlague);
            if (MyTarget.HaveBuff(DevouringPlague.Id))
            {
                Extension.BuffSpell(VampiricEmbrace);
            }
            Extension.FightSpell(ShadowWordPain);
            if (MyTarget.HaveBuff(DevouringPlague.Id) && MyTarget.HaveBuff(ShadowWordPain.Id) && PriestLevelSettings.CurrentSetting.ShadowUseMindflay)
            {
                Extension.FightSpell(MindFlay);
            }
            Extension.FightSpell(MindBlast);
        }
        if (Me.Level > 39 && Me.Level < 50)
        {
            //Vampiric Touch > Vampiric Embrace > Mind Blast > Mind Flay
            if (Extension.GetAttackingUnits(20).Count() > 1 && !Me.IsStunned)
            {
                WoWUnit MainTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit DotTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Max(x => x.HealthPercent)).FirstOrDefault();
                if (DotTarget != MainTarget)
                {
                    ObjectManager.Me.FocusGuid = DotTarget.Guid;
                    Extension.FightSpell(ShadowWordPain, true, true);
                    Logging.Write("Cast Dot on " + DotTarget);
                    Thread.Sleep(50);
                }
            }
            if (PriestLevelSettings.CurrentSetting.ShadowDPUse)
            {
                Extension.FightSpell(DevouringPlague);
            }
            Extension.FightSpell(ShadowWordPain);
            if (MyTarget.HaveBuff(ShadowWordPain.Id))
            {
                Extension.BuffSpell(VampiricEmbrace);
            }
            Extension.FightSpell(MindBlast);
            if (MyTarget.HaveBuff(ShadowWordPain.Id) && PriestLevelSettings.CurrentSetting.ShadowUseMindflay)
            {
                Extension.FightSpell(MindFlay);
            }
        }
        if (Me.Level > 49 && Me.Level < 60)
        {
            //Vampiric Touch > Vampiric Embrace > Mind Blast > Mind Flay
            if (Extension.GetAttackingUnits(20).Count() > 1 && !Me.IsStunned)
            {
                WoWUnit MainTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit DotTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Max(x => x.HealthPercent)).FirstOrDefault();
                if (DotTarget != MainTarget)
                {
                    ObjectManager.Me.FocusGuid = DotTarget.Guid;
                    Extension.FightSpell(ShadowWordPain, true, true);
                    Logging.Write("Cast Dot on " + DotTarget);
                    Thread.Sleep(50);
                }
            }
            if (PriestLevelSettings.CurrentSetting.ShadowDPUse)
            {
                Extension.FightSpell(DevouringPlague);
            }
            Extension.FightSpell(VampiricTouch);
            if (MyTarget.HaveBuff(VampiricTouch.Id))
            {
                Extension.FightSpell(ShadowWordPain);
            }
            Extension.BuffSpell(VampiricEmbrace);
            Extension.FightSpell(MindBlast);
            if (MyTarget.HaveBuff(VampiricTouch.Id)
                && MyTarget.HaveBuff(ShadowWordPain.Id) && PriestLevelSettings.CurrentSetting.ShadowUseMindflay)
            {
                Extension.FightSpell(MindFlay);
            }
        }
        if (Me.Level > 59)
        {
            //Vampiric Touch > Vampiric Embrace > Mind Blast > Mind Flay
            if (Extension.GetAttackingUnits(20).Count() > 1 && !Me.IsStunned)
            {
                WoWUnit MainTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Min(x => x.HealthPercent)).FirstOrDefault();
                WoWUnit DotTarget = Extension.GetAttackingUnits(20).Where(u => u.HealthPercent == Extension.GetAttackingUnits(20).Max(x => x.HealthPercent)).FirstOrDefault();
                if (DotTarget != MainTarget)
                {
                    ObjectManager.Me.FocusGuid = DotTarget.Guid;
                    Extension.FightSpell(ShadowWordPain, true, true);
                    Logging.Write("Cast Dot on " + DotTarget);
                    Thread.Sleep(50);
                }
            }
            if (PriestLevelSettings.CurrentSetting.ShadowDPUse)
            {
                Extension.FightSpell(DevouringPlague);
            }
            Extension.FightSpell(VampiricTouch);
            if (MyTarget.HaveBuff(VampiricTouch.Id))
            {
                Extension.FightSpell(ShadowWordPain);
            }
            Extension.BuffSpell(VampiricEmbrace);
            if (Me.ManaPercentage < 20)
            {
                Extension.FightSpell(ShadowFiend);
            }
            Extension.FightSpell(MindBlast);
            if (MyTarget.HaveBuff(VampiricTouch.Id)
                && MyTarget.HaveBuff(ShadowWordPain.Id) && PriestLevelSettings.CurrentSetting.ShadowUseMindflay)
            {
                Extension.FightSpell(MindFlay);
            }
        }
    }
    private static void DisciRotation()
    {
    }
    private static void HolyRotation()
    {
        if(PriestLevelSettings.CurrentSetting.HolyDamage)
        {
        Extension.FightSpell(Smite);
        Extension.FightSpell(HolyFire);
        }
    }
    private static void Under10Healrotation()
    {
        Extension.GroupHealSpell(Shield, 99);
        Extension.GroupHealSpell(Renew, 90);
        Extension.GroupHealSpell(LesserHeal, 75);
    }
    private static void ShadowHealRotation()
    {
        if (Me.HealthPercent < PriestLevelSettings.CurrentSetting.ShadowUseShieldTresh && Extension.GetAttackingUnits(20).Count() > 0)
        {
            Extension.BuffSpell(Shield);
        }
        if (Me.HealthPercent < PriestLevelSettings.CurrentSetting.ShadowUseRenewTresh && Extension.GetAttackingUnits(20).Count() >= 1)
        {
            Extension.HealSpell(Renew);
        }
        if (Me.HealthPercent < PriestLevelSettings.CurrentSetting.ShadowUseLessFlashTresh && Extension.GetAttackingUnits(20).Count() > 0 && MyTarget.HealthPercent > 10 && !FlashHeal.KnownSpell)
        {
            Extension.HealSpell(LesserHeal);
        }
        if (Me.HealthPercent < PriestLevelSettings.CurrentSetting.ShadowUseLessFlashTresh && Extension.GetAttackingUnits(20).Count() > 0 && MyTarget.HealthPercent > 10 && FlashHeal.KnownSpell)
        {
            Extension.HealSpell(FlashHeal);
        }
    }
    private static void DisciHealRotation()
    {

    }
    private static void HolyHealRotation()
    {
        Extension.TankHealSpell(PrayerofMending, PriestLevelSettings.CurrentSetting.HolyPrayerofMendingTreshTank);
        Extension.TankHealSpell(Renew, PriestLevelSettings.CurrentSetting.HolyUseRenewTreshTank);
        Extension.TankHealSpell(BindingHeal, PriestLevelSettings.CurrentSetting.HolyBindingHealTreshTank);
        if (Me.HaveBuff("Serendipity"))
        {
            Extension.TankHealSpell(GreaterHEal, PriestLevelSettings.CurrentSetting.HolyGreaterHealTreshTank);
        }
        else
        {
            Extension.TankHealSpell(BindingHeal, PriestLevelSettings.CurrentSetting.HolyGreaterHealTreshTank);
        }
        Extension.GroupHealSpell(GuardianSpirit, PriestLevelSettings.CurrentSetting.HolyGuardianSpiritTreshGroup, PriestLevelSettings.CurrentSetting.HolyGuardianSpiritCountTreshGroup);
        Extension.GroupHealSpell(CircleofHealing, PriestLevelSettings.CurrentSetting.HolyCircleofHealingTreshGroup);
        Extension.GroupHealSpell(PrayerofMending, PriestLevelSettings.CurrentSetting.HolyPrayerofMendingTreshGroup);
        Extension.GroupHealSpell(PrayerofHealing, PriestLevelSettings.CurrentSetting.HolyPrayerofHealingTreshGroup);
    }
    private static void BuffRotation()
    {
        Extension.GroupBuffSpell(Fortitude);
        Extension.BuffSpell(ShadowForm);
        Extension.BuffSpell(DivineSpirit);
        if (Me.ManaPercentage < 25)
        {
            Extension.BuffSpell(Dispersion, false, true);
        }

        if (Me.HealthPercent < 20 && Extension.GetAttackingUnits(20).Count() > 0 && MyTarget.HealthPercent > 10 && !FlashHeal.KnownSpell)
        {
            Extension.HealSpell(LesserHeal);
        }
        if (Me.HealthPercent < 20 && Extension.GetAttackingUnits(20).Count() > 0 && MyTarget.HealthPercent > 10 && FlashHeal.KnownSpell)
        {
            Extension.HealSpell(FlashHeal);
        }
    }

}