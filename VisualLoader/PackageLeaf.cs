using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VisualLoader
{
    public class Edge
    {
        public PackageLeaf to;
        float speed_slow;
        public ErrorType err;
        public Edge(PackageLeaf to, float speed_slow)
        {
            this.to = to;
            this.speed_slow = speed_slow;
            err = ErrorType.none;
        }
        public Edge(PackageLeaf to, float speed_slow, ErrorType err)
        {
            this.to = to;
            this.speed_slow = speed_slow;
            this.err = err;
        }
        public int StealBytes(int speed, Random rnd)
        {
            switch (err)
            {
                case ErrorType.unknown:
                    Console.WriteLine("\n");
                    for (int i = 0; i < Console.LargestWindowWidth * 10; i++)
                        Console.Write((char)(rnd.Next(255)));

                    return -1;
                    break;
                default:
                    break;
            }
            return to.StealBytes(speed);
        }
    }
    public enum ErrorType
    {
        none = -1,
        disconnect = 0,
        unknown = 1,
    }
    public class PackageLeaf
    {
        private int byteCount;
        private string name;
        public PackageLeaf pater;
        public List<Edge> files;
        public bool visited;

        public void MakeUser()
        {
            name = "userroot";
        }
        public PackageLeaf(string nam, int byt)
        {
            pater = null;
            byteCount = byt;
            name = nam;
            files = new List<Edge>();
            visited = false;
        }
        public string Company
        {
            get { return name.Remove(name.IndexOf("//")); }
        }
        public void Info()
        {
            //Namegen.Stat(name, byteCount);
        }
        public string GetName
        {
            get { return name; }
        }
        public int GetBytes
        {
            get { return byteCount; }
        }
        public int StealBytes(int speed)
        {
            byteCount = Math.Max(0, byteCount - speed);
            return speed;
        }
    }
    public class Tree
    {
        public PackageLeaf userroot;
        public Tree()
        {
            userroot = new PackageLeaf("user", 0);
            path = new List<string>();
            status = new List<string>();
        }
        public Tree(PackageLeaf root)
        {
            userroot = root;
            userroot.MakeUser();
            path = new List<string>();
            status = new List<string>();
        }
        int deep = 0;
        int loaded = 0;
        List<string> path;
        List<string> status;
        public void StartLoading(Random rnd)
        {

            Console.Clear();
            Console.WriteLine("Initialising root;");
            Loader.MakeLoader();
            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(rnd.Next(2, 10));
                Loader.ChangeLoader(i / 10, 100);
                i += rnd.Next(4);
            }
            Loader.ClearLoader();
            Console.WriteLine("\n\nRoot initialised;\nPress any key to start downloading ALL the shit...");

            deep = 0;
            Recursive(userroot.files[0], rnd);
            Console.Clear();
            Console.WriteLine("\tOperation finished!");
            Namegen.Stat("Deep:", deep);
            Namegen.Stat("Loaded packages:", loaded);
        }

        static int MaxVisiblePath = Console.LargestWindowHeight * 2 / 3;
        void Recursive(Edge where, Random rnd)
        {
            Console.Clear();
            deep++;
            path.Add(where.to.GetName);
            status.Add("processing");
            Namegen.Stat("\nDeep:", deep);
            Namegen.Stat("Loaded packages:", loaded);
            Console.WriteLine("\n");
            if (path.Count > MaxVisiblePath)
                Console.WriteLine("...");
            for (int i = Math.Max(0, path.Count - MaxVisiblePath); i < path.Count; i++)
                Namegen.Stat("-> " + path[i], status[i]);
            where.to.Info();


            Loader.MakeLoader();
            int bytesLoaded = 0, bytesNeed = where.to.GetBytes;
            while (where.to.GetBytes > 0)
            {
                Thread.Sleep(10);//(rnd.Next(2 * deep, 10 * deep) / 10 + 5);
                Loader.ChangeLoader(bytesLoaded, bytesNeed);
                int geted = where.StealBytes(10000, rnd);
                if (geted >= 0)
                    bytesLoaded += geted;
                else
                {
                    Namegen.Stat("\n\n\nError:", where.err.ToString());
                    Thread.Sleep(1500);
                    return;
                }
            }
            Loader.ClearLoader();
            Console.WriteLine("\n\n" + where.to.GetName + " loaded!");
            loaded++;
            status[status.Count - 1] = "loaded";

            Thread.Sleep(2);
            for (int i = 0; i < where.to.files.Count; i++)
                if (where.to.files[i].to.GetBytes > 0)
                    Recursive(where.to.files[i], rnd);
            path.RemoveAt(path.Count - 1);
            status.RemoveAt(status.Count - 1);
            deep--;
            Thread.Sleep(50);
        }
    }
    public class Namegen
    {
        static string[] companyes = new string[] { "RedComp", "Apple", "Vehicle", "BulletBret", "Napor98", "Intel", "Stella", "Perspective", "Origin", "Stole", "Misc", "City", "Transaction" };
        static string[] themes = new string[] { "IT", "Buisness", "Device", "System", "Database", "Data", "Bigdata", "Modelling", "Sculpt", "Render", "Site", "UML", "Info", "FBI", "Linkage", "Detroit", "Omega", "Alpha", "Beta" };
        static string[] adj = new string[] { "Visual", "Modern", "New", "Common", "Main", "Important", "Add", "Unit", "Windows", "IOS", "Android", "Place", "WEB", "Internet", "Client", "Backend", "Frontend" };
        static string[] what = new string[] { "Context", "Holder", "Model", "Proprty", "Program", "Package", "Library", "Unit", "Test", "Class", "Lib", "Repository", "Service", "Controller" };
        static string nums = "1234567890";

        public static string getName(Random rnd)
        {
            return
                companyes[rnd.Next(companyes.Length)] + "//" +
                themes[rnd.Next(themes.Length)] + adj[rnd.Next(adj.Length)] + what[rnd.Next(what.Length)] + ((rnd.Next(2) == 0) ? nums[rnd.Next(nums.Length)] + "" : "") + ((rnd.Next(2) == 0) ? nums[rnd.Next(nums.Length)] + "" : "") + ((rnd.Next(2) == 0) ? nums[rnd.Next(nums.Length)] + "" : "");
        }
        static int WentPacks = 0;
        static int Count = 0;
        public static Tree makeTree(int count)
        {
            Random rnd = new Random();

            Console.WriteLine("Activated net generation;\nWait, please...");
            List<string> names = new List<string>();
            List<PackageLeaf> packs = new List<PackageLeaf>();
            int failed = 0;
            for (int i = 0; i < count; i++)
            {
                string newname =
                    getName(rnd);
                if (names.IndexOf(newname) < 0)
                {
                    names.Add(newname);
                    packs.Add(new PackageLeaf(newname, rnd.Next((int)Math.Pow(10.0, (double)rnd.Next(2, 8)))));
                }
                else
                    failed++;
                if (packs.Count % (count / 10) == 0 && i > 0)
                {
                    Console.WriteLine(String.Format("Created {0} packages, {1} packages were broken;", packs.Count, failed));
                    failed = 0;
                }
            }
            Count = packs.Count;
            //Console.WriteLine(String.Format("\nIndependent packages:\t{0};", Count));
            Stat("\nIndependent packages:", Count);
            Console.WriteLine();

            int connections = 0,
                step = count * 2 / 10;
            for (int i = 0; i < Count; i++)
                for (int j = 0; j < Count; j++)
                    if (i != j && ((rnd.Next(Count * 4 / 3) < 25 && packs[i].Company == packs[j].Company) || rnd.Next(Count * 5) < 1))
                    {
                        packs[i].files.Add(new Edge(packs[j], rnd.Next(1000, 1500) / 1000.0f, (rnd.Next(1001) < 40) ? ErrorType.unknown : ErrorType.none));
                        connections++;
                        if (connections % (step) == 0 && i + j > 0)
                            Console.WriteLine(String.Format("Created {0} connections", connections));
                    }
            Stat("\nConnection count:", connections);

            int Attempts = 0, AttemptsMax = 10, bestInd = -1, bestCount = 0;
            float normal = .95f;
            Console.WriteLine();
            Loader.MakeLoader();
            do
            {
                WentPacks = 0;
                for (int i = 0; i < packs.Count; i++)
                    packs[i].visited = false;

                RecursiveGo(packs[Attempts * (Count / (AttemptsMax + 5))]);

                //Console.WriteLine(String.Format("\n\nAttempt {0} : start in package number {1}", Attempts, Attempts * (Count / (AttemptsMax + 5))));
                //packs[Attempts * (Count / (AttemptsMax + 5))].Info();
                //Stat("Packages pinged in this web:", WentPacks);
                if (WentPacks > bestCount)
                { bestCount = WentPacks; bestInd = Attempts * (Count / (AttemptsMax + 5)); }
                Attempts++;
            }
            while (Attempts < AttemptsMax && WentPacks < Count * normal);
            Loader.ClearLoader();

            Stat("\n\nMost optimised package's index:", bestInd);
            Stat("Count pingable packages from it", bestCount);

            Console.WriteLine("Tree created. Press any key to continue...");
            Console.ReadKey();

            return new Tree(packs[bestInd]);
        }
        public static void Stat(string a, int what)
        {
            Stat(a, what + ";");
        }
        public static void Stat(string a, string b)
        {
            Console.WriteLine(a.PadRight(50) + "\t" + b.PadLeft(20));
        }


        static void RecursiveGo(PackageLeaf pack)
        {
            if (pack.visited)
                return;
            pack.Info();
            pack.visited = true;
            WentPacks++;
            Loader.ChangeLoader(WentPacks, Count);
            for (int i = 0; i < pack.files.Count; i++)
                RecursiveGo(pack.files[i].to);
            //
        }
    }

    public class Loader
    {
        static int indexStart = ("loading % [").Length;
        static int indexFinish = ("loading % [░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░] ").Length;

        static int prevValue = int.MaxValue;
        static string Bar;
        public static void MakeLoader()
        {
            Console.WriteLine();
            Bar = "loading % [░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░] 0/0";
            Console.Write(Bar);
        }
        public static void ChangeLoader(int much, int from)
        {
            //if (much % 5 != 0)
            //    return;
            if (prevValue > much)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(Bar);
                prevValue = much;
            }

            //Thread.Sleep(100);
            int nowIn = (int)Math.Round(much * 1.0 / from * 60.0),
                wasIn = (int)Math.Round(prevValue * 1.0 / from * 60.0);
            prevValue = much;
            Console.SetCursorPosition(indexStart + wasIn, Console.CursorTop);
            for (int i = wasIn; i < nowIn; i++)
                Console.Write("█");

            Console.SetCursorPosition(indexFinish, Console.CursorTop);
            Console.Write(much + "/" + from + "\t\t");
        }

        public static void ClearLoader()
        {

            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < 8; i++)
            {
                Console.Write(("").PadRight(Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
            Console.SetCursorPosition(0, Console.CursorTop - 2);
        }
    }

}
