using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDlib
{
    class Pass : AbstractAbility
    {
        public Pass()
        {
            BaseStats("Pass", new List<int>() { 1,2, 3, 4 }, new List<int>() { }, 1, true, 100, 0, "Pass a turn.");
        }
        public override void UseAbility(AbstractUnit units)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n\nPassed a turn");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Beep(120, 500);
            Thread.Sleep(500);
        }
    }

    class Move : AbstractAbility
    {
        public int much;
        AbstractUnit host;
        public Move(int much, AbstractUnit host)
        {
            this.host = host;
            if (much == 0) throw new Exception("fuck no!");
            string whereName = "";
            this.much = much;
            if (much > 0)
                whereName = "forward " + much;
            else
                whereName = "back " + Math.Abs(much);

            BaseStats("Move " + whereName, (much > 0) ? new List<int>() { 2, 3, 4 } : new List<int>() { 1, 2, 3}, new List<int>() { }, 1, true, 100, 0, "Move "+whereName+" in a party.");
            moveHost = much;
        }

        public override void UseAbility(AbstractUnit unit)
        {
            host.MoveFor = much;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n\nMoved forward!");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Beep(500, 500);
            Thread.Sleep(500);
        }
    }

    public class Poison : AbstractBuff
    {
        int turnDamage;
        public Poison(AbstractUnit who, int turnDamage, int turnLength, int baseApplyChange)
        {
            this.turnDamage = turnDamage;
            BaseStats("Poisoned", String.Format("This unit takes {0} dmg by poisoned each turn.", turnDamage),
                baseApplyChange, 'p', turnLength, who);
        }
        public override void TickEffect()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(target.ToString() + " damaged from poison!");
            target.RecieveDamage(turnDamage);
        }
        public override void Apply()
        {
            base.Apply();
            target.AddTags("poisoned");
        }
        public override void Remove()
        {
            base.Remove();
            target.AddTags("poisoned");
        }
    }

    public class Stun : AbstractBuff
    {

        public Stun(AbstractUnit who, int duration, int baseApplyChance)
        {

            BaseStats("Stunned", "Unit is stunned and will skil it next turn.", baseApplyChance, '*', duration, who);
        }
        public override void TickEffect()
        {
            turnsLeft++;
        }
        public override void Apply()
        {
            base.Apply();
            target.AddTags("stunned");
        }
        public override void Remove()
        {
            base.Remove();
            target.AddTags("stunned");
        }
    }

    public class StunResist : AbstractBuff
    {
        public StunResist(AbstractUnit who, int duration)
        {
            BaseStats("Stun resist", "Has more resist to stun!", 100, '^', 1, who); 
        }
        public override void Apply()
        {
            base.Apply();
            target.MOD.STUN_resist_mod += 40;
        }
        public override void Remove()
        {
            base.Remove();
            target.MOD.STUN_resist_mod -= 40;
        }
    }
}
