using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Danger Perception
    /// When targeted by an enemy, swap to a position that cannot be attacked (prioritize the ally with the highest HP without this buff) and remove one stack. This enemy attack cannot reduce healing gauge.
    /// </summary>
    public class B_SwapChar_11_T:Buff, IP_TargetedAlly
    {
        public override void Init()
        {
            base.Init();
            PlusStat.dod = 20;
        }

        public IEnumerator Targeted(BattleChar Attacker, List<BattleChar> SaveTargets, Skill skill)
        {
            var ind = SaveTargets.IndexOf(BChar);
            if (ind >= 0)
            {
                var alliesSafe = BChar.MyTeam.AliveChars.FindAll(
                    bc => !SaveTargets.Contains(bc) && !bc.BuffFind(ModItemKeys.Buff_B_SwapChar_11_T, true));
                if (alliesSafe.Count <= 0) yield break;
                var swapAlly = alliesSafe.OrderByDescending(bc => bc.HP).First();
                yield return SwapUtils.SwapChars(BChar as BattleAlly, swapAlly as BattleAlly);
                SaveTargets[ind] = swapAlly;
                SelfStackDestroy();
                skill.ExtendedAdd(new Extended_SwapChar_11_Enemy());
            }
            yield break;
        }
    }
}