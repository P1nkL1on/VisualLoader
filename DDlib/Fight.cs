using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDlib
{
    public class Fight
    {
        List<AbstractUnit> left;
        List<AbstractUnit> right;

        int turnNumber = 0;

        public Fight(List<AbstractUnit> left, List<AbstractUnit> right)
        {
            this.left = left;
            this.right = right;
        }

        public bool Start()
        {
            while (left.Count != 0 && right.Count != 0)
            {
                Turn();
            }
            bool leftWins = left.Count != 0;
            if (leftWins)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n\nBrave heroes won a fight with enemies and archieve a fame and loot!");
                Thread.Sleep(5000);
                Console.WriteLine("\nYour loot:"); Console.ForegroundColor = ConsoleColor.Yellow; Thread.Sleep(1000);
                for (int i = 0; i < 24; i++)
                { Console.WriteLine("  some wonderfull shit;"); Console.Beep(900, 100); }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n\nBrave heroes fall in unfair duel with forces of evil!\nRest in peace, good guys...");
                Thread.Sleep(5000);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey();

            return leftWins;
        }

        public bool Turn()
        {
            List<int> idexes = new List<int>();
            List<int> speeds = new List<int>();
            for (int i = 0; i < left.Count; i++)
            {
                idexes.Add(i + 1);
                speeds.Add(left[i].Spd);
            }
            for (int i = 0; i < right.Count; i++)
            {
                idexes.Add(-i - 1);
                speeds.Add(right[i].Spd);
            }

            idexes = sortBySpd(idexes, speeds);

            for (int i = 0; i < idexes.Count; i++)
            {
                int pos = 0;
                AbstractUnit a = null;
                if (idexes[i] > 0)
                {
                    a = left[idexes[i] - 1];
                    pos = left[idexes[i] - 1].position;
                }
                if (idexes[i] < 0)
                {
                    a = right[-idexes[i] - 1];
                    pos = -right[-idexes[i] - 1].position;
                }

                Random rnd = new Random();
                AbstractUnit unit = null;

                bool isEnemy = !TraceFighters(pos, out unit, left, right);
                unit.TickAllBuffs();
                if (!unit.isDead())
                {
                    if (isEnemy)
                        unit.TryRandomlySelectAbility(rnd, right, left);
                    else
                        //unit.TryRandomlySelectAbility(rnd, left, right);
                        unit.TrySelectAbility(left, right);

                }
                List<int> deadIndexes = CheckForDeath();
                // remove dead characters from a queue
                for (int j = 0; j < deadIndexes.Count; j++)
                {
                    idexes.Remove(deadIndexes[j]);
                    for (int k = 0; k < idexes.Count; k++)
                    {
                        if (idexes[k] < deadIndexes[j] && deadIndexes[j] < 0) idexes[k]++;
                        if (idexes[k] > deadIndexes[j] && deadIndexes[j] > 0) idexes[k]--;
                    }
                    for (int z = 0; z < 5; z++)
                        Console.Beep(rnd.Next(100, 150), rnd.Next(50, 100));
                    AbstractUnit misc;
                    TraceFighters(pos, out misc, left, right);
                    Thread.Sleep(500);
                }
                if (left.Count == 0 || right.Count == 0)
                    return true;
            }

            return false;
        }

        List<int> CheckForDeath()
        {
            List<int> res = new List<int>();
            bool everDead = false;
            for (int i = 0; i < 2; i++)
            {
                List<AbstractUnit> chooseIn = (i == 0) ? left : right;
                for (int j = 0; j < chooseIn.Count; j++)
                    if (chooseIn[j].isDead())
                    {
                        if (chooseIn[j].leavesCorpse) {
                            Random rnd = new Random();
                            for (int z = 0; z < 5; z++)
                                Console.Beep(rnd.Next(100, 500), rnd.Next(50, 100));
                            chooseIn[j] = chooseIn[j].CreateCorpseOfThis();
                        }
                        else
                        {
                            chooseIn.RemoveAt(j); res.Add((j + 1) * ((1 - i) * 2 - 1)); j--;
                            if (!everDead) everDead = true;
                        }
                    }
            }
            return res;
        }

        List<int> sortBySpd(List<int> inds, List<int> spd)
        {
            for (int i = 0; i < inds.Count; i++)
            {
                int maxSpd = -1, maxInd = -1;
                for (int j = i; j < inds.Count; j++)
                    if (spd[j] > maxSpd)
                    { maxSpd = spd[j]; maxInd = j; }
                int temp = spd[i];
                spd[i] = spd[maxInd];
                spd[maxInd] = temp;

                temp = inds[i];
                inds[i] = inds[maxInd];
                inds[maxInd] = temp;
            }
            return inds;
        }

        public static bool TraceFighters(int position, out AbstractUnit unit, List<AbstractUnit> left, List<AbstractUnit> right)
        {
            return TraceFighters(position, out unit, left, right, 0);
        }

        public static bool TraceFighters(int position, out AbstractUnit unit, List<AbstractUnit> left, List<AbstractUnit> right, int far)
        {
            Console.Clear();
            int Ystart = 3,
                wid = 15,
                widHP = wid - 2;
            bool isAlly = false, isEnemy = false;
            AbstractUnit a = null, b = null;
            for (int i = 0; i < left.Count; i++)
            {
                isAlly = left[i].position == position;
                Console.ForegroundColor = ((far >= 0)? (left[i].position >= position && left[i].position <= position + far) : isAlly) ? ConsoleColor.DarkGreen : ConsoleColor.Gray;
                Console.SetCursorPosition((4 - left[i].position) * wid, Ystart);
                Console.Write(left[i].ToString());
                Console.SetCursorPosition((4 - left[i].position) * wid, Ystart + 1);
                Console.Write(left[i].HealthToString(widHP));
                Console.SetCursorPosition((4 - left[i].position) * wid, Ystart + 2);
                Console.Write(left[i].HealthToInt().PadLeft(widHP));
                Console.SetCursorPosition((4 - left[i].position) * wid, Ystart + 3);
                Console.Write(left[i].BuffsToString().PadLeft(widHP));

                if (isAlly)
                    a = left[i];
            }
            for (int i = 0; i < right.Count; i++)
            {
                isEnemy = right[i].position == -position;
                Console.ForegroundColor = ((far <= 0)? (right[i].position >= -position && right[i].position <= -position - far) : isEnemy) ? ConsoleColor.DarkRed : ConsoleColor.Gray;
                Console.SetCursorPosition((4 + right[i].position) * wid, Ystart);
                Console.Write(right[i].ToString());

                Console.SetCursorPosition((4 + right[i].position) * wid, Ystart + 1);
                Console.Write(right[i].HealthToString(widHP));

                Console.SetCursorPosition((4 + right[i].position) * wid, Ystart + 2);
                Console.Write(right[i].HealthToInt().PadLeft(widHP));

                Console.SetCursorPosition((4 + right[i].position) * wid, Ystart + 3);
                Console.Write(right[i].BuffsToString().PadLeft(widHP));

                if (isEnemy)
                    b = right[i];

            }
            Console.SetCursorPosition(wid * 9 / 2 - 2, Ystart + 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("VS");
            Console.SetCursorPosition(0, Ystart + 4);
            Console.ForegroundColor = ConsoleColor.Gray;

            if (a != null)
            {
                unit = a;
                return true;
            }

            unit = b;
            return false;
        }

    }
}
