
namespace SwapCharacter
{
    /// <summary>
    /// Spear of Protection
    /// Receive x stacks of Tenacity (protect healing gauge, +5% armor per stack) at the end of turn based on num of hits received this turn.
    /// </summary>
    public class EquipUnique_SwapChar:EquipBase, IP_BattleStart_Ones
    {
        public void BattleStart(BattleSystem Ins)
        {
            BChar.BuffAdd(ModItemKeys.Buff_B_EquipUnique_SwapChar, BChar);
        }

        public override void Init()
        {
            base.Init();
            PlusStat.atk = 2;
            PlusStat.def = 15;
            PlusStat.HIT_CC = 15;
            PlusStat.HIT_DEBUFF = 15;
            PlusStat.HIT_DOT = 15;
            PlusStat.RES_CC = 15;
            PlusStat.RES_DEBUFF = 15;
            PlusStat.RES_DOT = 15;
        }

    }
}