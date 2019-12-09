using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;

public class Talents
{
    private static bool _isAssigning = false;
    private static bool _isInitialized = false;
    public static bool _isRunning = false;
    public static string[] _talentsCodes = new string[] { };
    private static int _talentTimer = 60000 * 5; // 5 minutes

    // Talent initialization
    public static void InitTalents(bool assignTalents, bool useDefaultTalents, string[] customTalentsCodes)
    {
        if (assignTalents)
        {
            if (useDefaultTalents)
            {
                SetTalentCodes();
                Main.Log("Your are using the following default talents build:");
            }
            else
            {
                SetTalentCodes(customTalentsCodes);
                Main.Log("Your are using the following custom talents build:");
            }

            if (_talentsCodes.Count() > 0)
                Main.Log(_talentsCodes.Last());
            else
                Main.LogError("No talent code");

            _isInitialized = true;
        }
    }

    // Set the default talents codes to use
    public static void SetTalentCodes()
    {
        switch (Main.wowClass)
        {
            // FURY WARRIOR
            case "Warrior":
                _talentsCodes = new string[]
                {
                    "0000000000000000000000000000000305020030000000000000000000000000000000000000000000000",
                    "0000000000000000000000000000000305022030500010000000000000000000000000000000000000000",
                    "0000000000000000000000000000000305022030500310000000000000000000000000000000000000000",
                    "0000000000000000000000000000000305022030501310050120000000000000000000000000000000000",
                    "0000000000000000000000000000000305022030501310052120500300000000000000000000000000000",
                    "0000000000000000000000000000000305022030501310053120500300000000000000000000000000000",
                    "0000000000000000000000000000000305022030502310053120500351000000000000000000000000000",
                    "0000000000000000000000000000000305022030504310053120500351000000000000000000000000000",
                    "3500200023300000000000000000000305022030504310053120500351000000000000000000000000000"
                };
                break;

            // BLOOD DEATHKNIGHT
            case "DeathKnight":
                _talentsCodes = new string[]
                {
                    "000000000000000000000000505000500501000000000000000000000000000000",
                    "000000000000000000000000505000540501005010000000000000000000000000",
                    "000000000000000000000000505000540501005310000000000000000000000000",
                    "000000000000000000000000505000550501005310510000000000000000000000",
                    "323200113020000000000002505000551501005310510000000000000000000000"
                };
                break;

            // AFFLICTION WARLOCK
            case "Warlock":
                _talentsCodes = new string[]
                {
                    "235022200122351005350033115100000000000000000000000000000000000000000000000000000",
                    "235022200122351005350033115120122300112000000000000000000000000000000000000000000",
                    "235022200122351005350033115120122300112000000000000000000000000000000000000000000"
                };
                break;

            // BM HUNTER
            case "Hunter":
                _talentsCodes = new string[]
                {
                    "052031115251120431015310510000000000000000000000000000000000000000000000000000000",
                    "052031115251120431015310510150052002300000000000000000000000000000000000000000000",
                    "052031115251120431015310510150052002300000000000000000000000000000000000000000000"
                };
                break;

            // COMBAT ROGUE
            case "Rogue":
                _talentsCodes = new string[]
                {
                    "00000000000000000000000000002320300000000000000000000000000000000000000000000000000",
                    "00000000000000000000000000002520500000000000000000000000000000000000000000000000000",
                    "00000000000000000000000000002520510000320152231000000000000000000000000000000000000",
                    "00000000000000000000000000002520510000330152231005212510000000000000000000000000000",
                    "00000000000000000000000000002520510000350152231005212510000000000000000000000000000",
                    "00000000000000000000000000002520510000350152231005212515000000000000000000000000000",
                    "00532000300000000000000000002520510000350152231005212515000000000000000000000000000"

                };
                break;

            // ENHANCEMENT SHAMAN
            case "Shaman":
                _talentsCodes = new string[]
                {
                    "00000000000000000000000003020500010000000000000000000000000000000000000000000000",
                    "00000000000000000000000003020500310000000000000000000000000000000000000000000000",
                    "00000000000000000000000003020501310500130000000000000000000000000000000000000000",
                    "00000000000000000000000003020502310500130300000000000000000000000000000000000000",
                    "00000000000000000000000003020502310500132300100000000000000000000000000000000000",
                    "00000000000000000000000003020502310500132303110120000000000000000000000000000000",
                    "00000000000000000000000003020502310500132303112120100000000000000000000000000000",
                    "00000000000000000000000003020502310500132303112123100000000000000000000000000000",
                    "00000000000000000000000003020502310500133303112123105100000000000000000000000000",
                    "00000000000000000000000003020503310500133303113123105100000000000000000000000000",
                    "05003000000000000000000003020503310500133303113123105100000000000000000000000000",
                    "05003000000000000000000003020503310500133303113123105100000000000000000000000000",
                    "05203015000000000000000003020503310502133303113123105100000000000000000000000000"
                };
                break;

            // FERAL DRUID
            case "Druid":
                _talentsCodes = new string[]
                {
                    "0000000000000000000000000000500200000000000000000000000000000000000000000000000000000",
                    "0000000000000000000000000000503200030000000000000000000000000000000000000000000000000",
                    "0000000000000000000000000000503202030302000000000000000000000000000000000000000000000",
                    "0000000000000000000000000000503202030322000000000000000000000000000000000000000000000",
                    "0000000000000000000000000000503202032322010050120000000000000000000000000000000000000",
                    "0000000000000000000000000000503202032322010052120030000000000000000000000000000000000",
                    "0000000000000000000000000000503202032322010053120030000000000000000000000000000000000",
                    "0000000000000000000000000000503202032322011053120030010000000000000000000000000000000",
                    "0000000000000000000000000000503202032322011053120030311501000000000000000000000000000",
                    "0000000000000000000000000000503202032322011053120030313511000000000000000000000000000",
                    "0000000000000000000000000000503202032322011053120030313511203500010000000000000000000",
                    "0000000000000000000000000000503202032322011053120030313511203503012000000000000000000",
                    "0000000000000000000000000000503202032322012053120030313511203503012000000000000000000"
                };
                break;

            // FROST MAGE
            case "Mage":
                _talentsCodes = new string[]
                {
                    "00000000000000000000000000000000000000000000000000000000000503020010000000000000000000",
                    "00000000000000000000000000000000000000000000000000000000000503020310000000000000000000",
                    "00000000000000000000000000000000000000000000000000000000000503030310000000000000000000",
                    "00000000000000000000000000000000000000000000000000000000000503030310003000000000000000",
                    "00000000000000000000000000000000000000000000000000000000002503030310003100000000000000",
                    "00000000000000000000000000000000000000000000000000000000003503030310203110030000000000",
                    "00000000000000000000000000000000000000000000000000000000003503030310203110230142200000",
                    "00000000000000000000000000000000000000000000000000000000003503030310203110230152201000",
                    "00000000000000000000000000000000000000000000000000000000003503030310203130230152201000",
                    "00000000000000000000000000000000000000000000000000000000003523030310203130230152201051",
                    "23000503200003000000000000000000000000000000000000000000003523030310203130230152201051",
                    "23000503310003000000000000000000000000000000000000000000003523030310203130230152201051"
                };
                break;

            // RETRIBUTION PALADIN
            case "Paladin":
                _talentsCodes = new string[]
                {
                    "000000000000000000000000000000000000000000000000000005230041003231000000000000",
                    "000000000000000000000000000000000000000000000000000005230051003231300000000000",
                    "000000000000000000000000000000000000000000000000000005230051203231301133201300",
                    "000000000000000000000000000000000000000000000000000005230051203231302133201330",
                    "000000000000000000000000000000000000000000000000000005230051203231302133221331",
                    "000000000000000000000000000500000000000000000000000005230051203231302133221331",
                    "000000000000000000000000000500000000000000000000000005232251223331322133231331",
                    "050000000000000000000000000500000000000000000000000005232251223331322133231331"
                };
                break;

            // SHADOW PRIEST
            case "Priest":
                _talentsCodes = new string[]
                {
                    "0000000000000000000000000000000000000000000000000000000305021051023012023152301351",
                    "0503203100000000000000000000000000000000000000000000000325021051023012023152301351",
                    "0503203100000000000000000000000000000000000000000000000325121051023012323152301351"
                };
                break;

            default:
                break;
        }
    }

    // Set the custom talents codes to use
    public static void SetTalentCodes(string[] talentsCodes)
    {
        _talentsCodes = talentsCodes;
    }

    // Talent pulse
    public static void DoTalentPulse(object sender, DoWorkEventArgs args)
    {
        _isRunning = true;
        while (Main._isLaunched && _isRunning)
        {
            Thread.Sleep(3000);
            try
            {
                if (Conditions.InGameAndConnectedAndProductStartedNotInPause /*&& !ObjectManager.Me.InCombatFlagOnly */
                    && ObjectManager.Me.IsAlive && Main._isLaunched && !_isAssigning && _isInitialized && _isRunning)
                {
                    Main.LogDebug("Assigning Talents");
                    _isAssigning = true;
                    AssignTalents(_talentsCodes);
                    _isAssigning = false;
                }
            }
            catch (Exception arg)
            {
                Logging.WriteError(string.Concat(arg), true);
            }
            Thread.Sleep(_talentTimer);
        }
        _isRunning = false;
    }

    // Talent assignation 
    public static void AssignTalents(string[] TalentCodes)
    {
        // Number of talents in each tree
        List<int> NumTalentsInTrees = new List<int>()
        {
            Lua.LuaDoString<int>("return GetNumTalents(1)"),
            Lua.LuaDoString<int>("return GetNumTalents(2)"),
            Lua.LuaDoString<int>("return GetNumTalents(3)")
        };

        if (!_isInitialized)
        {
            Thread.Sleep(500);
        }
        else if (TalentCodes.Count() <= 0)
        {
            Main.LogError("No talent code");
        }
        else if (Lua.LuaDoString<int>("local unspentTalentPoints, _ = UnitCharacterPoints('player'); return unspentTalentPoints;") <= 0)
        {
            Main.LogDebug("No talent point to spend");
        }
        else
        {
            bool _stop = false;

            // Loop for each TalentCode in list
            foreach (string talentsCode in TalentCodes)
            {
                if (_stop)
                    break;

                // check if talent code length is correct
                if ((NumTalentsInTrees[0] + NumTalentsInTrees[1] + NumTalentsInTrees[2]) != talentsCode.Length)
                {
                    Main.LogError("WARNING: Your talents code length is incorrect. Please use " +
                        "http://armory.twinstar.cz/talent-calc.php to generate valid codes.");
                    Main.LogError("Talents code : " + talentsCode);
                    _stop = true;
                    break;
                }

                // TalentCode per tree
                List<string> TalentCodeTrees = new List<string>()
                {
                    talentsCode.Substring(0, NumTalentsInTrees[0]),
                    talentsCode.Substring(NumTalentsInTrees[0], NumTalentsInTrees[1]),
                    talentsCode.Substring(NumTalentsInTrees[0] + NumTalentsInTrees[1], NumTalentsInTrees[2])
                };

                // loop in 3 trees
                for (int k = 1; k <= 3; k++)
                {
                    if (_stop)
                        break;

                    // loop for each talent
                    for (int i = 0; i < NumTalentsInTrees[k - 1]; i++)
                    {
                        if (_stop)
                            break;

                        int _talentNumber = i + 1;
                        string _talentName = Lua.LuaDoString<string>("local name, _, _, _, _, _, _, _ = GetTalentInfo(" + k + ", " + _talentNumber + "); return name;");
                        int _currentRank = Lua.LuaDoString<int>("_, _, _, _, currentRank, _, _, _ = GetTalentInfo(" + k + ", " + _talentNumber + "); return currentRank;");
                        int _realMaxRank = Lua.LuaDoString<int>("_, _, _, _, _, maxRank, _, _ = GetTalentInfo(" + k + ", " + _talentNumber + "); return maxRank;");

                        int _pointsToAssignInTalent = Convert.ToInt16(TalentCodeTrees[k - 1].Substring(i, 1));

                        if (_currentRank > _pointsToAssignInTalent && TalentCodes.Last().Equals(talentsCode))
                        {
                            Main.LogError("WARNING: Your assigned talent points don't match your talent code. Please reset your talents or review your talents code." +
                                " You have " + _currentRank + " point(s) in " + _talentName + " where you should have " + _pointsToAssignInTalent + " point(s)");
                            Main.LogError("Talents code : " + talentsCode);
                            _stop = true;
                        }
                        else if (_pointsToAssignInTalent > _realMaxRank)
                        {
                            Main.LogError($"WARNING : You're trying to assign {_pointsToAssignInTalent} points into {_talentName}," +
                                $" maximum is {_realMaxRank} points for this talent. Please check your talent code.");
                            Main.LogError("Talents code : " + talentsCode);
                            _stop = true;
                        }
                        else if (_currentRank != _pointsToAssignInTalent)
                        {
                            // loop for individual talent rank
                            for (int j = 0; j < _pointsToAssignInTalent - _currentRank; j++)
                            {
                                if (!Main._isLaunched)
                                    _stop = true;
                                if (_stop)
                                    break;
                                Lua.LuaDoString("LearnTalent(" + k + ", " + _talentNumber + ")");
                                Thread.Sleep(500 + Usefuls.Latency);
                                int _newRank = Lua.LuaDoString<int>("_, _, _, _, currentRank, _, _, _ = GetTalentInfo(" + k + ", " + _talentNumber + "); return currentRank;");
                              //Main.Log("Assigned talent: " + _talentName + " : " + _newRank + "/" + _pointsToAssignInTalent, Color.SteelBlue);
                                if (Lua.LuaDoString<int>("local unspentTalentPoints, _ = UnitCharacterPoints('player'); return unspentTalentPoints;") <= 0)
                                    _stop = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
