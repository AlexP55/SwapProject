using System.Collections.Generic;

namespace SwapBaseMethod
{
    public class PositionChangeLogs : List<PositionChangeLog>
    {
        private int Ind = 0;

        public static PositionChangeLogs Instance => BattleSystem.instance?.GetOrAddBattleValue<PositionChangeLogs>();

        public static void AddLog(PositionChangeLog log)
        {
            Instance?.Add(log);
        }

        public static void AddLog(BattleChar bc, int oldPos, int newPos)
        {
            Instance?.AddOneLog(bc, oldPos, newPos);
        }

        public static void NewInd()
        {
            Instance?.IncInd();
        }

        private void AddOneLog(BattleChar bc, int oldPos, int newPos)
        {
            Add(new PositionChangeLog(bc, oldPos, newPos, Ind));
        }

        private void IncInd()
        {
            Ind++;
        }
    }

    public class PositionChangeLog
    {
        public BattleChar BC;
        public int OldPos;
        public int NewPos;
        public int Turn;
        public int Ind;

        public PositionChangeLog(BattleChar bc, int oldPos, int newPos, int ind)
        {
            BC = bc;
            OldPos = oldPos;
            NewPos = newPos;
            Turn = bc.BattleInfo.TurnNum;
            Ind = ind;
        }
    }
}
