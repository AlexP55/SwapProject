using System.Collections.Generic;

namespace SwapCharacter
{
    /// <summary>
    /// Watch Out!
    /// Draw 2 skills.
    /// </summary>
    public class S_SwapChar_Lucy:Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            BattleSystem.instance.AllyTeam.Draw(2);
        }
    }
}