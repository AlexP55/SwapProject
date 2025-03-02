using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Piercing Tempest
    /// Recast this skill a times based on <b>(number of attacks received + position changes)</b> of user during this turn and the last turn.
    /// </summary>
    public class S_SwapChar_Rare_2:Skill_Extended
    {
        public Dictionary<int, int> NumRecord = new Dictionary<int, int>();

        public int PlusHits
        {
            get
            {
                if (BattleSystem.instance == null) return 0;
                int turn = BattleSystem.instance.TurnNum;
                int num = ParticleLogs.Instance.Count(log => (log.Turn == turn || log.Turn == turn - 1) && log.UsedSkill.IsDamage && log.Targets.Contains(BChar));
                num += PositionChangeLogs.Instance.Count(log =>
                    (log.Turn == turn || log.Turn == turn - 1) && log.BC == BChar
                );
                return num;
            }
        }

        public override void Init()
        {
            base.Init();
            OnePassive = true;
        }

        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&a", PlusHits.ToString());
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            for (int i = 0; i < PlusHits; i++)
            {
                BattleSystem.DelayInput(PlusHit(Targets[0]));
            }
        }

        public IEnumerator PlusHit(BattleChar target)
        {
            yield return new WaitForSeconds(0.1f);
            var skill = Skill.TempSkill(ModItemKeys.Skill_S_SwapChar_Rare_2_Add, BChar, BChar.MyTeam);
            skill.FreeUse = true;
            skill.PlusHit = true;
            if (target.IsDead)
            {
                var enemies = BChar.BattleInfo.EnemyTeam.AliveChars;
                target = enemies.Count > 0 ? enemies.Random(BChar.GetRandomClass().Main) : null;
            }
            if (target != null) BChar.ParticleOut(skill, target);
            yield break;
        }

    }
}