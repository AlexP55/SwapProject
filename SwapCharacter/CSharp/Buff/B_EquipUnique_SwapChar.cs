
namespace SwapCharacter
{
    /// <summary>
    /// Spear of Protection
    /// Receive a stacks of Tenacity (protect healing gauge, +5% armor per stack) at the end of turn based on num of hits received this turn.
    /// </summary>
    public class B_EquipUnique_SwapChar : Buff, IP_TurnEnd, IP_Hit
    {
        int num = 0;

        public override string DescExtended()
        {
            return base.DescExtended().Replace("&a", num.ToString());
        }

        public void Hit(SkillParticle SP, int Dmg, bool Cri)
        {
            if (Dmg >= 1)
            {
                num++;
            }
        }

        public void TurnEnd()
        {
            for (int i = 0; i < num; i++)
            {
                BChar.BuffAdd(ModItemKeys.Buff_B_SwapChar_Tenacity, BChar);
            }
            num = 0;
        }
    }
}