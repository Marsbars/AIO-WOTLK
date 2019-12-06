using robotManager.Helpful;
using System;
using System.Threading;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using wManager.Events;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;

public class PaladinRetribution
{
    public float Range { get { return 5.0f; } }
    private bool _isLaunched;
    public WoWUnit MyTarget { get { return ObjectManager.Target; } }
    public WoWPlayer Me { get { return ObjectManager.Me; } }

    //Damage Spells
    public Spell JudgementofLight = new Spell("Judgement of Light");
    public Spell JudgementofWisdom = new Spell("Judgement of Wisdom");
    public Spell HandofReckoning = new Spell("Hand of Reckoning");
    public Spell AvengersShield = new Spell("Avenger's Shield");
    public Spell HammerofWrath = new Spell("Hammer of Wrath");
    public Spell Exorcism = new Spell("Exorcism");
    public Spell Consecration = new Spell("Consecration");
    public Spell DivineStorm = new Spell("Divine Storm");
    public Spell CrusaderStrike = new Spell("Crusader Strike");
    public Spell ShieldofRighteousness = new Spell("Shield of Righteousness");
    public Spell HolyShield = new Spell("Holy Shield");
    public Spell HolyWrath = new Spell("Holy Wrath");
    //Buff Blessings
    public Spell BlessingofMight = new Spell("Blessing of Might");
    public Spell BlessingofKings = new Spell("Blessing of Kings");
    public Spell BlessingofWisdom = new Spell("Blessing of WisdHaom");
    public Spell BlessingofSanctuary = new Spell("Blessing of Sanctuary");
    //Buff Aura
    public Spell DevotionAura = new Spell("Devotion Aura");
    public Spell CrusaderAura = new Spell("Crusader Aura");
    public Spell ConcentrationAura = new Spell("Concentration Aura");
    //Buff Seals	
    public Spell SealofRighteousness = new Spell("Seal of Righteousness");
    public Spell SealofCommand = new Spell("Seal of Command");
    public Spell SealofWisdom = new Spell("Seal of Wisdom");
    public Spell SealofCorruption = new Spell("Seal of Corruption");
    //Buff General
    public Spell DivinePlea = new Spell("Divine Plea");
    public Spell AvengingWrath = new Spell("Avenging Wrath");
    public Spell DivineProtection = new Spell("Divine Protection");
    public Spell SacredShield = new Spell("Sacred Shield");
    public Spell RighteousFury = new Spell("Righteous Fury");
    //Healing Spells
    public Spell HolyLight = new Spell("Holy Light");
    public Spell FlashofLight = new Spell("Flash of Light");
    public Spell LayonHands = new Spell("Lay on Hands");
    public Spell HandofProtection = new Spell("Hand of Protection");
    //Buff Rest
    public Spell BloodCorruption = new Spell("Blood Corruption");
    public Spell TheartofWar = new Spell("The Art of War");
    //stun
    public Spell HammerofJustice = new Spell("Hammer of Justice");
    // dispell
    public Spell Purify = new Spell("Purify");

    public void Initialize()
    {
        _isLaunched = true;
        PaladinRetributionSettings.Load();
        Logging.Write("Paladin Level Dungeon Class...loading...");
        Rotation();
    }

    public void Dispose() // When product stopped
    {
        {
            _isLaunched = false;
        }
    }

    public void ShowConfiguration() // When use click on Fight class settings
    {
        PaladinRetributionSettings.Load();
        PaladinRetributionSettings.CurrentSetting.ToForm();
        PaladinRetributionSettings.CurrentSetting.Save();
    }

    public void Rotation()
    {
        Logging.Write("Rotation Loaded");
        while (_isLaunched)
        {
            try
            {
                if (!(Fight.InFight))
                {
                    BuffRotation();
                }
                else
                 if (Me.Target > 0)
                {
                    if (PaladinRetributionSettings.CurrentSetting.Framelock)
                    {
                        Extension.Framelock();
                    }
                    CombatRotation();
                    if (PaladinRetributionSettings.CurrentSetting.Framelock)
                    {
                        Extension.Frameunlock();
                    }
                    Healing();
                }

            }
            catch (Exception e)
            {
                Logging.Write("error" + e);
            }
            Thread.Sleep(Usefuls.Latency);
        }
    }

    public void CombatRotation()
    {

        Extension.FightSpell(HammerofWrath, false);
        Extension.FightSpell(JudgementofWisdom, false);
        Extension.FightSpell(CrusaderStrike, false);
        Extension.FightSpell(DivineStorm, false);
        Extension.FightSpell(Consecration, false);
        if (Me.HaveBuff(TheartofWar.Id))
        {
            Extension.FightSpell(Exorcism, false);
        }
        Extension.FightSpell(HolyWrath, false);
    }

    private void BuffRotation()
    {
        if (Me.IsMounted)
        {
            Extension.BuffSpell(CrusaderAura, true);
        }
        if (PaladinRetributionSettings.CurrentSetting.Autobuffing)
        {
            Extension.BuffSpell(BlessingofMight, false);
            Extension.BuffSpell(DevotionAura, false);
            Extension.BuffSpell(SealofRighteousness, false);
        }
        if (Me.ManaPercentage < 70 && !Fight.InFight)
        {
            Extension.BuffSpell(DivinePlea, false);
        }
    }

    private void Healing()
    {
        if (PaladinRetributionSettings.CurrentSetting.Purify)
        {
            Purifying();
        }
        if (Me.HaveBuff(TheartofWar.Id) && Me.HealthPercent <= 75)
        {
            Extension.HealSpell(FlashofLight, false, true);
        }

        if (Me.HealthPercent <= 30)
        {
            Extension.HealSpell(HolyLight, false, true);
        }
        if (Me.HealthPercent <= 40)
        {
            Extension.HealSpell(FlashofLight, false, true);
        }

    }


    #region Purify
    private void Purifying()
    {
        var members = getPartymembers().Where(o => !TraceLine.TraceLineGo(o.Position)).OrderBy(o => o.HealthPercent);
        if (members.Count() > 0)
        {
            var u = members.First();
            WoWPlayer healTarget = new WoWPlayer(u.GetBaseAddress);
            if (!TraceLine.TraceLineGo(healTarget.Position) && healTarget.IsAlive)
            {
                //Interact.InteractGameObject(healTarget.GetBaseAddress, false);
                if (Lua.LuaDoString<bool>("for j=1,40 do local m=5; local d={UnitDebuff(\"{healTarget}\",j)}; if (d[5]==\"Poison\" or d[5]==\"Disease\") and d[7]>m then j=41 return 1 end end;"))
                {
                    ObjectManager.Me.FocusGuid = healTarget.Guid;
                    Extension.HealSpell(Purify,false,false, true);
                    Logging.WriteDebug("Purified " + healTarget.Name);
                    return;
                }
                return;
            }
        }
        return;
    }
    #endregion


    #region get party members
    List<WoWPlayer> getPartymembers()
    {
        List<WoWPlayer> ret = new List<WoWPlayer>();
        var u = Party.GetPartyHomeAndInstance().Where(p => p.GetDistance < 80 && p.IsValid && !TraceLine.TraceLineGo(p.Position));

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

}
