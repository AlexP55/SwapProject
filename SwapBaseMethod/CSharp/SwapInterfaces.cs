
namespace SwapBaseMethod
{
    public interface IP_PositionChanged
    {
        void PositionChanged(int oldPos, int newPos);
    }

    public interface IP_Swapped
    {
        void Swapped(BattleChar char1, BattleChar char2);
    }

    public interface IP_StatsStable
    {

    }
}
