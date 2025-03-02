using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Breakout Thrust
    /// Frontal Assault: Draw 1 skill.
    /// </summary>
    public class S_SwapChar_3 : Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            if (SwapUtils.SamePosition(MySkill.Master as BattleAlly, Targets[0] as BattleEnemy))
            {
                BattleSystem.instance.AllyTeam.Draw(1);
            }
        }
    }
}