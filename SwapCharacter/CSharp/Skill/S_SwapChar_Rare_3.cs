using System.Collections.Generic;
using GameDataEditor;

namespace SwapCharacter
{
    /// <summary>
    /// Holographic Force Field
    /// The target applies taunted to all enemies (a%).
    /// </summary>
    public class S_SwapChar_Rare_3:Skill_Extended
    {
        public const int rate = 20;

        public override string DescExtended(string desc)
        {
            var ratePlus = new GDEBuffData(GDEItemKeys.Buff_B_Taunt).TagPer;
            return base.DescExtended(desc).Replace("&a", ((int)BChar.GetStat.HIT_CC + rate + ratePlus).ToString());
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            foreach (var enemy in BattleSystem.instance.EnemyList)
            {
                enemy.BuffAdd(GDEItemKeys.Buff_B_Taunt, Targets[0], false, 
                    (int)BChar.GetStat.HIT_CC - (int)Targets[0].GetStat.HIT_CC + rate);
            }
        }
    }
}