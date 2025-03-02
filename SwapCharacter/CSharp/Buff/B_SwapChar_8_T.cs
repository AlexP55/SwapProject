using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Guerrilla Target
    /// Receive an additional hit that deals a damage (b% attack power) whenever a swap occurs.
    /// </summary>
    public class B_SwapChar_8_T : Buff, IP_Swapped
    {
        public static string AdditionSkill = ModItemKeys.Skill_S_SwapChar_8_Add;

        public override void Init()
        {
            base.Init();
            PlusStat.def = -10;
        }

        public override string DescExtended()
        {
            Skill skill = Skill.TempSkill(AdditionSkill, Usestate_L, Usestate_L.MyTeam);
            return base.DescExtended().Replace("&a", skill.TargetDamage.ToString())
                .Replace("&b", skill.MySkill.Effect_Target.DMG_Per.ToString());
        }

        public void Swapped(BattleChar ally1, BattleChar ally2)
        {
            if (ally1 is BattleAlly && ally2 is BattleAlly)
            {
                Skill skill = Skill.TempSkill(AdditionSkill, Usestate_L, Usestate_L.MyTeam);
                skill.PlusHit = true;
                skill.FreeUse = true;
                Usestate_L.ParticleOut(skill, BChar);
            }
        }
    }
}