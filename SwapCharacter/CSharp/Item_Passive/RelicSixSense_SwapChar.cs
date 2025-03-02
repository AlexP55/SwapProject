
namespace SwapCharacter
{
    /// <summary>
    /// 6th Sense
    /// Apply "6th Sense" on a random enemy when a battle starts.
    /// "6th Sense": Receiving Critical Chance +10%. Apply nonvanishing "Identified!" to the unit.When defeated, transfer "6th Sense" to another unit.
    /// </summary>
    public class RelicSixSense_SwapChar : PassiveItemBase, IP_BattleStart_Ones
    {
        public override void Init()
        {
            base.Init();
            OnePassive = true;
        }

        public void BattleStart(BattleSystem Ins)
        {
            var enemies = Ins.EnemyTeam.AliveChars;
            if (enemies.Count > 0) enemies.Random(BattleRandom.PassiveItem).BuffAdd(ModItemKeys.Buff_B_RelicSixSense_SwapChar, Ins.DummyChar);
        }
    }
}