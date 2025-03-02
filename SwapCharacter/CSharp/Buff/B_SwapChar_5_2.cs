using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Healing Increase!
    /// Unique Position Buff
    /// It moves to the left ally every turn, and vanishes after reaching the end.
	/// </summary>
    public class B_SwapChar_5_2:B_SwapStable, IP_PlayerTurn, IP_Awake
    {
        public void Awake()
        {
            foreach (var ally in BChar.BattleInfo.GetGlobalChar())
            {
                if (ally != BChar) ally.BuffRemove(BuffData.Key);
            }
        }

        public override void Init()
        {
            base.Init();
            PlusPerStat.Heal = 30;
            PlusStat.HEALTaken = 15;
        }

        public void Turn()
        {
            var allies = BChar.MyTeam.AliveChars;
            var ind = allies.IndexOf(BChar);
            if (ind > 0)
            {
                allies[ind - 1].BuffAdd(BuffData.Key, Usestate_L);
            }
            SelfDestroy();
        }

    }
}