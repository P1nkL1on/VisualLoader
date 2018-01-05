using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDlib.Units.Player
{
    public class Crusader : AbstractPlayer
    {
        public Crusader(int pos)
        {
            PlayerBaseStats(pos, "Crusader", "John", 6, 12, 33, 1, 10, 0, 5, 40, 30, 30, 40, 30, 
                new List<AbstractAbility>() { new CrusaderAttack(this), new CrusaderPapers(this), new CrusaderElbow(this), new CrusaderLance(this) }, 1);
        }
    }

    public class CrusaderAttack : AbstractPlayersAbility
    {
        int DMG_vs_unholy = 25;
        string tag = "unholy";
        public CrusaderAttack(AbstractPlayer host)
        {
            PlayerBaseStats(host, "Smite", new List<int>() { 1, 2 }, new List<int>() { 1, 2 }, 1, false, 85, 0, 0, "Hit bastards at first positions! +" + DMG_vs_unholy + "% DMG vs " + tag);
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(getDmg(Mod.Versus(unit, tag, DMG_vs_unholy, "% DMG")));
        }
    }

    public class CrusaderPapers : AbstractPlayersAbility
    {
        public CrusaderPapers(AbstractPlayer host)
        {
            PlayerBaseStats(host, "Accusation", new List<int>() { 1, 2 }, new List<int>() { 1 }, 2, false, 85, -40, 0, "Show enemies a paper in close, they will recieve some damage");
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(getDmg(0));
        }
    }

    public class CrusaderElbow : AbstractPlayersAbility
    {
        public CrusaderElbow(AbstractPlayer host)
        {
            PlayerBaseStats(host, "Stun blow", new List<int>() { 1, 2 }, new List<int>() { 1, 2 }, 1, false, 90, -75, 0, "Stun an enemy with glorios blow.");
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(getDmg(0));

            Stun s = new Stun(unit, 1, 90);
            s.TurnOn(host.MOD.apply_stun_chance, unit.MOD.ResistBleed);

            unit.buffs.Add(s);
        }
    }
    public class CrusaderLance : AbstractPlayersAbility
    {
        int DMG_vs_unholy = 15;
        string tag = "unholy";
        public CrusaderLance(AbstractPlayer host)
        {
            PlayerBaseStats(host, "Holy lance", new List<int>() { 3, 4 }, new List<int>() { 3, 4 }, 1, false, 85, 0, 5, "Hit bastards at last positions! +" + DMG_vs_unholy + "% DMG vs " + tag);
            moveHost = 1;
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(getDmg(Mod.Versus(unit, tag, DMG_vs_unholy, "% DMG")));
        }
    }
}
