using GameDataEditor;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Dazzling Shield
    /// Position Buff
    /// Apply stun(a%) when hit by an enemy.
    /// </summary>
    public class B_SwapChar_Rare_3_T:B_SwapStable, IP_Hit
    {
        public const int rate = 110;

        public void Hit(SkillParticle SP, int Dmg, bool Cri)
        {
            if (SP.SkillData.Master.Info.Ally != Usestate_L.Info.Ally)
            {
                SP.SkillData.Master.BuffAdd(GDEItemKeys.Buff_B_Common_Rest, Usestate_L, false, rate);
            }
        }

        public override string DescExtended()
        {
            var ratePlus = new GDEBuffData(GDEItemKeys.Buff_B_Common_Rest).TagPer;
            return base.DescExtended().Replace("&a", ((int)Usestate_L.GetStat.HIT_CC + rate + ratePlus).ToString());
        }
    }
}