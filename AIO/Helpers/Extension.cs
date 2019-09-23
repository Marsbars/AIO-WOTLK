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


    public static bool FightSpell(Spell spell, bool focus = false, bool force = false, bool stopMoving = false, bool debuff = true)
    {
        if (spell.KnownSpell && spell.IsSpellUsable && spell.IsDistanceGood && ObjectManager.Me.HasTarget && ObjectManager.Target.IsAttackable && !TraceLine.TraceLineGo(ObjectManager.Me.Position, ObjectManager.Target.Position))
        {
            if (debuff)
            {
                if (ObjectManager.Target.HaveBuff(spell.Name))
                {
                    return false;
                }
            }
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
        if (spell.KnownSpell && spell.IsSpellUsable && !ObjectManager.Me.IsMounted && ObjectManager.Pet.IsValid && ObjectManager.Pet.IsAlive)
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

    //Old Version
    #region Check Interruptspell Casting
    public static bool InterruptSpell(Spell spell, bool CanBeMounted = false)
    {
        var resultLua = Lua.LuaDoString("ret = \"false\"; spell, rank, displayName, icon, startTime, endTime, isTradeSkill, ca﻿stID, interrupt = UnitCastingInfo(\"target\"); if interrupt then ret ﻿= \"true\" end", "ret");
        if (spell.KnownSpell
            && spell.IsSpellUsable
            && resultLua == "true")
        {
            if (ObjectManager.Me.IsMounted == CanBeMounted)
            {
                Frameunlock();
                spell.Launch();
                Usefuls.WaitIsCasting();
                return true;
            }
        }
        return false;
    }
    #endregion

    public static bool HasPoisonDebuff()
    {
        bool hasPoisonDebuff = Lua.LuaDoString<bool>
            (@"for i=1,25 do 
	            local _, _, _, _, d  = UnitDebuff('player',i);
	            if d == 'Poison' then
                return true
                end
            end");
        return hasPoisonDebuff;
    }

    public static bool HasDiseaseDebuff()
    {
        bool hasDiseaseDebuff = Lua.LuaDoString<bool>
            (@"for i=1,25 do 
	            local _, _, _, _, d  = UnitDebuff('player',i);
	            if d == 'Disease' then
                return true
                end
            end");
        return hasDiseaseDebuff;
    }
    //Gives Attackingunits which are Casting, interrupt after 30% Casttime

    public static WoWUnit InterruptableUnit(float distance)
    {
        return ObjectManager.GetWoWUnitAttackables(distance).Where(x =>
                                                                    x.InCombat &&
                                                                    x.HasTarget &&
                                                                    x.IsCast &&
                                                                    /*x.CanInterruptCasting &&*/
                                                                    (((x.CastingTimeLeft / 1000) / x.CastingSpell.CastTime) * 100) < 70 &&
                                                                    !TraceLine.TraceLineGo(ObjectManager.Me.Position, x.Position, CGWorldFrameHitFlags.HitTestSpellLoS)).OrderBy(x => x.GetDistance).FirstOrDefault();
    }

    //Gives Attackingunits in given Range
    public static IEnumerable<WoWUnit> GetAttackingUnits(int range)
    {
        return ObjectManager.GetUnitAttackPlayer().Where(u => u.Position.DistanceTo(ObjectManager.Target.Position) <= range);
    }
    //Gives a List of attacking Units
    public List<WoWUnit> AttackingUnits(WoWUnit from, float maxRange = 10)
    {
        return ObjectManager.GetWoWUnitHostile().Where(i =>
            from.Guid == i.TargetObject.Guid
            && from.Position.DistanceTo(i.Position) <= maxRange
        ).ToList();
    }
    //Unlocks Framelock
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
    //Locks Framelock
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
    //Get Item Amount
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
    //Get Item Amount
    public static int GetItemCount(string itemName)
    {
        string countLua =
            $@"
        local fullCount = 0;
        for bag=0,4 do
            for slot=1,36 do
                local itemLink = GetContainerItemLink(bag, slot);
                if (itemLink) then
                    local _,_, itemId = string.find(itemLink, 'item:(%d+):');
                    if (GetItemInfo(itemId) == ""{itemName}"") then
                        local texture, count = GetContainerItemInfo(bag, slot);
                        fullCount = fullCount + count;
                    end
                end
            end
        end
        return fullCount;";
        return Lua.LuaDoString<int>(countLua);
    }

    //deletes items until the given Amount
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

    //Checks if there are Players around me !equal my Faction
    private void PlayerCheck()
    {
        List<WoWPlayer> enemyPlayerList;

        if (ObjectManager.Me.IsAlliance)
            enemyPlayerList = ObjectManager.GetWoWUnitHorde();
        else
            enemyPlayerList = ObjectManager.GetWoWUnitAlliance();

        WoWPlayer nearestPlayerEnemy = ObjectManager.GetNearestWoWPlayer(enemyPlayerList);

    }
    // Determines if me is behind the Target
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

    // Uses the first item found in your bags that matches any element from the list
    public static void UseFirstMatchingItem(List<string> list)
    {
        List<WoWItem> _bagItems = Bag.GetBagItem();
        foreach (WoWItem item in _bagItems)
        {
            if (list.Contains(item.Name))
            {
                ItemsManager.UseItemByNameOrId(item.Name);
                Main.Log("Using " + item.Name);
                return;
            }
        }
    }

    // Checks if you have any of the listed items in your bags
    public static bool HaveOneInList(List<string> list)
    {
        List<WoWItem> _bagItems = Bag.GetBagItem();
        bool _haveItem = false;
        foreach (WoWItem item in _bagItems)
        {
            if (list.Contains(item.Name))
                _haveItem = true;
        }
        return _haveItem;
    }

    // Get item ID in bag from a list passed as argument (good to check CD)
    public static int GetItemID(List<string> list)
    {
        List<WoWItem> _bagItems = Bag.GetBagItem();
        foreach (WoWItem item in _bagItems)
            if (list.Contains(item.Name))
                return item.Entry;

        return 0;
    }

    // Get item ID in bag from a string passed as argument (good to check CD)
    public static int GetItemID(string itemName)
    {
        List<WoWItem> _bagItems = Bag.GetBagItem();
        foreach (WoWItem item in _bagItems)
            if (itemName.Equals(item))
                return item.Entry;

        return 0;
    }

    public static bool HaveRangedWeaponEquipped()
    {
        return ObjectManager.Me.GetEquipedItemBySlot(wManager.Wow.Enums.InventorySlot.INVSLOT_RANGED) != 0;
    }

}