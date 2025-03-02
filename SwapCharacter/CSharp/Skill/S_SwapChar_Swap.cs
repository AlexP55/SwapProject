using System.Collections;
using System.Collections.Generic;
using SwapBaseMethod;
using UnityEngine;

namespace SwapCharacter
{
    /// <summary>
    /// Swap!
    /// Swap with the target.
    /// Can be used infinity times as a fixed skill.
    /// </summary>
    public class S_SwapChar_Swap:Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            var p = AddressableLoadManager.Instantiate(MySkill.MySkill.Particle_Path, AddressableLoadManager.ManageType.Battle);
            p.transform.position = BChar.GetPos();
            if (Targets[0] is BattleAlly ally && BChar is BattleAlly me)
            {
                BattleSystem.DelayInput(SwapUtils.SwapChars(ally, me));
            }
        }

        public override void SkillUseSingleAfter(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingleAfter(SkillD, Targets);
            if ((BChar as BattleAlly).MyBasicSkill.buttonData == MySkill)
            {
                BattleSystem.DelayInputAfter(Refill());
            }
        }

        private IEnumerator Refill()
        {
            yield return new WaitForSeconds(0.2f);
            BChar.MyTeam.BasicSkillRefill(BChar, BChar.BattleBasicskillRefill);
            yield break;
        }
    }
}