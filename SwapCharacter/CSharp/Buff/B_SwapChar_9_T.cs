using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Counter-Protected
    /// When an enemy targets this ally, a swap with the ally and attacks before with b damage. Then remove this buff and create a "Counter-Offensive" that can only be used this turn.
    /// (This buff can be active on only one ally at a time)
    /// </summary>
    public class B_SwapChar_9_T : Buff, IP_TargetedAlly, IP_SomeOneDead, IP_Awake
    {
        public static string CounterSkill = ModItemKeys.Skill_S_SwapChar_9_Counter;

        public override void Init()
        {
            base.Init();
            PlusStat.Strength = true;
        }

        public void Awake()
        {
            foreach (BattleChar battleChar in BChar.BattleInfo.GetGlobalChar())
            {
                var buff = battleChar.BuffReturn(BuffData.Key);
                if (buff != null && buff != this && (buff.Usestate_L == Usestate_L || battleChar == Usestate_L))
                {
                    buff.SelfDestroy();
                }
            }
        }

        public override string DescExtended()
        {
            Skill skill = Skill.TempSkill(CounterSkill, Usestate_L, Usestate_L.MyTeam);
            return base.DescExtended().Replace("&a", Usestate_L.Info.Name).Replace("&b", skill.TargetDamage.ToString());
        }

        public IEnumerator Targeted(BattleChar Attacker, List<BattleChar> SaveTargets, Skill skill)
        {
            if (Usestate_L.IsDead) yield break;
            var ind = SaveTargets.IndexOf(BChar);
            if (ind >= 0 && !SaveTargets.Contains(Usestate_L))
            {
                Usestate_L.AddParryBuff(skill);
                yield return SwapUtils.SwapChars(BChar as BattleAlly, Usestate_L as BattleAlly);
                SaveTargets[ind] = Usestate_L;
                Skill counter = Skill.TempSkill(CounterSkill, Usestate_L, Usestate_L.MyTeam);
                counter.PlusHit = true;
                counter.FreeUse = true;
                Usestate_L.ParticleOut(counter, Attacker);
                SelfDestroy();
                var createSkill = Skill.TempSkill(ModItemKeys.Skill_S_SwapChar_9, Usestate_L, Usestate_L.MyTeam);
                createSkill.AutoDelete = 1;
                createSkill.isExcept = true;
                BattleSystem.instance.AllyTeam.Add(createSkill, true);
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }

        public void SomeOneDead(BattleChar DeadChar)
        {
            if (DeadChar == Usestate_L) SelfDestroy();
        }
    }
}