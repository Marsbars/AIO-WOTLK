using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using robotManager.Helpful;
using robotManager.Products;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;
using System.Threading;

public class Main : ICustomClass
{
    private static string wowClass = ObjectManager.Me.WowClass.ToString();
    public static float settingRange = 5f;
    public static bool _isLaunched;
    private static bool _debug = true;

    public float Range
	{
		get
        {
            return settingRange;
        }
    }

    public void Initialize()
    {
        Log("Started V. 1.0.30. Discovering class and finding rotation...");
        var type = Type.GetType(ObjectManager.Me.Level < 80 ? ObjectManager.Me.WowClass + "Level" : ObjectManager.Me.WowClass + GetSpec());

        if (type != null)
        {
            _isLaunched = true;
            type.GetMethod("Initialize").Invoke(null, null);
        }
        else
        {
            LogError("Class not supported.");
            Products.ProductStop();
        }
    }

    public void Dispose()
    {
        var type = Type.GetType(ObjectManager.Me.Level < 80 ? ObjectManager.Me.WowClass + "Level" : ObjectManager.Me.WowClass + GetSpec());
        if (type != null)
            type.GetMethod("Dispose").Invoke(null, null);
        _isLaunched = false;
    }

    public void ShowConfiguration()
    {
        var type = Type.GetType(ObjectManager.Me.Level < 80 ? ObjectManager.Me.WowClass + "Level" : ObjectManager.Me.WowClass + GetSpec());

        if (type != null)
            type.GetMethod("ShowConfiguration").Invoke(null, null);
        else
            LogError("Class not supported.");
    }

    public static void LogFight(string message)
    {
        Logging.Write($"[WOTLK - {wowClass}]: { message}", Logging.LogType.Fight, Color.ForestGreen);
    }

    public static void LogError(string message)
    {
        Logging.Write($"[WOTLK - {wowClass}]: {message}", Logging.LogType.Error, Color.DarkRed);
    }

    public static void Log(string message)
    {
        Logging.Write($"[WOTLK - {wowClass}]: {message}");
    }

    public static void LogDebug(string message)
    {
        if (_debug)
            Logging.WriteDebug($"[WOTLK - {wowClass}]: { message}");
    }

    private string GetSpec()
    {
        var Talents = new Dictionary<string, int>();
        for (int i = 1; i <= 3; i++)
        {
            Talents.Add(
                Lua.LuaDoString<string>($"local name, iconTexture, pointsSpent = GetTalentTabInfo({i}); return name"),
                Lua.LuaDoString<int>($"local name, iconTexture, pointsSpent = GetTalentTabInfo({i}); return pointsSpent")
            );
        }
        var highestTalents = Talents.Max(x => x.Value);
        return Talents.Where(t => t.Value == highestTalents).FirstOrDefault().Key;
    }
}
