using UnityEngine;
using System.Collections;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Fake Ally
    /// Passive:
    /// Destroyed when HP is below 0 or its sommoner faints.
    /// </summary>
    public class P_DummySummon:Passive_Char, IP_Dead
    {
        public override void Init()
        {
            base.Init();
            OnePassive = true;
        }

        public IEnumerator NewSummon(BattleChar summoner, int hp)
        {
            PlusStat.maxhp += hp - BChar.GetStat.maxhp;
            var buff = BChar.BuffAdd(ModItemKeys.Buff_B_DummySummon_P, BChar);
            if (buff is B_DummySummon_P dummyBuff) dummyBuff.Summoner = summoner;
            BChar.Info.Hp = BChar.GetStat.maxhp;
            yield break;
        }

        public void Dead()
        {
            BattleSystem.instance.StartCoroutine(RemoveUI());
        }

        public IEnumerator RemoveUI()
        {
            yield return new WaitForSeconds(1f);
            if (BChar is BattleAlly ally)
            {
                SwapUtils.RemoveAlly(ally);
            }
            yield break;
        }
    }
}