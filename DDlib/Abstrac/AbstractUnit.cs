using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDlib
{
    public abstract class AbstractAbility
    {
        protected List<int> canBeUsedFrom;
        public List<int> canTarget;
        protected int targets;
        public bool multyTarget { get { return targets > 1; } }
        public bool castOnAllies;
        public string skillName;
        protected string skillDescr;
        protected int acc;
        protected int crit;
        public int moveHost;
        public int moveTarget;

        public override string ToString()
        {
            //            01234567890123456
            string res = "o o o o ";   // ⊙◯◦○ ⚫
            for (int i = 0; i < canBeUsedFrom.Count; i++)
                res = res.Insert(6 - (canBeUsedFrom[i] - 1) * 2, "☺").Remove(7 - (canBeUsedFrom[i] - 1) * 2, 1);
            //if (!castOnAllies)
            {
                res += "> * * * *";
                if (multyTarget)
                {
                    for (int i = 0; i < targets; i++)
                        res = res.Insert(9 + (canTarget[0] + i - 1) * 2, "-☻").Remove(11 + (canTarget[0] + i - 1) * 2, 2);
                    int pos = res.IndexOf('-');
                    if (pos > 7)
                        res = res.Insert(pos, " ").Remove(pos + 1, 1);
                }
                else
                {
                    for (int i = 0; i < canTarget.Count; i++)
                        res = res.Insert(10 + (canTarget[i] - 1) * 2, "☻").Remove(11 + (canTarget[i] - 1) * 2, 1);
                }
                if (castOnAllies)
                {
                    string other = res.Substring(10).Replace('☻', '☺');
                    res = res.Substring(0, 10);
                    for (int i = other.Length - 1; i >= 0; i--)
                        res += other[i];
                }
            }
            return String.Format(" [{0}]\t{1}\n\n", res, DESCR());
        }

        protected virtual string DESCR()
        {
            return String.Format("ACC: {0}  CRT: {1}\n   {2} : {3}", acc, crit, skillName, skillDescr);
        }

        public bool CanBeUsedFrom(int pos)
        {
            return canBeUsedFrom.IndexOf(pos) >= 0;
        }

        public bool CanBeUsedOnAnyOf(List<AbstractUnit> units)
        {
            if (canTarget.Count == 0)
                return true;
            for (int i = 0; i < units.Count; i++)
                if (CanBeUsedTo(units[i].position))
                    return true;
            return false;
        }

        public bool CanBeUsedTo(int pos)
        {
            return canTarget.IndexOf(pos) >= 0;
        }

        protected void BaseStats(string name, List<int> from, List<int> to, int targets, bool toallies, int acc, int crit, string descr)
        {
            this.skillName = name;
            this.skillDescr = descr;
            this.canBeUsedFrom = from;
            this.canTarget = to;
            this.targets = targets;
            this.castOnAllies = toallies;
            this.acc = acc;
            this.crit = crit;

            moveHost = 0;
            moveTarget = 0;
        }

        public virtual List<AbstractUnit> SelectTarget(List<AbstractUnit> allies, List<AbstractUnit> enemyes)
        {
            //List<AbstractUnit> res = new List<AbstractUnit>();
            AbstractUnit a;
            int chosen = 0, mult = (castOnAllies) ? 1 : -1;
            List<AbstractUnit> chooseIn = (castOnAllies) ? allies : enemyes;
            ConsoleKey key = ConsoleKey.A;
            do
            {
                int dones = 0;
                do
                {
                    Fight.TraceFighters(chosen * mult, out a, allies, enemyes, (targets - 1) * mult);
                    Console.WriteLine("\nSelect target for\n" + ToString());
                    Console.WriteLine("Select a " + ((a == null) ? "is null" : a.ToString()));
                    int incr = 0;

                    if (dones++ == 0)
                        incr = 1;
                    else
                    {
                        key = Console.ReadKey().Key;

                        switch (key)
                        {
                            case ConsoleKey.LeftArrow:
                                incr = 1;
                                break;
                            case ConsoleKey.RightArrow:
                                incr = -1;
                                break;
                            case ConsoleKey.DownArrow:
                                incr = 1;
                                break;
                            case ConsoleKey.UpArrow:
                                incr = -1;
                                break;
                            case ConsoleKey.Escape:
                                Console.Clear();
                                Fight.TraceFighters(0, out a, allies, enemyes);
                                return null;
                            default:
                                break;
                        }
                    }
                    int trys = 0;
                    do
                    {
                        chosen += incr * mult;
                        if (chosen < 1)
                            chosen = 4;
                        if (chosen > 4)
                            chosen = 1;
                        a = null;
                        for (int i = 0; i < chooseIn.Count; i++)
                            if (chooseIn[i].position == chosen)
                                a = chooseIn[i];
                        trys++;
                    } while (trys < 100 && (a == null || canTarget.IndexOf(a.position) < 0));
                    if (trys >= 99)
                    {
                        // nothing to attack with this spell
                        Console.Clear();
                        Fight.TraceFighters(0, out a, allies, enemyes);
                        return null;
                    }
                } while (key != ConsoleKey.Enter);
                //

                //
                if (chosen > 0 && chosen <= 4 && !this.CanBeUsedTo(chosen))
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ability '" + skillName + "' can not be used on unit in position " + chosen + "\t\t\t\t\t");
                    Thread.Sleep(500);
                }
            } while (!(chosen > 0 && chosen <= 4 && this.CanBeUsedTo(chosen)));

            List<AbstractUnit> resultUnits = new List<AbstractUnit>();
            resultUnits.Add(a);
            for (int i = 1; i < targets; i++)
            {
                int respos = a.position + i;
                for (int j = 0; j < chooseIn.Count; j++)
                    if (chooseIn[j].position == respos)
                        resultUnits.Add(chooseIn[j]);
            }
            return resultUnits;
        }

        public virtual void UseAbility(AbstractUnit unit)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("... " + unit.ToString() + " -- success");
            Console.Beep(600, 100);
        }

        public virtual void UseAbilityHostEffect(AbstractUnit host)
        {

        }

        public virtual void TryUseAbility(AbstractUnit host, List<AbstractUnit> to)
        {
            int nowIn = Console.CursorTop;
            Console.Beep(100, 50);
            Random rnd = new Random();

            UseAbilityHostEffect(host);
            host.MoveFor = moveHost;

            Thread.Sleep(100);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(String.Format("Used a '{0}' on a...", skillName));

            foreach (AbstractUnit a in to)
            {
                int rolled = rnd.Next(100);
                if (host.Acc + acc > rolled)
                {
                    if (host.Acc + acc - a.Ddg > rolled)
                    {
                        UseAbility(a);
                        a.MoveFor = moveTarget;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(String.Format("= = base ACC: {0}%; skill ACC: {1}%; target DDG: {2};",
                            host.Acc, acc, a.Ddg));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("... " + a.ToString() + " -- DODGE");
                        for (int i = 0; i < 3; i++) Console.Beep(rnd.Next(800, 1000), 50);
                        Thread.Sleep(300);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(String.Format("= = base ACC: {0}%; skill ACC: {1}%;",
                        host.Acc, acc));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("... " + a.ToString() + " -- MISS");
                    Console.Beep(300, 500);
                    Thread.Sleep(300);
                }
            }

            awaitEndOfAbility(400 * (Console.CursorTop - nowIn));
        }
        protected virtual void awaitEndOfAbility(int X)
        {
            Thread.Sleep(X);
        }
    }


    public abstract class AbstractUnit
    {
        public Mod MOD;
        protected string namepostfix;
        protected List<String> tags;
        protected string name;
        protected int healthMax;
        protected int health;

        protected int def;
        protected int acc;
        protected int crit;
        protected int spd;
        protected int dodge;

        public List<AbstractBuff> buffs;

        public int Def { get { return MOD.Def(def); } }
        public int Acc { get { return MOD.Acc(acc); } }
        public int Crt { get { return MOD.Crt(crit); } }
        public int Spd { get { return MOD.Spd(spd); } }
        public int Ddg { get { return MOD.Ddg(dodge); } }

        public int getMaxHealth { get { return healthMax; } }

        public int position;

        protected bool onDeathDoor;
        public bool leavesCorpse;
        public int MoveFor;

        protected List<AbstractAbility> abs;

        protected void BaseStats(int pos, string name, int health, int spd, int def, int dodge, int acc, int crit, List<AbstractAbility> abs, bool leavesCorpse)
        {
            this.position = pos; this.name = name;
            this.healthMax = this.health = health;
            this.def = def;
            this.acc = acc;
            this.crit = crit;
            this.abs = abs;
            this.spd = spd;
            this.dodge = dodge;
            this.onDeathDoor = false;
            buffs = new List<AbstractBuff>();
            this.leavesCorpse = leavesCorpse;
            this.MoveFor = 0;
            namepostfix = numbs[position];
            MOD = new Mod(0);
            tags = new List<string>();
        }

        public void AddTags(params String[] tags)
        {
            foreach (string tag in tags)
                if (this.tags.IndexOf(tag) < 0)
                    this.tags.Add(tag);
        }
        public void RemoveTags(params String[] tags)
        {
            foreach (string tag in tags)
                this.tags.Remove(tag);
        }

        public void Unstun()
        {
            for (int i = 0; i < buffs.Count; i++)
                if (buffs[i] as Stun != null)
                {
                    buffs[i].Remove();
                    buffs.RemoveAt(i);
                    i--;
                }
            Console.WriteLine(ToString()+ " is back to fight!");
            Console.Beep(400, 400);

            StunResist sr = new StunResist(this, 2);
            sr.TurnOn(0, 0);
            buffs.Add(sr);
        }

        public bool Is(string tag)
        {
            return tags.IndexOf(tag) >= 0;
        }

        public bool isDead()
        {
            return health <= 0;
        }
        public virtual bool isOnDeathDoor()
        {
            return health == 0;
        }
        public void RecieveDamage(int X)
        {
            RecieveDamage(X, false);
        }
        public virtual void RecieveDamage(int X, bool ignoreDef)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            int healthwas = health;
            health -= Math.Max(0, (int)(.5 + X / 100.0 * (100 - ((ignoreDef) ? 0 : Def))));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(String.Format("    {0} recieved {1} dmg ( DEF={2}% -> blocked {3} dmg)", ToString(), (healthwas - health), Def, X - (healthwas - health)));
            Console.Beep(100, 150);
            Thread.Sleep(1000);
        }
        public virtual void HealFor(int X)
        {
            int washealth = health;
            health = Math.Min(healthMax, health + X);
            Console.Beep(1500, 350);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    " + ToString() + " healed for " + (health - washealth));
            Thread.Sleep(500);
        }

        static string[] numbs = new string[] { "0", "I", "II", "III", "IV", "V" };
        public override string ToString()
        {
            return name + " " + namepostfix;
        }

        public string HealthToString(int leng)
        {
            string res = "";
            int proc = (int)(health * 1.0 / healthMax * leng);
            for (int i = 0; i < leng; i++)
                res += (i <= proc) ? "♥" : "-";

            if (health < healthMax * 2 / 3) Console.ForegroundColor = ConsoleColor.Yellow;
            if (health < healthMax / 3) Console.ForegroundColor = ConsoleColor.Red;
            if (health <= 0 || name.IndexOf("'s corpse") >= 0) Console.ForegroundColor = ConsoleColor.DarkMagenta;
            return res;
        }

        public string HealthToInt()
        {
            return String.Format(" HP:{0}/{1}", health, healthMax);
        }

        public string BuffsToString()
        {
            string res = "";
            for (int i = 0; i < buffs.Count; i++)
                if (!buffs[i].isFinished())
                    res += " " + buffs[i].symbol;
            return res;
        }

        public void Trace(int parameterSelected)
        {
            Console.SetCursorPosition(0, 8);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\"" + name + "\"\n");// Console.WriteLine(HealthToString(15));
            Console.WriteLine("Base stats:");
            Console.WriteLine(String.Format("   ACC: {0}\tDDG: {1}%\t\tSPD: {4}\n   CRT: {2}%\tDEF: {3}%\n", Acc, Ddg, Crt, Def, Spd));
            for (int i = 0; i < abs.Count; i++)
            {
                Console.ForegroundColor = (parameterSelected == i) ? ConsoleColor.White : ConsoleColor.DarkGray;
                if (!abs[i].CanBeUsedFrom(position)) Console.ForegroundColor = (parameterSelected == i) ? ConsoleColor.Red : ConsoleColor.DarkRed;

                Console.Write((i + 1) + "." + abs[i].ToString());
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void TryRandomlySelectAbility(Random rnd, List<AbstractUnit> allies, List<AbstractUnit> enemyes)
        {
            List<int> canBeUsed = new List<int>();
            List<List<int>> possibleTargets = new List<List<int>>();
            for (int i = 0; i < abs.Count; i++)
                if (abs[i].CanBeUsedFrom(position))
                    canBeUsed.Add(i);

            for (int i = 0; i < canBeUsed.Count; i++)
            {
                List<int> canBeUsedTo = new List<int>();

                if (abs[canBeUsed[i]].castOnAllies)
                {
                    for (int j = 0; j < allies.Count; j++)
                        if (abs[canBeUsed[i]].canTarget.IndexOf(allies[j].position) >= 0)
                            canBeUsedTo.Add(j);
                }
                else
                {
                    for (int j = 0; j < enemyes.Count; j++)
                        if (abs[canBeUsed[i]].canTarget.IndexOf(enemyes[j].position) >= 0)
                            canBeUsedTo.Add(j);
                }

                if (canBeUsedTo.Count == 0 && abs[canBeUsed[i]].canTarget.Count != 0)
                {
                    canBeUsed.RemoveAt(i); i--;
                }
                else
                    possibleTargets.Add(canBeUsedTo);
            }
            if (canBeUsed.Count <= 0)
                return;
            int selected = rnd.Next(canBeUsed.Count),
                chosen = canBeUsed[selected],
                chosenUnit = (abs[chosen].canTarget.Count > 0) ? possibleTargets[selected][rnd.Next(possibleTargets[selected].Count)] : -1;

            List<AbstractUnit> chooseFrom = (abs[chosen].castOnAllies) ? allies : enemyes;
            //Console.WriteLine(String.Format("Selected {0} with target {1}, so on {2}->{3}", chosen, chosenUnit, abs[chosen].skillName, chooseFrom[chosenUnit].ToString()));
            //Console.ReadLine();
            abs[chosen].TryUseAbility(this, (abs[chosen].canTarget.Count > 0) ? new List<AbstractUnit>() { chooseFrom[chosenUnit] } : new List<AbstractUnit>());
        }

        public void TrySelectAbility(List<AbstractUnit> allies, List<AbstractUnit> enemyes)
        {

            Console.ForegroundColor = ConsoleColor.Gray;
            int chosen = -1;
            ConsoleKey key;
        SelectAbility:
            do
            {
                do
                {
                    Trace(chosen);
                    Console.WriteLine("\nSelect an ability using keys 1, 2, ...");

                    key = Console.ReadKey().Key;
                    switch (key)
                    {
                        case ConsoleKey.D1:
                            chosen = 0; break;
                        case ConsoleKey.D2:
                            chosen = 1; break;
                        case ConsoleKey.D3:
                            chosen = 2; break;
                        case ConsoleKey.D4:
                            chosen = 3; break;
                        case ConsoleKey.D5:
                            chosen = 4; break;
                        case ConsoleKey.D6:
                            chosen = 5; break;
                        case ConsoleKey.D7:
                            chosen = 6; break;
                        case ConsoleKey.D8:
                            chosen = 7; break;
                        case ConsoleKey.D9:
                            chosen = 8; break;
                        case ConsoleKey.DownArrow:
                            chosen++;
                            if (chosen >= abs.Count)
                                chosen = 0;
                            break;
                        case ConsoleKey.UpArrow:
                            chosen--;
                            if (chosen < 0)
                                chosen = abs.Count - 1;
                            break;
                        default:
                            break;
                    }
                } while (key != ConsoleKey.Enter);
                if (!abs[chosen].CanBeUsedFrom(position))
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ability '" + abs[chosen].skillName + "' can not be used from position " + position + ".\t\t\t\t\t");
                }
                if (!abs[chosen].CanBeUsedOnAnyOf((abs[chosen].castOnAllies) ? allies : enemyes))
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ability '" + abs[chosen].skillName + "' can not be used on any of " + ((abs[chosen].castOnAllies) ? "allies." : "enemies.") + "\t\t\t\t\t");
                }
            } while (!(chosen >= 0 && chosen < abs.Count && abs[chosen].CanBeUsedFrom(position) && abs[chosen].CanBeUsedOnAnyOf((abs[chosen].castOnAllies) ? allies : enemyes)));

            if (abs[chosen].canTarget.Count == 0)
            { abs[chosen].UseAbility(null); return; }

            List<AbstractUnit> selected = abs[chosen].SelectTarget(allies, enemyes);
            if (selected == null)
                goto SelectAbility;
            abs[chosen].TryUseAbility(this, selected);
        }

        public void TickAllBuffs()
        {
            for (int i = 0; i < buffs.Count; i++)
            {
                buffs[i].Tick();
                if (buffs[i].isFinished())
                { buffs.RemoveAt(i); i--; }
            }
        }

        public Corpse CreateCorpseOfThis()
        {
            return new Corpse(name + "'s corpse", healthMax / 2, position);
        }
    }
    public class Corpse : AbstractUnit
    {
        public Corpse(string name, int healthmax, int pos)
        {
            BaseStats(pos, name, healthmax, 0, 0, 0, 0, 0, new List<AbstractAbility>() { new Pass() }, false);
        }
    }

    public abstract class AbstractBuff
    {
        static Random rnd = new Random();
        protected string name;
        protected string descr;
        public char symbol;
        public int turnsLeft;
        protected AbstractUnit target;
        protected bool enabled;

        public int applyChance;

        public void BaseStats(string name, string descr, int applyChance, char symbol, int turnsLeft, AbstractUnit target)
        {
            enabled = false;
            this.turnsLeft = turnsLeft;
            this.name = name;
            this.descr = descr;
            this.symbol = symbol;
            this.target = target;
            this.applyChance = applyChance;
        }

        public void Tick()
        {
            CheckEnd();
            if (!isFinished())
            {
                TickEffect();
                turnsLeft--;
            }
        }
        public bool isFinished()
        {
            return turnsLeft <= 0;
        }
        public virtual void Apply()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format("    {0} recieved a '{1}' ({3}) buff for {2} turns", target.ToString(), name, turnsLeft, symbol.ToString()));
            Console.Beep(900, 200);
            Thread.Sleep(80);
        }
        public virtual void Remove()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Removed " + name + " from " + target.ToString());
            Console.Beep(130, 300);
            Thread.Sleep(400);
        }
        public virtual void TickEffect() { }

        public void TurnOn(int hostApplyChance, int targetResist)
        {
            if (!isFinished())
            {
                if (!enabled)
                {
                    // CHANCE TO NOT APPLY!
                    int rolled = rnd.Next(100);

                    if (rolled < applyChance + hostApplyChance - targetResist)
                    {
                        enabled = true; Apply();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(String.Format("= = base CHANCE: {0}  host CHANCE: {1}  target RESIST: {2}",
                            applyChance, hostApplyChance, targetResist));
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(String.Format("# # {0} -- RESIST!", name));
                        turnsLeft = 0;
                    }

                }
            }
        }
        public void CheckEnd()
        {
            if (enabled && isFinished())
            { enabled = false; Remove(); }
        }
        public override string ToString()
        {
            return String.Format("{3}  {0} ({1} turns left) : {2}", name, turnsLeft, descr, symbol.ToString());
        }
    }
    //public enum BuffType
    //{
    //    blight = 14,
    //    bleed = 15,
    //    stun = 16,
    //    move = 17,
    //    debuff = 18,

    //    buff = 5,
    //    neutral = 6,
    //    self = 7
    //}
}
