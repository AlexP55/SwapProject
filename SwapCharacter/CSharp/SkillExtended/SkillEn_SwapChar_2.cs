using GameDataEditor;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// When use in hand, swap with the target and give both characters Tenacity (protect healing gauge, +5% armor)
    /// </summary>
    public class SkillEn_SwapChar_2:Skill_Extended
    {
        public override bool CanSkillEnforce(Skill MainSkill)
        {
            return MainSkill.MySkill.Target.Key == GDEItemKeys.s_targettype_ally ||
                MainSkill.MySkill.Target.Key == GDEItemKeys.s_targettype_otherally;
        }

        public override void SkillUseHand(BattleChar Target)
        {
            base.SkillUseHand(Target);
            Target.BuffAdd(ModItemKeys.Buff_B_SwapChar_Tenacity, BChar);
            BChar.BuffAdd(ModItemKeys.Buff_B_SwapChar_Tenacity, BChar);
            BattleSystem.DelayInputAfter(SwapUtils.SwapChars(Target as BattleAlly, BChar as BattleAlly));
        }
    }
}