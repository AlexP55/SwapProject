using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Armored Charge
    /// Damage +b (a%) based on user's armor x2.
    /// Frontal Assault: Restore 2 mana.
    /// </summary>
    public class S_SwapChar_4:Skill_Extended
    {
        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&a", ((int)(BChar.GetStat.def * 2)).ToString())
                .Replace("&b", ((int)Misc.PerToNum(BChar.GetStat.atk, BChar.GetStat.def * 2)).ToString());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            SkillBasePlus.Target_BaseDMG = (int)Misc.PerToNum(BChar.GetStat.atk, BChar.GetStat.def * 2);
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            SkillBasePlus.Target_BaseDMG = (int)Misc.PerToNum(BChar.GetStat.atk, BChar.GetStat.def);
            if (SwapUtils.SamePosition(MySkill.Master as BattleAlly, Targets[0] as BattleEnemy))
            {
                BattleSystem.instance.AllyTeam.AP += 2;
            }
        }
    }
}