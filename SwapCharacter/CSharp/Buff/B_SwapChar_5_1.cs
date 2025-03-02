using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Attack Increase!
    /// Unique Position Buff
    /// It moves to the right ally every turn, and vanishes after reaching the end.
	/// </summary>
    public class B_SwapChar_5_1:B_SwapStable, IP_PlayerTurn, IP_Awake
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
            PlusPerStat.Damage = 30;
            PlusStat.hit = 10;
        }

        public void Turn()
        {
            var allies = BChar.MyTeam.AliveChars;
            var ind = allies.IndexOf(BChar);
            if (ind < allies.Count - 1)
            {
                allies[ind + 1].BuffAdd(BuffData.Key, Usestate_L);
            }
            SelfDestroy();
        }
    }
}