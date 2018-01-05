using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDlib
{
    public struct Mod
    {
        public static int Versus(AbstractUnit unit, string tag, int caseSuccess, string param)
        {
            int extradamage = 0;
            if (unit.Is(tag))
            {
                extradamage = caseSuccess;
                Console.WriteLine(String.Format("   >{0} is {1}. Ability gain +{2}{3}", unit.ToString(), tag, caseSuccess, param));
            }
            return extradamage;
        }

        static Random rnd = new Random();
        public int DMG_add;
        /// <summary>
        /// in %
        /// </summary>
        public int DMG_mod;

        public int CRT_add;
        public int CRT_mod;

        public int DEF_add;
        public int DEF_mod;

        public int HP_add;
        public int HP_mod;

        public int ACC_add;
        public int ACC_mod;

        public int SPD_add;
        public int SPD_mod;

        public int DDG_add;
        public int DDG_mod;

        public int BLEED_resist;
        public int BLEED_resist_mod;

        public int BLIGHT_resist;
        public int BLIGHT_resist_mod;

        public int STUN_resist;
        public int STUN_resist_mod;

        public int MOVE_resist;
        public int MOVE_resist_mod;

        public int DEBUFF_resist;
        public int DEBUFF_resist_mod;

        public int apply_blight_chance;
        public int apply_bleed_chance;
        public int apply_stun_chance;
        public int apply_move_chance;
        public int apply_debuff_chance;


        public void SetDebuffResissts(int blight, int bleed, int stun, int move, int debuff)
        {
            BLEED_resist = bleed;
            BLIGHT_resist = blight;
            STUN_resist = stun;
            MOVE_resist = move;
            DEBUFF_resist = debuff;

        }

        public int ResistStun
        {
            get { return STUN_resist + STUN_resist_mod; }
        }
        public int ResistBlight
        {
            get { return BLIGHT_resist + BLIGHT_resist_mod; }
        }
        public int ResistBleed
        {
            get { return BLEED_resist + BLEED_resist_mod; }
        }
        public int ResistMove
        {
            get { return MOVE_resist + MOVE_resist_mod; }
        }
        public int ResistDebuff
        {
            get { return DEBUFF_resist + DEBUFF_resist_mod; }
        }

        public Mod(int X)
        {
            apply_bleed_chance = apply_blight_chance = apply_debuff_chance = apply_move_chance = apply_stun_chance = 0;
            DMG_add = DMG_mod = CRT_add = CRT_mod = DEF_add = DEF_mod = HP_add = HP_mod = ACC_add = ACC_mod = SPD_add = SPD_mod = DDG_add = DDG_mod = 0;
            BLEED_resist_mod = BLIGHT_resist_mod = MOVE_resist_mod = STUN_resist_mod = DEBUFF_resist_mod = BLEED_resist = BLIGHT_resist = MOVE_resist = STUN_resist = DEBUFF_resist = 0;
        }

        public int FinalDamage(int minDmg, int maxDmg)
        {
            return rnd.Next((int)((minDmg + DMG_add) * (DMG_mod + 100) / 100.0), 1 + (int)((maxDmg + DMG_add) * (DMG_mod + 100) / 100.0));
        }
        public int Def(int def)
        {
            return (int)((def + DEF_add) * (DEF_mod + 100) / 100.0);
        }
        public int Acc(int acc)
        {
            return (int)((acc + ACC_add) * (ACC_mod + 100) / 100.0);
        }
        public int Ddg(int ddg)
        {
            return (int)((ddg + DDG_add) * (DDG_mod + 100) / 100.0);
        }
        public int Spd(int spd)
        {
            return (int)((spd + SPD_add) * (SPD_mod + 100) / 100.0);
        }
        public int Crt(int crt)
        {
            return (int)((crt + CRT_add) * (CRT_mod + 100) / 100.0);
        }
        public int MaxHP(int hp)
        {
            return (int)((hp + HP_add) * (HP_mod + 100) / 100.0);
        }
    }
}
