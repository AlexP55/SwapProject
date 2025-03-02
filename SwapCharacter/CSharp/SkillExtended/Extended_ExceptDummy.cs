
namespace SwapCharacter
{
    /// <summary>
    /// Cannot target Hologram character
    /// </summary>
    public class Extended_ExceptDummy:Skill_Extended
    {
        public override bool ButtonSelectTerms()
        {
            if (BChar.Info.KeyData == ModItemKeys.Character_DummySummon) return false;
            return base.ButtonSelectTerms();
        }

        public override bool TargetSelectExcept(BattleChar ExceptTarget)
        {
            if (ExceptTarget.Info.KeyData == ModItemKeys.Character_DummySummon) return true;
            return base.TargetSelectExcept(ExceptTarget);
        }
    }
}