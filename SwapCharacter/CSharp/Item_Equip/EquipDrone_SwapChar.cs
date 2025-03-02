using System.Collections;

namespace SwapCharacter
{
    /// <summary>
    /// Support Drone
    /// At the start of the battle, move the stats of this equipment to a Position Buff.
    /// At the start of a new turn, select to move the buff to a new position.
    /// </summary>
    public class EquipDrone_SwapChar : EquipBase, IP_BattleStart_Ones, IP_BattleEnd, IP_Dead
    {
        public Stat EnchantStat = default;
        public PerStat EnchantPerStat = default;
        public Stat CurseStat = default;
        public PerStat CursePerStat = default;

        public override void Init()
        {
            base.Init();
            PlusStat.atk = 2;
            PlusStat.reg = 2;
            PlusStat.def = 10;
        }

        public void BattleStart(BattleSystem Ins)
        {
            BattleSystem.DelayInputAfter(Del());
        }

        private IEnumerator Del()
        {
            var buff = BChar.BuffAdd(ModItemKeys.Buff_B_EquipDrone_SwapChar, BChar);
            EnchantStat = MyItem.Enchant.EnchantData.PlusStat;
            EnchantPerStat = MyItem.Enchant.EnchantData.PlusPerStat;
            CurseStat = MyItem.Curse.PlusStat;
            CursePerStat = MyItem.Curse.PlusPerStat;
            buff.PlusStat = PlusStat + EnchantStat + CurseStat;
            buff.PlusPerStat = PlusPerStat + EnchantPerStat + CursePerStat;
            PlusStat = default;
            PlusPerStat = default;
            MyItem.Enchant.EnchantData.PlusStat = default;
            MyItem.Enchant.EnchantData.PlusPerStat = default;
            MyItem.Curse.PlusStat = default;
            MyItem.Curse.PlusPerStat = default;
            yield break;
        }

        public void BattleEnd()
        {
            ResetStats();
        }

        public void Dead()
        {
            ResetStats();
        }

        private void ResetStats()
        {
            Init();
            MyItem.Enchant.EnchantData.PlusStat = EnchantStat;
            MyItem.Enchant.EnchantData.PlusPerStat = EnchantPerStat;
            MyItem.Curse.PlusStat = CurseStat;
            MyItem.Curse.PlusPerStat = CursePerStat;
            EnchantStat = default;
            EnchantPerStat = default;
            CurseStat = default;
            CursePerStat = default;
    }
    }
}