
namespace SwapCharacter
{
    /// <summary>
    /// Tenacity
    /// Armor +5% for each stack.
    /// Consume one stack when hit by an attack that can reduce healing gauge.
    /// </summary>
    public class B_SwapChar_Tenacity:Buff, IP_Hit
    {
        public override void BuffStat()
        {
            base.BuffStat();
            PlusStat.def = StackNum * 5;
            PlusStat.Strength = true;
        }

        public void Hit(SkillParticle SP, int Dmg, bool Cri)
        {
            var skill = SP.SkillData;
            var user = skill.Master;
            if (Dmg >= 1 && !user.GetStat.Weak && !skill.PlusSkillStat.Weak)
            {
                SelfStackDestroy();
            }
        }
    }
}