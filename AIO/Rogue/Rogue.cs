using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager.Events;
using System.Collections.Generic;
using System.Linq;

public static class Rogue
{

    public static bool lowlevel;
    private static bool _isLaunched;
    public static WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public static WoWPlayer Me { get { return ObjectManager.Me; } }
    public static int MP;
    public static int OP;
    //Damage Spells
    public static Spell Garrotte = new Spell("Garrotte");
    public static Spell SinisterStrike = new Spell("Sinister Strike");
    public static Spell Riposte = new Spell("Riposte");
    public static Spell KillingSpree = new Spell("Killing Spree");
    public static Spell Rupture = new Spell("Rupture");
    public static Spell CheapShot = new Spell("Cheap Shot");
    public static Spell Envenom = new Spell("Envenom");
    public static Spell DeadlyThrow = new Spell("Deadly Throw");
    public static Spell FanofKnives = new Spell("Fan of Knives");
    //Buff Spells
    public static Spell SliceandDice = new Spell("Slice and Dice");
    public static Spell BladeFlurry = new Spell("Blade Flurry");
    public static Spell Sprint = new Spell("Sprint");
    public static Spell AdrenalineRush = new Spell("Adrenaline Rush");
    public static Spell Eviscerate = new Spell("Eviscerate");
    public static Spell TricksoftheTrade = new Spell("Tricks of the Trade");
    //interrupt
    public static Spell Kick = new Spell("Kick");
    public static Spell Blind = new Spell("Blind");

    //Usefuls
    public static Spell Sap = new Spell("Sap");
    public static Spell Dismantle = new Spell("Dismantle");
    public static Spell Vanish = new Spell("Vanish");
    public static Spell Distract = new Spell("Distract");
    public static Spell PickPocket = new Spell("Pick Pocket");
    public static Spell CloakofShadows = new Spell("Cloak of Shadows");

    public static void Initialize()
    {
        {
            _isLaunched = true;
            Roguesettings.Load();
            Logging.Write("Settings Loaded");
            Rotation();
        }
    }

    public static void Dispose() // When product stopped
    {
        {
            _isLaunched = false;
        }
    }

    public static void ShowConfiguration() // When use click on Fight class settings
    {
        Roguesettings.Load();
        Roguesettings.CurrentSetting.ToForm();
        Roguesettings.CurrentSetting.Save();
    }

    public static void Rotation()
    {
        Logging.Write("Rotation Loaded");
        while (_isLaunched)
        {
            try
            {

                if (!(Fight.InFight))
                {
                    PoisonWeapon();
                    Healing();
                    BuffRotation();
                    Pull();
                }
                else
                 if (Me.Target > 0)
                {
                    BuffRotation();
                    if (Roguesettings.CurrentSetting.Framelock)
                    {
                        Extension.Framelock();
                    }
                    CombatRotation();
                    if (Roguesettings.CurrentSetting.Framelock)
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

    public static void CombatRotation()
    {


    }

    private static void Pull()
    {
        if (Me.Level > 9 && Me.Level < 20)
        {


        }
    }

    private static void BuffRotation()
    {

    }

    private static void Healing()
    {


    }

    private static void PoisonWeapon()
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
        /* Poisons:

        Instant Poison 		Level 20	6947	Deadly Poison 		Level 30	2892
        Instant Poison II	Level 28	6949	Deadly Poison II	Level 38	2893
        Instant Poison III	Level 36	6950	Deadly Poison III	Level 46	8984
        Instant Poison IV	Level 44	8926	Deadly Poison IV	Level 54	8985
        Instant Poison V	Level 52	8927	Deadly Poison V		Level 60	20844
        Instant Poison VI	Level 60	8928	Deadly Poison VI	Level 62	22053
        Instant Poison VII	Level 68	21927	Deadly Poison VII	Level 70	22054
        Instant Poison VIII	Level 73	43230	Deadly Poison VIII	Level 76	43232
        Instant Poison IX	Level 79	43231	Deadly Poison IX	Level 80	43233

                if (!ObjectManager.Me.HaveBuff(Rogue.CurrentSetting.p2) && ItemsManager.GetItemCountByNameLUA(Rogue.CurrentSetting.p2) < 1 && !(Rogue.CurrentSetting.p2 == string.Empty))
                {
                    ItemsManager.UseItem(Rogue.CurrentSetting.p2);
                    Thread.Sleep(10);
                    Lua.LuaDoString("/click PickupInventoryItem(17)﻿");
                    Thread.Sleep(5000);
                    return;
                }

                    if(Me.Level >= 20 && Me.Level <28)
                    {
                        MP = 6947;				
                    } 
                    if(Me.Level >= 28 && Me.Level <36)
                    {
                        MP = 6949;				
                    } 
                    if(Me.Level >= 36 && Me.Level <44)
                    {
                        MP = 6950;				
                    } 			
                    if(Me.Level >= 44 && Me.Level <52)
                    {
                        MP = 8926;				
                    } 
                    if(Me.Level >= 52 && Me.Level <60)
                    {
                        MP = 8927;				
                    } 	
                    if(Me.Level >= 60 && Me.Level <68)
                    {
                        MP = 8928;				
                    } 
                    if(Me.Level >= 68 && Me.Level <73)
                    {
                        MP = 21927;				
                    } 
                    if(Me.Level >= 73 && Me.Level <79)
                    {
                        MP = 43230;				
                    } 
                    if(Me.Level >= 79)
                    {
                        MP = 43231;				
                    } 
                if (!hasMainHandEnchant)
                {
                    ItemsManager.UseItemById(MP);
                    Thread.Sleep(10);
                    Lua.LuaDoString("/click PickupInventoryItem(17)﻿");
                    Thread.Sleep(5000);
                    return;					
                }
                if (!hasoffHandEnchant && hasoffHandWeapon)
                {
                    if(Me.Level )
                }	
        */
        if (Extension.GetItemCount("Instant Poison") > 1 && !hasMainHandEnchant)
        {

        }
        if (Extension.GetItemCount("Deadly Poison") > 1 && hasoffHandWeapon && !hasOffHandEnchant)
        {

        }
    }
}
