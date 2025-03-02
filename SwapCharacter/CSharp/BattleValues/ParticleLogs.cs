using System.Collections.Generic;
using System.Linq;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Battle value for storing the skill particle from enemies
    /// </summary>
    public class ParticleLogs : List<BattleLog>
    {
        public static ParticleLogs Instance => BattleSystem.instance?.GetOrAddBattleValue<ParticleLogs>();

        public static void AddLog(BattleLog log)
        {
            Instance?.Add(log);
        }

        public static void AddLog(BattleChar bc, Skill skill, List<BattleChar> targets)
        {
            Instance?.Add(new BattleLog { 
                UsedSkill = skill,
                WhoUse = bc,
                Targets = targets?.ToList(),
                Turn = bc.BattleInfo.TurnNum
            });
        }
    }
}
