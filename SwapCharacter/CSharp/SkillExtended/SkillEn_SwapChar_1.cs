using GameDataEditor;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Ignore Taunt
    /// Frontal Assault: Damage +30%
    /// </summary>
    public class SkillEn_SwapChar_1 : Skill_Extended, IP_DamageChange
    {
        public override bool CanSkillEnforce(Skill MainSkill)
        {
            return MainSkill.IsDamage && MainSkill.TargetTypeKey == GDEItemKeys.s_targettype_enemy;
        }

        public int DamageChange(Skill SkillD, BattleChar Target, int Damage, ref bool Cri, bool View)
        {
            if (SwapUtils.SamePosition(BChar as BattleAlly, Target as BattleEnemy))
            {
                return Damage + (int)Misc.PerToNum(Damage, 30);
            }
            return Damage;
        }

        public override void Init()
        {
            base.Init();
            OnePassive = true;
            IgnoreTaunt = true;
        }
    }
}