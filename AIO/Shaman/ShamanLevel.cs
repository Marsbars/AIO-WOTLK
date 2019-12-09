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

public static class ShamanLevel
{
    public static bool lowlevel;
    public static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    static TotemManager totemManager = new TotemManager();
    internal static Vector3 _fireTotemPosition = null;

    //FightSpells
    private static Spell LightningBolt = new Spell("Lightning Bolt");
    private static Spell EarthShock = new Spell("Earth Shock");
    private static Spell FlameShock = new Spell("Flame Shock");
    private static Spell Stormstrike = new Spell("Stormstrike");
    private static Spell ShamanisticRage = new Spell("Shamanistic Rage");
    private static Spell LavaLash = new Spell("Lava Lash");
    private static Spell ChainLightning = new Spell("Chain Lightning");
    private static Spell Attack = new Spell("Attack");
    private static Spell FireNova = new Spell("Fire Nova");
    private static Spell WindShear = new Spell("Wind Shear");
    private static Spell FeralSpirit = new Spell("Feral Spirit");
    //HealSpells
    private static Spell HealingWave = new Spell("Healing Wave");
    private static Spell LesserHealingWave = new Spell("Lesser Healing Wave");
    private static Spell CurePoison = new Spell("Cure Toxins");

    //BuffSpells
    private static Spell LightningShield = new Spell("Lightning Shield");
    private static Spell WaterShield = new Spell("Water Shield");
    private static Spell GhostWolf = new Spell("Ghost Wolf");
    private static Spell WindfuryWeapon = new Spell("Windfury Weapon");
    private static Spell RockbiterWeapon = new Spell("Rockbiter Weapon");
    private static Spell FlametongueWeapon = new Spell("Flametongue Weapon");


    public static void Initialize()
    {
        ShamanLevelSettings.Load();
        Talents.InitTalents(ShamanLevelSettings.CurrentSetting.AssignTalents,
                            ShamanLevelSettings.CurrentSetting.UseDefaultTalents,
                            ShamanLevelSettings.CurrentSetting.TalentCodes.ToArray());
        Logging.Write("Shaman Low Level Class...loading...");
        {
            lowlevel = true;
            _isLaunched = true;

            Rotation();
        }
    }

    public static void Dispose()
    {

        {
            lowlevel = false;
            _isLaunched = false;
        }
    }

    public static void ShowConfiguration()
    {
        ShamanLevelSettings.Load();
        var settingWindow = new MarsSettingsGUI.SettingsWindow(ShamanLevelSettings.CurrentSetting, ObjectManager.Me.WowClass.ToString());
        settingWindow.ShowDialog();
        ShamanLevelSettings.CurrentSetting.Save();
    }

    internal static void Rotation()
    {
        while (_isLaunched)
        {
            try
            {
                if (Conditions.InGameAndConnectedAndAliveAndProductStartedNotInPause)
                {
                    if (lowlevel)
                    {
                        Main.settingRange = 25f;
                    }
                    if (!lowlevel)
                    {
                        Main.settingRange = 5f;
                    }
                    if (!Fight.InFight)
                    {
                        EnchantWeapon();
                        TotemManager.CheckForTotemicCall();
                        BuffRotation();
                        Pull();
                    }

                    if (Fight.InFight && ObjectManager.Me.HasTarget)
                    {

                        if (ShamanLevelSettings.CurrentSetting.Framelock)
                        {
                            Extension.Framelock();
                        }
                        CombatRotation();
                        if (ShamanLevelSettings.CurrentSetting.Framelock)
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
            Thread.Sleep(ShamanLevelSettings.CurrentSetting.Delay);
        }

    }
    private static void CombatRotation()
    {
        bool Poison = Extension.HasPoisonDebuff();
        bool Disease = Extension.HasDiseaseDebuff();
        if (Extension.InterruptableUnit(10f) != null && WindShear.KnownSpell && WindShear.IsSpellUsable)
        {
            Logging.Write("Interrupt Target found");
            ObjectManager.Me.FocusGuid = Extension.InterruptableUnit(10f).Guid;
            Logging.Write("Interrupt Target Set" + Extension.InterruptableUnit(10f).Guid);
            Extension.FightSpell(WindShear, true);
        }

        if (Me.HealthPercent < 30 && MyTarget.HealthPercent > ShamanLevelSettings.CurrentSetting.Enemylife)
        {
            Extension.HealSpell(HealingWave);
        }
        if (Poison)
        {
            Extension.BuffSpell(CurePoison);
        }
        if (Disease)
        {
            Extension.BuffSpell(CurePoison);
        }
        if (Extension.GetAttackingUnits(20).Count() > 1)
        {
            Extension.FightSpell(FeralSpirit);
        }

        if (Me.Level < 10)
        {
            if (lowlevel != true)
            {
                lowlevel = true;
            }

            if (MyTarget.GetDistance < 20)
            {
                TotemManager.CastTotems();
            }
            if (Me.Level < 4)
            {
                Extension.FightSpell(LightningBolt);
            }
            if (MyTarget.GetDistance > 7)
            {
                Extension.FightSpell(LightningBolt);
            }
            Extension.FightSpell(EarthShock);
            if (Me.ManaPercentage > 40)
            {
                Extension.BuffSpell(LightningShield);
            }
        }
        if (Me.Level > 9 && Me.Level < 20)
        {
            if (lowlevel != true)
            {
                lowlevel = true;
            }
            if (MyTarget.GetDistance < 20)
            {
                TotemManager.CastTotems();
            }
            //_lastTotemPosition.DistanceTo(ObjectManager.Me.Position)

            //if (_fireTotemPosition.DistanceTo(Me.Position) < 10 && Extension.GetAttackingUnits(5).Count() > 1 && ShamanLevelSettings.CurrentSetting.UseFireNova)
            //{
            //    Extension.FightSpell(FireNova);
            //}
            if (MyTarget.GetDistance > 7)
            {
                Extension.FightSpell(LightningBolt);
            }
            if (!FlameShock.KnownSpell)
            {
                Extension.FightSpell(EarthShock);
            }
            Extension.FightSpell(FlameShock);
            if (Me.ManaPercentage > 40)
            {
                Extension.BuffSpell(LightningShield);
            }
        }
        if (Me.Level > 19 && Me.Level < 40)
        {
            if (lowlevel == true)
            {
                lowlevel = false;
            }
            if (MyTarget.GetDistance < 20)
            {
                TotemManager.CotE();
                TotemManager.CastTotems();
            }
            if (MyTarget.GetDistance > 20)
            {
                Extension.FightSpell(LightningBolt);
            }
            Extension.FightSpell(EarthShock);
            if (Me.ManaPercentage < 20)
            {
                Extension.BuffSpell(WaterShield);
            }
            if (Me.ManaPercentage > 20 && !Me.HaveBuff(WaterShield.Id))
            {
                Extension.BuffSpell(LightningShield);
            }
        }
        if (Me.Level > 39 && Me.Level < 50)
        {
            if (MyTarget.GetDistance < 20)
            {
                TotemManager.CotE();
                TotemManager.CastTotems();
            }
            if (lowlevel == true)
            {
                lowlevel = false;
            }
            if (MyTarget.GetDistance > 20)
            {
                Extension.FightSpell(LightningBolt);
            }
            Extension.FightSpell(Stormstrike);
            Extension.FightSpell(EarthShock);
            Extension.FightSpell(LavaLash);
            if (Me.ManaPercentage < 20)
            {
                Extension.BuffSpell(WaterShield);
            }
            if (Me.ManaPercentage > 20 && !Me.HaveBuff(WaterShield.Id))

            {
                Extension.BuffSpell(LightningShield);
            }
        }
        if (Me.Level > 49 && Me.Level <= 80)
        {
            if (lowlevel == true)
            {
                lowlevel = false;
            }
            if (MyTarget.GetDistance < 20)
            {
                TotemManager.CotE();
                TotemManager.CastTotems();
            }
            if (MyTarget.GetDistance > 20 && ShamanLevelSettings.CurrentSetting.LNB)
            {
                Extension.FightSpell(LightningBolt);
            }
            if (Me.BuffStack(53817) >= 4)
            {
                if (Extension.GetAttackingUnits(5).Count() == 1)
                {
                    Extension.FightSpell(LightningBolt);
                }
                if (Extension.GetAttackingUnits(5).Count() > 1)
                {
                    Extension.FightSpell(ChainLightning);
                }
            }
            if (MyTarget.GetDistance < 7)
            {
                Extension.BuffSpell(ShamanisticRage);
            }
            Extension.FightSpell(Stormstrike);
            Extension.FightSpell(EarthShock);
            Extension.FightSpell(LavaLash);
            if (Me.ManaPercentage < 30)
            {
                Extension.BuffSpell(WaterShield);
            }
            if (Me.ManaPercentage > 30 && !Me.HaveBuff(WaterShield.Id))

            {
                Extension.BuffSpell(LightningShield);
            }
        }
    }


    private static void BuffRotation()
    {

        if (Me.ManaPercentage > 50 && ShamanLevelSettings.CurrentSetting.Ghostwolf && !Me.IsMounted)
        {
            Extension.BuffSpell(GhostWolf);
        }
        if (Me.ManaPercentage < 40)
        {
            Extension.BuffSpell(WaterShield);
        }
        if (Me.ManaPercentage > 90)
        {
            Extension.BuffSpell(LightningShield);
        }
        if (Me.HealthPercent < 40 && !Me.IsMounted)
        {
            Extension.HealSpell(HealingWave);
        }
        bool Poison = Extension.HasPoisonDebuff();
        bool Disease = Extension.HasDiseaseDebuff();
        if (Poison)
        {
            Extension.BuffSpell(CurePoison);
        }
        if (Disease)
        {
            Extension.BuffSpell(CurePoison);
        }
    }

    #region Pull
    private static void Pull()
    {
        if (ShamanLevelSettings.CurrentSetting.LNB)
        {
            Extension.FightSpell(LightningBolt);
        }

    }

    #endregion

    private static void EnchantWeapon()
    {
        bool hasMainHandEnchant = Lua.LuaDoString<bool>
            (@"local hasMainHandEnchant, _, _, _, _, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasMainHandEnchant) then 
               return '1'
            else
               return '0'
            end");

        bool hasOffHandEnchant = Lua.LuaDoString<bool>
            (@"local _, _, _, _, hasOffHandEnchant, _, _, _, _ = GetWeaponEnchantInfo()
            if (hasOffHandEnchant) then 
               return '1'
            else
               return '0'
            end");

        bool hasoffHandWeapon = Lua.LuaDoString<bool>(@"local hasWeapon = OffhandHasWeapon()
            return hasWeapon");

        if (!hasMainHandEnchant || (hasoffHandWeapon && !hasOffHandEnchant))
        {
            if (!WindfuryWeapon.KnownSpell && RockbiterWeapon.KnownSpell && !FlametongueWeapon.KnownSpell)
            {
                Extension.BuffSpell(RockbiterWeapon);
            }
            if (!WindfuryWeapon.KnownSpell && FlametongueWeapon.KnownSpell)
            {
                Extension.BuffSpell(FlametongueWeapon);
            }

            if (WindfuryWeapon.KnownSpell)
            {
                Extension.BuffSpell(WindfuryWeapon);
            }
        }
    }

}
