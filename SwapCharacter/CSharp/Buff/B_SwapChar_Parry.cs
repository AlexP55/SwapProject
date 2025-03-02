using DarkTonic.MasterAudio;

namespace SwapCharacter
{
    /// <summary>
    /// Hidden buff for parry sound effects only
    /// </summary>
    public class B_SwapChar_Parry : Buff, IP_Hit, IP_TurnEnd
    {
        public Skill skillParry;

        public void Hit(SkillParticle SP, int Dmg, bool Cri)
        {
            if (SP.SkillData == skillParry)
            {
                MasterAudio.PlaySound("SC_Impact_04");
                SelfDestroy();
            }
        }

        public void TurnEnd()
        {
            SelfDestroy();
        }
    }
}