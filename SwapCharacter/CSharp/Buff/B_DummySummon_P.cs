using System.Collections;
using UnityEngine;

namespace SwapCharacter
{
    /// <summary>
    /// Protection Hologram
    /// Hologram is destroyed when HP is below 0 or summoner faints.
    /// Can't be targeted by some boss skills.
    /// </summary>
    public class B_DummySummon_P:Buff, IP_HPChange, IP_SomeOneDead
    {
        public BattleChar Summoner;

        public override string DescExtended()
        {
            return base.DescExtended().Replace("&a", Summoner?.Info?.Name.ToString());
        }

        public void HPChange(BattleChar Char, bool Healed)
        {
            if (Char.HP <= 0)
            { // make sure it's triggered after healing wounds
                IEnumerator DelayDestroy()
                {
                    yield return new WaitForEndOfFrame();
                    IEnumerator DestorySelf()
                    {
                        if (Char.HP <= 0) DestoryDummy();
                        yield break;
                    }
                    BattleSystem.DelayInputAfter(DestorySelf());
                }
                BattleSystem.instance.StartCoroutine(DelayDestroy());
            }
        }

        public void SomeOneDead(BattleChar DeadChar)
        {
            if (DeadChar == Summoner) DestoryDummy();
        }

        public void DestoryDummy()
        {
            if (BChar is BattleAlly ally)
            {
                ally.ForceDead();
            }
            else
            {
                BChar.Dead();
            }
        }

        public override void Init()
        {
            base.Init();
            PlusStat.dod = -100;
            PlusStat.RES_CC = 100;
            PlusStat.RES_DOT = 30;
            PlusStat.AggroPer = -100;
        }
    }
}