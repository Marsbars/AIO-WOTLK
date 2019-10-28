using System.Collections.Generic;
using wManager.Wow.Helpers;

public static class Consumables
{
    // Healthstones list
    public static List<string> HealthStones()
    {
        return new List<string>
        {
            "Minor Healthstone",
            "Lesser Healthstone",
            "Healthstone",
            "Greater Healthstone",
            "Major Healthstone",
            "Fel Healthstone"
        };
    }

    // Checks if we have a Healthstone
    public static bool HaveHealthstone()
    {
        if (Extension.HaveOneInList(HealthStones()))
            return true;
        return false;
    }

    // Use Healthstone
    public static void UseHealthstone()
    {
        Extension.UseFirstMatchingItem(HealthStones());
    }

    // Soulstones list
    public static List<string> SoulStones()
    {
        return new List<string>
        {
            "Minor Soulstone",
            "Lesser Soulstone",
            "Soulstone",
            "Major Soulstone",
            "Greater Soulstone",
            "Master Soulstone"
        };
    }

    // Checks if we have a Soulstone
    public static bool HaveSoulstone()
    {
        if (Extension.HaveOneInList(SoulStones()))
            return true;
        return false;
    }

    // Use Soulstone
    public static void UseSoulstone()
    {
        Extension.UseFirstMatchingItem(SoulStones());
    }

}