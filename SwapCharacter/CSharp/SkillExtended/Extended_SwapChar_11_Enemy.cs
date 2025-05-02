
namespace SwapCharacter
{
    public class Extended_SwapChar_11_Enemy : Skill_Extended, IP_SkillUse_User_After
    {
        public override void Init()
        {
            base.Init();
            PlusSkillStat.Weak = true;
        }

        public void SkillUseAfter(Skill SkillD)
        {
            if (SkillD == MySkill)
            {
                SelfDestroy();
            }
        }
    }
}
