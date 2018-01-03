using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DDlib;

namespace DDsimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth * 6 / 10, Console.LargestWindowHeight / 2);
            Console.SetWindowPosition(0, 0);

            Fight f = new Fight(new List<AbstractUnit>() { new Booka(4), new Booka(3), new Booka(2), new Booka(1) },
                                new List<AbstractUnit>() { new Baka(1), new Baka(2), new Baka(3), new Baka(4) });

            f.Start();
        }
    }
}
