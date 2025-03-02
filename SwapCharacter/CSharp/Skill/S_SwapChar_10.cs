using System.Collections.Generic;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Defensive Formation
    /// Randomly remove a debuff from both target and user, then swap with the target.
    /// Generate barrier number equal to a% max HP.
    /// </summary>
    public class S_SwapChar_10:Skill_Extended
    {
        public override string DescExtended(string desc)
        {
            return base.DescExtended(desc).Replace("&a", B_SwapChar_10_S.ratio.ToString());
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            RemoveRandomDot(Targets[0]);
            RemoveRandomDot(BChar);
            var p = AddressableLoadManager.Instantiate(MySkill.MySkill.Particle_Path, AddressableLoadManager.ManageType.Battle);
            p.transform.position = BChar.GetPos();
            BattleSystem.DelayInput(SwapUtils.SwapChars(BChar as BattleAlly, Targets[0] as BattleAlly));
        }

        public void RemoveRandomDot(BattleChar bc)
        {
            List<Buff> buffs = bc.GetBuffs(BattleChar.GETBUFFTYPE.ALLDEBUFF, true, false);
            buffs.RemoveAll(buff => buff.DestroyBuff);
            if (buffs.Count >= 1)
            {
                buffs.Random(BChar.GetRandomClass().Main).SelfDestroy();
            }
        }
    }
}