using GameDataEditor;

namespace SwapCharacter
{
    /// <summary>
    /// Analyze Battlefield
    /// When cast, apply "Identified!" to all enemies.
    /// </summary>
    public class S_SwapChar_6:Skill_Extended, IP_SkillCastingStart
    {
        public void SkillCasting(CastingSkill ThisSkill)
        {
            foreach (BattleChar battleChar in BattleSystem.instance.EnemyTeam.AliveChars)
            {
                battleChar.BuffAdd(GDEItemKeys.Buff_B_Control_P, BChar);
            }
        }
    }
}