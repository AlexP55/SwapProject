using System.Linq;
using System.Collections.Generic;

namespace SwapCharacter
{
    /// <summary>
    /// Terrain Advantage
    /// Apply "Attack Increase!" to the leftmost ally.
    /// Apply "Healing Increase!" to the rightmost ally.
    /// </summary>
    public class S_SwapChar_5:Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            var allies = BattleSystem.instance.AllyTeam.AliveChars;
            allies.First().BuffAdd(ModItemKeys.Buff_B_SwapChar_5_1, BChar);
            allies.Last().BuffAdd(ModItemKeys.Buff_B_SwapChar_5_2, BChar);
            AddressableLoadManager.Instantiate(MySkill.MySkill.Particle_Path, AddressableLoadManager.ManageType.Battle);
        }
    }
}