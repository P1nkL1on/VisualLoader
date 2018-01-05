using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDlib
{
    public class Booka : AbstractUnit
    {
        public Booka(int pos)
        {
            BaseStats(pos, "Booka", 10, 4 + pos, 10, 0, 25, 5, new List<AbstractAbility>() { new BookaHit(), new BookaSwipe(), new BookaBomb(this), new BookaThrow(), new BookaHeal(), new Move(3, this), new Move(-3, this), new Pass() }, true);
            health += 10;
        }
    }
    public class Baka : AbstractUnit
    {
        public Baka(int pos)
        {
            BaseStats(pos, "Ugly", 10, 4 - pos, 10, 0, 25, 5, new List<AbstractAbility>() { new BookaHit(), new BookaThrow(), new BookaHeal(), new Pass()}, true );
            AddTags("unholy", "ugly", "test");
        }
    }

    class BookaHit : AbstractAbility
    {
        public BookaHit()
        {
            BaseStats("Weak attack", new List<int>() { 1, 2, 3 }, new List<int>() { 1,2,3}, 1, false, 50, 15, "Booka's basic attack to hurt enemies.");
            moveTarget = -1;
            moveHost = 1;
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(4);
            Thread.Sleep(1000);
        }
    }
    class BookaSwipe : AbstractAbility
    {
        public BookaSwipe()
        {
            BaseStats("Swipe attack", new List<int>() { 1,2,3,4 }, new List<int>() { 1 }, 4, false, 5, 15, "Booka's fuirous swipe to damage to three of enemies.");
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(2);
            Thread.Sleep(1000);
        }
    }
    class BookaThrow : AbstractAbility
    {
        public BookaThrow()
        {
            BaseStats("Stone throw", new List<int>() { 3,4}, new List<int>() { 3,4 }, 1, false, 40, 2, "Throw a piece of stone to far enemies.");
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(300);
            Thread.Sleep(1000);
        }
    }
    class BookaHeal : AbstractAbility
    {
        public BookaHeal()
        {
            BaseStats("Low healing", new List<int>(){2,3,4}, new List<int>(){1,2}, 3, true, 100, 10, "Heal all allies.");
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.HealFor(1);
            unit.buffs.Add(new BookaDef(unit));
            Thread.Sleep(1000);
        }
    }
    class BookaBomb : AbstractAbility
    {
        private AbstractUnit host;
        public BookaBomb(AbstractUnit host)
        {
            this.host = host;
            BaseStats("Poison bomb", new List<int>(){3,4}, new List<int>(){3}, 2, false, 60, 3, "Apply poison for far enemies.");
        }
        public override void UseAbility(AbstractUnit unit)
        {
            base.UseAbility(unit);
            unit.RecieveDamage(1);

            Poison p = new Poison(unit, 2, 3, 70);
            p.TurnOn(80, unit.MOD.ResistBleed);

            unit.buffs.Add(p);

            Thread.Sleep(1000);
        }
    }
}
