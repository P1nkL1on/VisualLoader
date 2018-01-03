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
                whereName = "back " + much;

            BaseStats("Move " + whereName, (much > 0) ? new List<int>() { 2, 3, 4 } : new List<int>() { 1, 2, 3}, new List<int>() { }, 1, true, 100, 0, "Move "+whereName+" in a party.");
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
}
