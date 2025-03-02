using System.Linq;
using System.Collections;
using GameDataEditor;

namespace SwapCharacter
{
    /// <summary>
    /// Diversionary Attack
    /// When in countdown:
    /// - User gains healing gauge protection and armor +20%
    /// - Target cannot reduce healing gauge(a%)
    /// - When targeted enemy targets an ally, user swaps with the ally
    /// </summary>
    public class S_SwapChar_2 : Skill_Extended, IP_SkillCastingStart, IP_SkillCastingQuit
    {
        public override void Init()
        {
            base.Init();
        }

        public override string DescExtended(string desc)
        {
            var plusRate = new GDEBuffData(ModItemKeys.Buff_B_SwapChar_2_T).TagPer;
            return base.DescExtended(desc).Replace("&a", ((int)BChar.GetStat.HIT_DEBUFF + plusRate).ToString());
        }

        public void SkillCasting(CastingSkill ThisSkill)
        {
            ThisSkill.Target?.BuffAdd(ModItemKeys.Buff_B_SwapChar_2_T, BChar);
            if (BChar.BuffAdd(ModItemKeys.Buff_B_SwapChar_2_S, BChar) is B_SwapChar_2_S buff)
            {
                buff.target = ThisSkill.Target;
            }
        }

        public void SkillCastingQuit(CastingSkill ThisSkill)
        {
            BattleSystem.DelayInputAfter(BuffRemove(ThisSkill));
        }

        public IEnumerator BuffRemove(CastingSkill ThisSkill)
        {
            var castList = BattleSystem.instance.CastSkills.ToList();
            castList.AddRange(BattleSystem.instance.SaveSkill);
            castList = castList.FindAll(cs => cs != ThisSkill && cs.skill.MySkill.KeyID == ModItemKeys.Skill_S_SwapChar_2);
            if (!castList.Any(cs => cs.Usestate == BChar))
                BChar.BuffRemove(ModItemKeys.Buff_B_SwapChar_2_S);
            if (!castList.Any(cs => cs.Target == ThisSkill.Target))
                ThisSkill.Target.BuffRemove(ModItemKeys.Buff_B_SwapChar_2_T);
            yield break;
        }
    }
}