using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDlib
{
    public class BookaDef : AbstractBuff
    {
        public BookaDef(AbstractUnit who)
        {
            BaseStats("Wonderfull defence", "Has +50 DEF for 5 turns", 100, 'D', 2, who);
        }
        public override void Apply()
        {
            base.Apply();
            target.MOD.DEF_add += 50;
        }
        public override void Remove()
        {
            base.Remove();
            target.MOD.DEF_add -= 50;
        }
    }
}
