using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Drone Support
    /// Position Buff
    /// At the start of a new turn, select to move the buff to a new position.
    /// </summary>
    public class B_EquipDrone_SwapChar:B_SwapStable, IP_PlayerTurn, IP_StatsStable
    {
        public void Turn()
        {
            var list = new List<Skill>();
            bool hasTarget = false;
            foreach (var ally in BattleSystem.instance.AllyTeam.CharsViewable())
            {
                var skill = Skill.TempSkill(ModItemKeys.Skill_S_SwapChar_MoveBuff, ally, ally.MyTeam);
                var extended = skill.ExtendedFind<S_SwapChar_Active>();
                if (extended != null)
                {
                    extended.AllowSelf = true;
                    extended.target = BChar as BattleAlly;
                    if (extended.CanSwap()) hasTarget = true;
                }
                list.Add(skill);
            }
            if (hasTarget)
            {
                BattleSystem.DelayInput(
                    BattleSystem.I_OtherSkillSelect(list,
                        new SkillButton.SkillClickDel(Del), ModLocalization.SwapCharacterSelectMoveText, false, false));
            }
        }

        private void Del(SkillButton MyButton)
        {
            this.MoveTo(MyButton.Myskill.Master);
        }
    }
}