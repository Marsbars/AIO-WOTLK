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

class Extension
{


    public static bool FightSpell(Spell spell, bool focus = false, bool force = false, bool stopMoving = false)
    {
        if (spell.KnownSpell && spell.IsSpellUsable && spell.IsDistanceGood && ObjectManager.Me.HasTarget && ObjectManager.Target.IsAttackable && !ObjectManager.Target.HaveBuff(spell.Name))
        {
            if (force)
            {
                Lua.LuaDoString("SpellStopCasting();");
            }
            if (stopMoving)
            {
                MovementManager.StopMove();
            }
            if (focus)
            {
                Lua.LuaDoString($"CastSpellByID({ spell.Id}, \"focus\")");
                Usefuls.WaitIsCasting();
                Logging.Write("Casting Fightspell @ focus: " + spell);
                return true;
            }
            else
            {
                spell.Launch();
                Usefuls.WaitIsCasting();
                Logging.Write("Casting Fightspell: " + spell);
                return true;
            }
        }
        return false;
    }


    public static bool BuffSpell(Spell spell, bool CanBeMounted = false, bool force = false)
    {
        if (spell.KnownSpell && spell.IsSpellUsable && !ObjectManager.Me.HaveBuff(spell.Name))
        {
            if (force)
            {
                Lua.LuaDoString("SpellStopCasting();");
            }
            if (ObjectManager.Me.IsMounted == CanBeMounted)
            {
                spell.Launch();
                Usefuls.WaitIsCasting();
                Logging.Write("Casting Buffspell: " + spell);
                return true;
            }
        }
        return false;
    }

    public static bool PetSpell(Spell spell)
    {
        if (spell.KnownSpell && spell.IsSpellUsable && !ObjectManager.Me.IsMounted && !ObjectManager.Pet.IsValid && !ObjectManager.Pet.IsAlive)
        {
            spell.Launch();
            Usefuls.WaitIsCasting();
            Logging.Write("Casting Petspell: " + spell);
            return true;
        }
        return false;
    }

    public static bool HealSpell(Spell spell, bool CanBeMounted = false, bool force = false, bool focus = false)
    {
        if (spell.KnownSpell && spell.IsSpellUsable && !ObjectManager.Me.HaveBuff(spell.Name))
        {
            if (force)
            {
                Lua.LuaDoString("SpellStopCasting();");
            }
            if (ObjectManager.Me.IsMounted == CanBeMounted)
            {
                if (focus)
                {
                    Lua.LuaDoString($"CastSpellByID({ spell.Id}, \"focus\")");
                    Usefuls.WaitIsCasting();
                    Logging.Write("Casting Healspell @ focus: " + spell);
                    return true;
                }
                else
                {
                    spell.Launch();
                    Usefuls.WaitIsCasting();
                    Logging.Write("Casting Healspell: " + spell);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool GroupHealSpell(Spell spell, WoWUnit target, int buffTimeLeft = 0, int stacks = 0, Spel﻿l debuff = null, bool owner = true)
    {
        bool hasDebuff;
        if (debuff != null)
        {
            hasDebuff = Extension.HasBuff(debuff, target, buffTimeLeft, stacks, owner);
        }
        else
        {
            hasDebuff = Extension.HasBuff(spell, target, buffTimeLeft, stacks, owner);
        }

        // Validate spell
        if (!ObjectManager.Me.IsStunned && !ObjectManager.Me.IsDead && !ObjectManager.Me.IsCast && !target.IsDead && spell.KnownSpell && spell.IsSpellUsable && spell.IsDistanceGood && !hasDebuff)
        {
            if (target.Guid == ObjectManager.Me.Guid)
            {
                // Cast on self
                Lua.LuaDoString($"CastSpellByID({spell.Id}, \"player\")");
                Usefuls.WaitIsCasting();
            }
            else
            {
                // Cast on target
                Lua.LuaDoString($"CastSpellByID({spell.Id}, \"{target.Name}\")");
                Usefuls.WaitIsCasting();
            }

            // Log
            Logging.WriteDebug($"Cast: {spell.NameInGame}");

            // Return
            return true;
        }

        // Return
        return false;
    }

    public static bool HasBuff(Spell spell, WoWUnit target, double buffTimeLeft = 0, int stacks = 0, bool owner = true)
    {
        // Get target auras
        List<Aura> auraList = target.GetAllBuff();

        // Get aura
        Aura aura = null;
        if (owner)
        {
            // Set
            aura = auraList.Where(s => s.ToString().Contains(spell.Name) && s.Owner == ObjectManager.Me.Guid).FirstOrDefault();
        }
        else
        {
            // Set
            aura = auraList.FirstOrDefault(s => s.ToString().Contains(spell.Name));
        }

        // Any found?
        if (aura != null)
        {
            // Validate
            if (aura.TimeLeftSeconde > buffTimeLeft && aura.Stack >= stacks)
            {
                // Return
                return true;
            }
        }

        // Return
        return false;
    }

    public static IEnumerable<WoWUnit> GetAttackingUnits(int range)
    {
        return ObjectManager.GetUnitAttackPlayer().Where(u => u.Position.DistanceTo(ObjectManager.Target.Position) <= range);
    }

    public List<WoWUnit> AttackingUnits(WoWUnit from, float maxRange = 10)
    {
        return ObjectManager.GetWoWUnitHostile().Where(i =>
            from.Guid == i.TargetObject.Guid
            && from.Position.DistanceTo(i.Position) <= maxRange
        ).ToList();
    }

    public static void Frameunlock()
    {
        if (Memory.WowMemory.FrameIsLocked && Hook.AllowFrameLock)
        {
            wManagerSetting.CurrentSetting.UseLuaToMove = false;
            Thread.Sleep(10);
            Memory.WowMemory.UnlockFrame(true);
            Thread.Sleep(10);
        }
    }

    public static void Framelock()
    {
        if (!Memory.WowMemory.FrameIsLocked && Hook.AllowFrameLock)
        {
            wManagerSetting.CurrentSetting.UseLuaToMove = true;
            Thread.Sleep(10);
            Memory.WowMemory.LockFrame();
            Thread.Sleep(10);
        }
    }

    public static int GetItemQuantity(string itemName)
    {
        var execute =
            "local itemCount = 0; " +
            "for b=0,4 do " +
                "if GetBagName(b) then " +
                    "for s=1, GetContainerNumSlots(b) do " +
                        "local itemLink = GetContainerItemLink(b, s) " +
                        "if itemLink then " +
                            "local _, stackCount = GetContainerItemInfo(b, s)\t " +
                            "if string.find(itemLink, \"" + itemName + "\") then " +
                                "itemCount = itemCount + stackCount; " +
                            "end " +
                       "end " +
                    "end " +
                "end " +
            "end; " +
            "return itemCount; ";
        return Lua.LuaDoString<int>(execute);
    }



    public static void DeleteItems(string itemName, int leaveAmount)
    {
        var itemQuantity = GetItemQuantity(itemName) - leaveAmount;
        if (string.IsNullOrWhiteSpace(itemName) || itemQuantity <= 0)
            return;
        var execute =
            "local itemCount = " + itemQuantity + "; " +
            "local deleted = 0; " +
            "for b=0,4 do " +
                "if GetBagName(b) then " +
                    "for s=1, GetContainerNumSlots(b) do " +
                        "local itemLink = GetContainerItemLink(b, s) " +
                        "if itemLink then " +
                            "local _, stackCount = GetContainerItemInfo(b, s)\t " +
                            "local leftItems = itemCount - deleted; " +
                            "if string.find(itemLink, \"" + itemName + "\") and leftItems > 0 then " +
                                "if stackCount <= 1 then " +
                                    "PickupContainerItem(b, s); " +
                                    "DeleteCursorItem(); " +
                                    "deleted = deleted + 1; " +
                                "else " +
                                    "if (leftItems > stackCount) then " +
                                        "SplitContainerItem(b, s, stackCount); " +
                                        "DeleteCursorItem(); " +
                                        "deleted = deleted + stackCount; " +
                                    "else " +
                                        "SplitContainerItem(b, s, leftItems); " +
                                        "DeleteCursorItem(); " +
                                        "deleted = deleted + leftItems; " +
                                    "end " +
                                "end " +
                            "end " +
                        "end " +
                    "end " +
                "end " +
            "end; ";
        Lua.LuaDoString(execute);
    }

    public static int GetSpecialization()
    {
        KeyValuePair<int, int> highestPointTree = new KeyValuePair<int, int>(0, 0);

        // Process talent trees
        for (int i = 1; i <= 3; i++)
        {
            // Get current talent points
            int treePointValue = Lua.LuaDoString<int>($"local _, _, talentPoints = GetTalentTabInfo({i}); return talentPoints;");

            // Bigger than old value?
            if (treePointValue > highestPointTree.Value)
            {
                // Set new value
                highestPointTree = new KeyValuePair<int, int>(i, treePointValue);
            }
        }

        // Return
        return highestPointTree.Key;
    }


    private void PlayerCheck()
    {
        List<WoWPlayer> enemyPlayerList;

        if (ObjectManager.Me.IsAlliance)
            enemyPlayerList = ObjectManager.GetWoWUnitHorde();
        else
            enemyPlayerList = ObjectManager.GetWoWUnitAlliance();

        WoWPlayer nearestPlayerEnemy = ObjectManager.GetNearestWoWPlayer(enemyPlayerList);

    }

    public static bool meBehindTarget()
    {
        var target = ObjectManager.Target;

        float Pi = (float)System.Math.PI;
        bool backLeft = false;
        bool backRight = false;
        float target_x = ObjectManager.Target.Position.X;
        float target_y = ObjectManager.Target.Position.Y;
        float target_r = ObjectManager.Target.Rotation;
        float player_x = ObjectManager.Me.Position.X;
        float player_y = ObjectManager.Me.Position.Y;
        float d = (float)System.Math.Atan2((target_y - player_y), (target_x - player_x));
        float r = d - target_r;

        if (r < 0) r = r + (Pi * 2);
        if (r > 1.5 * Pi) backLeft = true;
        if (r < 0.5 * Pi) backRight = true;
        if (backLeft || backRight) return true; else return false;
    }

}