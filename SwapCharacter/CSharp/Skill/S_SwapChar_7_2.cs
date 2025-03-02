using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Fake Reinforcement
    /// Usable only if ally number is less than c.
    /// Deal pain damage equal to 25% of max HP to create a hologram(ally) with a% (b) max HP and very low aggro.The hologram is destroyed when HP is below 0 and has a fixed skill to swap position (the first use is free, after that costs 1 mana).
    /// </summary>
    public class S_SwapChar_7_2:Skill_Extended
    {
        public const float ratio = 67f;
        public static int MaxAlly => SwapCharacter_Plugin.FifthMember ? 5 : 4;

        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&c", MaxAlly.ToString()).Replace("&a", ratio.ToString())
                .Replace("&b", ((int)Misc.PerToNum(BChar.GetStat.maxhp, ratio)).ToString());
        }

        public override bool ButtonSelectTerms()
        {
            if (BattleSystem.instance.AllyTeam.CharsViewable().Count >= MaxAlly) return false;
            return base.ButtonSelectTerms();
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            Targets[0].Damage(BChar, (int)Misc.PerToNum(Targets[0].GetStat.maxhp, 25), false, true);
            if (BattleSystem.instance.AllyTeam.CharsViewable().Count >= MaxAlly)
            {
                Targets[0].BuffAdd(ModItemKeys.Buff_B_SwapChar_7_T, BChar);
            }
            else
            {
                var battleAlly = SwapUtils.NewAlly(ModItemKeys.Character_DummySummon);
                battleAlly.MyBasicSkill.buttonData.APChange = -99;
                var hp = (int)Misc.PerToNum(BChar.GetStat.maxhp, ratio);
                BattleSystem.DelayInput((battleAlly.Info.Passive as P_DummySummon).NewSummon(BChar, hp));
            }
        }
    }
}