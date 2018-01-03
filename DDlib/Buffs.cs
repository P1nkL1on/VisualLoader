using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDlib
{
    public class Poison : AbstractBuff
    {
        int turnDamage;
        public Poison(AbstractUnit who, int turnDamage, int turnLength)
        {
            this.turnDamage = turnDamage;
            BaseStats("Poisoned", String.Format("This unit takes {0} dmg by poisoned each turn.", turnDamage), 'p', turnLength, who);
        }
        public override void TickEffect()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(target.ToString() + " damaged from poison!");
            target.RecieveDamage(turnDamage);
        }
    }
}
