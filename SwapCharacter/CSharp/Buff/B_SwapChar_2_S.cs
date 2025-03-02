using SwapBaseMethod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SwapCharacter
{
    /// <summary>
    /// Guarding
    /// When a targets another ally, swap with the ally.
    /// This buff has lower priority than "Counter-Protected" and "Hold the Line!".
    /// </summary>
    public class B_SwapChar_2_S:Buff, IP_TurnEnd, IP_TargetedAlly
    {
        public BattleChar target;

        public override void Init()
        {
            base.Init();
            PlusStat.Strength = true;
            PlusStat.def = 20;
        }

        public override string DescExtended()
        {
            return base.DescExtended().Replace("&a", target?.Info.Name.ToString());
        }

        public void TurnEnd()
        {
            SelfDestroy();
        }

        public IEnumerator Targeted(BattleChar Attacker, List<BattleChar> SaveTargets, Skill skill)
        {
            if (target == Attacker)
            {
                if (SaveTargets.Count <= 0 || SaveTargets.Contains(BChar)) yield break;
                if (BChar.BuffFind(ModItemKeys.Buff_B_SwapChar_Rare_1_T)) yield break;
                if (SaveTargets.Any(bc => bc.BuffReturn(ModItemKeys.Buff_B_SwapChar_9_T)?.Usestate_L == BChar)) yield break;
                BChar.AddParryBuff(skill);
                yield return SwapUtils.SwapChars(BChar as BattleAlly, SaveTargets[0] as BattleAlly);
                SaveTargets[0] = BChar;
            }
            yield break;
        }
    }
}