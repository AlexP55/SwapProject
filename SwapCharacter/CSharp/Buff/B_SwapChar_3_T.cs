using System.Collections;

namespace SwapCharacter
{
    /// <summary>
    /// Lost Balance
    /// 1 stack removed per action.
    /// </summary>
    public class B_SwapChar_3_T:Buff, IP_SkillUse_User_After
    {
        public override void BuffStat()
        {
            base.BuffStat();
            PlusStat.Weak = true;
            PlusStat.cri = -10 * StackNum;
        }

        public void SkillUseAfter(Skill SkillD)
        {
            BattleSystem.DelayInput(SkillUseAfter());
        }

        public IEnumerator SkillUseAfter()
        {
            SelfStackDestroy();
            yield break;
        }
    }
}