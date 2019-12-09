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


    class Partystuff
    {


        public static int maxhealrange;


    #region get party
        public static List<WoWPlayer> getPartymembers()
        {
            List<WoWPlayer> ret = new List<WoWPlayer>();
            var u = Party.GetPartyHomeAndInstance().Where(p => p.GetDistance < maxhealrange && p.IsValid && !TraceLine.TraceLineGo(p.Position));

            if (u.Count() > 0)
            {
                foreach (var unit in u)
                {
                    WoWPlayer p = new WoWPlayer(unit.GetBaseAddress);
                    ret.Add(p);
                }
            }
            WoWPlayer v = new WoWPlayer(ObjectManager.Me.GetBaseAddress);
            ret.Add(v);
            return ret;
        }
    #endregion
    #region get all Party Members
    public static List<WoWPlayer> getAllPartymembers()
        {
            List<WoWPlayer> ret = new List<WoWPlayer>();

            var u = Party.GetPartyHomeAndInstance().Where(p => p.GetDistance < 80 && p.IsValid);

            if (u.Count() > 0)
            {
                foreach (var unit in u)
                {
                    WoWPlayer p = new WoWPlayer(unit.GetBaseAddress);
                    ret.Add(p);
                }
            }
            WoWPlayer v = new WoWPlayer(ObjectManager.Me.GetBaseAddress);
            ret.Add(v);
            return ret;
        }
    #endregion
    #region get Party Targets
    public static List<WoWUnit> GetPartyTargets()
        {
            List<WoWPlayer> party = Party.GetPartyHomeAndInstance();
            List<WoWPlayer> partyMembers = new List<WoWPlayer>();
            var ret = new List<WoWUnit>();
            partyMembers.AddRange(party.Where(p => p.GetDistance < 40 && p.IsValid && p.HealthPercent > 0));
            WoWPlayer Me = new WoWPlayer(ObjectManager.Me.GetBaseAddress);
            partyMembers.Add(Me);
            foreach (var m in partyMembers)
            {
                var targetUnit = new WoWUnit(ObjectManager.GetObjectByGuid(m.Target).GetBaseAddress);
                if (m.IsValid && (m.HealthPercent > 0) && (m.InCombat || targetUnit.InCombat) && m.Target.IsNotZero())
                {
                    if (ret.All(u => u.Guid != m.Target)) // prevent double list entrys
                    {
                        if (targetUnit.IsValid && targetUnit.IsAlive)
                        {
                            ret.Add(targetUnit);
                        }
                    }
                }
            }
            return ret;
        }
    #endregion
    #region get tanks
    public static List<WoWPlayer> getTanks()
        {
            List<WoWPlayer> ret = new List<WoWPlayer>();
            var u = Party.GetPartyHomeAndInstance().Where(p => p.GetDistance < 80 && p.IsValid && !TraceLine.TraceLineGo(p.Position));

            if (u.Count() > 0)
            {
                foreach (var unit in u)
                {
                    //Logging.WriteDebug("Unit name: " + unit.Name.ToString().Trim());
                    if (IsTank(unit.Name.ToString()))
                    {
                        WoWPlayer p = new WoWPlayer(unit.GetBaseAddress);
                        ret.Add(p);
                    }
                }
            }
            /*          if (ret.Count() == 0)
                            {
                                Logging.WriteDebug("Could not find a tank!");
                                WoWPlayer v = new WoWPlayer(ObjectManager.Me.GetBaseAddress);
                                ret.Add(v);
                            }
            */
            return ret;
        }
    public static string GetTankPlayerName()
        {
            var lua = new[]
                  {
                          "partyTank = \"\";",
                          "for groupindex = 1,MAX_PARTY_MEMBERS do",
                          "	if (UnitInParty(\"party\" .. groupindex)) then",
                          "		local role = UnitGroupRolesAssigned(\"party\" .. groupindex);",
                          "		if role == \"TANK\" then",
                          "			local name, realm = UnitName(\"party\" .. groupindex);",
                          "			partyTank = name;",
                          "			return;",
                          "		end",
                          "	end",
                          "end",
                      };
            return Lua.LuaDoString(lua, "partyTank");
        }
        public static bool IsTank(string unit)
        {
            var tankNaam = GetTankPlayerName();
            WoWPlayer v = new WoWPlayer(ObjectManager.Me.GetBaseAddress);
            if (tankNaam.Contains(unit))
            {
                return true;
            }
            return false;
        }
        #endregion

    #region getHealers
        public static List<WoWPlayer> getHealers()
        {
            List<WoWPlayer> ret = new List<WoWPlayer>();
            var u = Party.GetPartyHomeAndInstance().Where(p => p.GetDistance < 80 && p.IsValid && !TraceLine.TraceLineGo(p.Position));

            if (u.Count() > 0)
            {
                foreach (var unit in u)
                {
                    //Logging.WriteDebug("Healer name: " + unit.Name.ToString().Trim());
                    if (IsHealer(unit.Name.ToString()))
                    {
                        WoWPlayer p = new WoWPlayer(unit.GetBaseAddress);
                        ret.Add(p);
                    }
                }
            }
            return ret;
        }
        public static string GetHealerName()
        {
            var lua = new[]
                  {
                          "partyHealer = \"\";",
                          "for groupindex = 1,MAX_PARTY_MEMBERS do",
                          "	if (UnitInParty(\"party\" .. groupindex)) then",
                          "		local role = UnitGroupRolesAssigned(\"party\" .. groupindex);",
                          "		if role == \"HEALER\" then",
                          "			local name, realm = UnitName(\"party\" .. groupindex);",
                          "			partyHeaer = name;",
                          "			return;",
                          "		end",
                          "	end",
                          "end",
                      };

            return Lua.LuaDoString(lua, "partyHealer");
        }
        public static bool IsHealer(string unit)
        {
            var healerNaam = GetHealerName();
            if (healerNaam.Contains(unit))
            {
                return true;
            }
            return false;
        }
        #endregion

    }

