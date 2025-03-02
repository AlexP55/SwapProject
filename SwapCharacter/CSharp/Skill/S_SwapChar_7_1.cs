using System.Collections.Generic;

namespace SwapCharacter
{
    /// <summary>
    /// VIP Cover
    /// Deal pain damage equal to 25% of target max HP to create a hologram barrier (a% max hp of user).
    /// </summary>
    public class S_SwapChar_7_1:Skill_Extended
    {
        public const float ratio = B_SwapChar_7_T.ratio;

        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&a", ratio.ToString());
        }

        public override bool TargetSelectExcept(BattleChar ExceptTarget)
        {
            if (ExceptTarget.Info.KeyData == ModItemKeys.Character_DummySummon) return true;
            return base.TargetSelectExcept(ExceptTarget);
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            Targets[0].Damage(BChar, (int)Misc.PerToNum(Targets[0].GetStat.maxhp, 25), false, true);
        }
    }
}