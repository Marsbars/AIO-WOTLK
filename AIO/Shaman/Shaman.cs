﻿using robotManager.Helpful;
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

public static class Shaman
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
    //HealSpells
    private static Spell HealingWave = new Spell("Healing Wave");
    private static Spell LesserHealingWave = new Spell("Lesser Healing Wave");
    private static Spell CurePoison = new Spell("Cure Poison");
    private static Spell CureDisease = new Spell("Cure Disease");

    //BuffSpells
    private static Spell LightningShield = new Spell("Lightning Shield");
    private static Spell WaterShield = new Spell("Water Shield");
    private static Spell GhostWolf = new Spell("Ghost Wolf");
    private static Spell WindfuryWeapon = new Spell("Windfury Weapon");
    private static Spell RockbiterWeapon = new Spell("Rockbiter Weapon");


    public static void Initialize()
    {
        Shamansettings.Load();
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
        Shamansettings.Load();
        Shamansettings.CurrentSetting.ToForm();
        Shamansettings.CurrentSetting.Save();
    }

    internal static void Rotation()
    {
        while (_isLaunched)
        {
            try
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

                    if (Shamansettings.CurrentSetting.Framelock)
                    {
                        Extension.Framelock();
                    }
                    CombatRotation();
                    if (Shamansettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                }

            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(Usefuls.Latency);
        }

    }
    private static void CombatRotation()
    {
        bool Poison = Extension.HasPoisonDebuff();
        bool Disease = Extension.HasDiseaseDebuff();

        if (Me.HealthPercent < 20)
        {
            Extension.HealSpell(HealingWave);
        }
        if (Poison)
        {
            Extension.BuffSpell(CurePoison);
        }
        if (Disease)
        {
            Extension.BuffSpell(CureDisease);
        }
        if (Me.Level < 10)
        {
            lowlevel = true;
            Extension.FightSpell(LightningBolt);
            Extension.FightSpell(EarthShock);
            if (Me.ManaPercentage > 40)
            {
                Extension.BuffSpell(LightningShield);
            }
        }
        if (Me.Level > 9 && Me.Level < 20)
        {
            lowlevel = true;
            Extension.FightSpell(LightningBolt);
            Extension.FightSpell(EarthShock);
            if (Me.ManaPercentage > 40)
            {
                Extension.BuffSpell(LightningShield);
            }
        }
        if (Me.Level > 19 && Me.Level < 40)
        {
            lowlevel = false;
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
            lowlevel = false;
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
            lowlevel = false;
            if (MyTarget.GetDistance > 20)
            {
                Extension.FightSpell(LightningBolt);
            }
            if (Me.BuffStack(51532) >= 4)
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
            if (MyTarget.GetDistance < 6)
            {
                Extension.BuffSpell(ShamanisticRage);
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
    }


    private static void BuffRotation()
    {
        if (Me.ManaPercentage > 50 && Shamansettings.CurrentSetting.Ghostwolf && !Me.IsMounted)
        {
            Extension.BuffSpell(GhostWolf);
        }
        if (Me.ManaPercentage < 40)
        {
            Extension.BuffSpell(WaterShield);
        }

    }

    #region Pull
    private static void Pull()
    {
        Extension.FightSpell(LightningBolt);
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
            if (!WindfuryWeapon.KnownSpell && RockbiterWeapon.KnownSpell)
                Extension.BuffSpell(RockbiterWeapon);

            if (WindfuryWeapon.KnownSpell)
                Extension.BuffSpell(WindfuryWeapon);
        }
    }

}