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

class TotemManager
{
    private WoWLocalPlayer Me = ObjectManager.Me;
    private static Vector3 _lastTotemPosition = null;

    private static Spell StoneclawTotem = new Spell("Stoneclaw Totem");
    private static Spell StrengthOfEarthTotem = new Spell("Strength of Earth Totem");
    private static Spell StoneskinTotem = new Spell("Stoneskin Totem");
    private static Spell SearingTotem = new Spell("Searing Totem");
    internal static Spell TotemicCall = new Spell("Totemic Recall");
    private static Spell ManaSpringTotem = new Spell("Mana Spring Totem");
    private static Spell MagmaTotem = new Spell("Magma Totem");
    private static Spell WrathOfAirTotem = new Spell("Wrath of Air Totem");
    private static Spell EarthElementalTotem = new Spell("Earth Elemental Totem");
    private static Spell CalloftheElements = new Spell("Call of the Elements");



    internal static bool CastTotems()
    {
        if (CastWaterTotem())
            return true;
        if (CastEarthTotem())
            return true;
        if (CastFireTotem())
            return true;
        if (CastAirTotem())
            return true;
        return false;
    }

    internal static void CotE()
    {

        bool haveEarthTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(2); return totemName;").Contains("Totem");
        bool haveFireTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(1); return totemName;").Contains("Totem");
        bool haveWindTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(4); return totemName;").Contains("Totem");
        bool haveWaterTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(3); return totemName;").Contains("Totem");
        bool haveTotem = haveEarthTotem || haveFireTotem || haveWaterTotem || haveWindTotem;

        if (CalloftheElements.KnownSpell && !haveTotem)
        {
            Cast(CalloftheElements);
        }
    }
    internal static void CheckForTotemicCall()
    {
        if (Shamansettings.CurrentSetting.UseTotemicCall)
        {
            //IEnumerable<WoWUnit> units =  ObjectManager.GetObjectWoWUnit().Where(u => u.UnitFlags == wManager.Wow.Enums.UnitFlags.Totem);

            bool haveEarthTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(2); return totemName;").Contains("Totem");
            bool haveFireTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(1); return totemName;").Contains("Totem");
            bool haveWindTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(4); return totemName;").Contains("Totem");
            bool haveWaterTotem = Lua.LuaDoString<string>(@"local _, totemName, _, _ = GetTotemInfo(3); return totemName;").Contains("Totem");
            bool haveTotem = haveEarthTotem || haveFireTotem || haveWaterTotem || haveWindTotem;

            if (_lastTotemPosition != null && haveTotem && !ObjectManager.Me.IsMounted && _lastTotemPosition.DistanceTo(ObjectManager.Me.Position) > 17
                && !ObjectManager.Me.IsCast && TotemicCall.KnownSpell)
                Cast(TotemicCall);
        }
    }

    internal static bool CastEarthTotem()
    {
        if (Shamansettings.CurrentSetting.UseEarthTotems)
        {
            string currentEarthTotem = Lua.LuaDoString<string>
                (@"local haveTotem, totemName, startTime, duration = GetTotemInfo(2); return totemName;");

            // Earth Elemental Totem on multiaggro
            if (ObjectManager.GetNumberAttackPlayer() > 1 && EarthElementalTotem.KnownSpell
                && !currentEarthTotem.Contains("Stoneclaw Totem") && !currentEarthTotem.Contains("Earth Elemental Totem"))
            {
                {
                    if (Cast(EarthElementalTotem))
                        return true;
                }
            }

            // Stoneclaw on multiaggro
            if (ObjectManager.GetNumberAttackPlayer() > 1 && StoneclawTotem.KnownSpell
                && !currentEarthTotem.Contains("Stoneclaw Totem") && !currentEarthTotem.Contains("Earth Elemental Totem"))
            {
                {
                    if (Cast(StoneclawTotem))
                        return true;
                }
            }

            // Strenght of Earth totem
            if (!Shamansettings.CurrentSetting.UseStoneSkinTotem && !ObjectManager.Me.HaveBuff("Strength of Earth")
                && !currentEarthTotem.Contains("Stoneclaw Totem") && !currentEarthTotem.Contains("Earth Elemental Totem"))
            {
                {
                    if (Cast(StrengthOfEarthTotem))
                        return true;
                }
            }

            // Stoneskin Totem
            if (Shamansettings.CurrentSetting.UseStoneSkinTotem && !ObjectManager.Me.HaveBuff("Stoneskin")
                && !currentEarthTotem.Contains("Stoneclaw Totem") && !currentEarthTotem.Contains("Earth Elemental Totem"))
            {
                {
                    if (Cast(StoneskinTotem))
                        return true;
                }
            }
        }
        return false;
    }

    internal static bool CastFireTotem()
    {
        if (Shamansettings.CurrentSetting.UseFireTotems)
        {
            string currentFireTotem = Lua.LuaDoString<string>
                (@"local haveTotem, totemName, startTime, duration = GetTotemInfo(1); return totemName;");

            // Magma Totem
            if (ObjectManager.GetNumberAttackPlayer() > 1 && ObjectManager.Me.ManaPercentage > 50 && ObjectManager.Target.GetDistance < 10
                && !currentFireTotem.Contains("Magma Totem") && Shamansettings.CurrentSetting.UseMagmaTotem)
            {
                if (Cast(MagmaTotem))
                    return true;
            }

            // Searing Totem
            if ((!currentFireTotem.Contains("Searing Totem") || Shaman._fireTotemPosition == null || ObjectManager.Me.Position.DistanceTo(Shaman._fireTotemPosition) > 15f)
                && ObjectManager.Target.GetDistance < 10 && !currentFireTotem.Contains("Magma Totem"))
            {
                if (Cast(SearingTotem))
                {
                    Shaman._fireTotemPosition = ObjectManager.Me.Position;
                    return true;
                }
            }
        }
        return false;
    }

    internal static bool CastAirTotem()
    {
        if (Shamansettings.CurrentSetting.UseAirTotems)
        {
            string currentAirTotem = Lua.LuaDoString<string>
                (@"local _, totemName, _, _ = GetTotemInfo(4); return totemName;");

            // Mana Spring Totem
            if (!ObjectManager.Me.HaveBuff("Wrath of Air"))
            {
                if (Cast(WrathOfAirTotem))
                    return true;
            }
        }
        return false;
    }

    internal static bool CastWaterTotem()
    {
        if (Shamansettings.CurrentSetting.UseWaterTotems)
        {
            string currentWaterTotem = Lua.LuaDoString<string>
                (@"local _, totemName, _, _ = GetTotemInfo(3); return totemName;");

            // Mana Spring Totem
            if (!ObjectManager.Me.HaveBuff("Mana Spring"))
            {
                if (Cast(ManaSpringTotem))
                    return true;
            }
        }
        return false;
    }

    internal static bool Cast(Spell s)
    {
        Main.LogDebug("Into Totem Cast() for " + s.Name);

        if (!s.IsSpellUsable || !s.KnownSpell || ObjectManager.Me.IsCast)
            return false;

        s.Launch();

        if (s.Name.Contains(" Totem"))
            _lastTotemPosition = ObjectManager.Me.Position;

        if (s.Name.Contains(" Elements"))
            _lastTotemPosition = ObjectManager.Me.Position;

        return true;
    }
}
