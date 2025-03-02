
namespace SwapCharacter
{
    /// <summary>
    /// Weak Points
    /// Receiving attack damage increases by a.
    /// </summary>
    public class B_SwapChar_Rare_2_T : Buff, IP_DamageChange_Hit_sumoperation
    {
        public override string DescExtended()
        {
            return base.DescExtended().Replace("&a", StackNum.ToString());
        }

        public void DamageChange_Hit_sumoperation(Skill SkillD, int Damage, ref bool Cri, bool View, ref int PlusDamage)
        {
            if (Damage > 0) PlusDamage = StackNum;
            else PlusDamage = 0;
        }
    }
}