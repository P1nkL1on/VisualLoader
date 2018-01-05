using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDlib.Units.Player
{
    public abstract class AbstractPlayersAbility : AbstractAbility
    {
        int DMG_mod;
        protected AbstractPlayer host;
        protected void PlayerBaseStats(AbstractPlayer host, string name, List<int> from, List<int> to, int targets, bool toallies, int acc, int dmg, int crit, string descr)
        {
            this.host = host;
            this.DMG_mod = dmg;
            BaseStats(name, from, to, targets, toallies, acc, crit, descr);
        }
        protected int getDmg(int specialDMGMOD)
        {
            host.MOD.DMG_mod += DMG_mod + specialDMGMOD;
            int resDmg = host.MOD.FinalDamage(host.minDamage, host.maxDamage);
            host.MOD.DMG_mod -= (DMG_mod + specialDMGMOD);
            return resDmg;
        }
        protected override string DESCR()
        {
            return String.Format("ACC: {0}  CRT: {1}  BASE DMG: {4}-{5}\n   {2} : {3}", acc, crit, skillName, skillDescr,
                Math.Floor(host.minDamage * (100 - DMG_mod) / 100.0) + 1, Math.Floor(host.maxDamage * (100 - DMG_mod) / 100.0) + 1);
        }
        protected override void awaitEndOfAbility(int X)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }

    public abstract class AbstractPlayer : AbstractUnit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Занимаемая позиция в отряде</param>
        /// <param name="name">Имя класса</param>
        /// <param name="characterName">Имя персонажа</param>
        /// <param name="minDmg">Минимальный урон</param>
        /// <param name="maxDmg">Макс. урон</param>
        /// <param name="health">Максимальный запас ХП</param>
        /// <param name="spd">Скорость</param>
        /// <param name="def">База защиты</param>
        /// <param name="acc">База точность %</param>
        /// <param name="crit">Баз крит %</param>
        /// <param name="resStun">Сопротивление стану</param>
        /// <param name="resBlight">Сопротивление яду</param>
        /// <param name="resBleed">Сопротивление кровотечению</param>
        /// <param name="resMove">Сопротивление перемещени.</param>
        /// <param name="resDebuff">Сопротивление дебаффам различного рода</param>
        /// <param name="abs">Список способностей (перемещение и пасс будут добавлены автоматически)</param>
        /// <param name="moveLength">Длина подшага</param>

        public int minDamage;
        public int maxDamage;
        protected void PlayerBaseStats(int pos, string name, string characterName,
            int minDmg, int maxDmg,
            int health, int spd, int def, int acc, int crit, 
            int resStun, int resBlight, int resBleed, int resMove, int resDebuff, 
            List<AbstractAbility> abs, int moveLength)
        {
            this.minDamage = minDmg;
            this.maxDamage = maxDmg;
            this.position = pos; this.name = name;
            this.healthMax = this.health = health;
            this.def = def;
            this.acc = acc;
            this.crit = crit;

            abs.Add(new Move(moveLength, this));
            abs.Add(new Move(-moveLength, this));
            abs.Add(new Pass());

            this.abs = abs;
            this.spd = spd;
            this.onDeathDoor = false;
            buffs = new List<AbstractBuff>();
            this.leavesCorpse = leavesCorpse;
            this.MoveFor = 0;
            namepostfix = characterName;
            MOD = new Mod(0);
            tags = new List<string>() {"player"};
            MOD.SetDebuffResissts(resBlight, resBleed, resStun, resMove, resDebuff);
        }

    }
}
