using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();

            Console.SetWindowSize(Console.LargestWindowWidth * 7 / 8, Console.LargestWindowHeight * 7 / 8);
            Console.SetWindowPosition(0, 0);
            Console.ReadKey();
           
            Namegen.makeTree(3000).StartLoading(new Random());
            Console.ReadKey();
        }
    }
}
