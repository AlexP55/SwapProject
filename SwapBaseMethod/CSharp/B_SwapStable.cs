using System.Linq;

namespace SwapBaseMethod
{
    public class B_SwapStable : Buff, IP_PositionChanged
    {
        public void PositionChanged(int oldPos, int newPos)
        {
            var allies = BChar.Info.Ally ? BattleSystem.instance.AllyTeam.Chars : BattleSystem.instance.SortedEnemies().Cast<BattleChar>().ToList();
            this.MoveTo(allies[oldPos], true);
        }
    }
}