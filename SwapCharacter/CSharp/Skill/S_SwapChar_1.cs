using System.Collections.Generic;
using GameDataEditor;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Recon Strike
    /// Gain one charge of Eye of Phalanx (active item).
    /// Frontal Assault: Apply one more stack of "Identified!".
    /// </summary>
    public class S_SwapChar_1:Skill_Extended
    {
        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            P_SwapChar.ChargeActive();
            if (SwapUtils.SamePosition(BChar as BattleAlly, Targets[0] as BattleEnemy))
            {
                MySkill.MySkill.Effect_Target.Buffs.Add(new GDEBuffData(GDEItemKeys.Buff_B_Control_P));
            }
        }
    }
}