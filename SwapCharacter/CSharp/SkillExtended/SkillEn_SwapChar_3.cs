using System.Collections;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// When in hand, every 2 times two allies swap with each other, cost -1
    /// </summary>
    public class SkillEn_SwapChar_3:Skill_Extended, IP_Swapped
    {
        public int SwapNum = 0;

        public override void Init()
        {
            base.Init();
            OnePassive = true;
        }

        public override IEnumerator DrawAction()
        {
            SwapNum = 0;
            APChange = 0;
            yield break;
        }

        public override bool CanSkillEnforce(Skill MainSkill)
        {
            return MainSkill.AP >= 1;
        }

        public void Swapped(BattleChar ally1, BattleChar ally2)
        {
            if (ally1 is BattleAlly && ally2 is BattleAlly)
            {
                SwapNum++;
                APChange = -(SwapNum / 2);
            }
        }
    }
}