using System.Collections.Generic;
using GameDataEditor;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Swap!
    /// Swap positions of &a and &b.
    /// </summary>
    public class S_SwapChar_Active:Skill_Extended
    {
        public BattleChar target;
        public bool AllowSelf = false;

        public override void Init()
        {
            base.Init();
            CanUseStun = true;
        }

        public bool CanSwap()
        {
            if (!target.CanSelect() || !BChar.CanSelect()) return false;
            if (target == BChar) return false;
            return true;
        }

        public override bool ButtonSelectTerms()
        {
            if (!target.CanSelect() || !BChar.CanSelect()) return false;
            if (!AllowSelf && target == BChar) return false;
            return base.ButtonSelectTerms();
        }

        public override string DescExtended(string desc)
        {
            if (MySkill.Master.Dummy) return new GDEItem_ActiveData(ModItemKeys.Item_Active_Active_SwapChar).Description;
            if (!CanSwap()) return ModLocalization.SwapCharacterNoActionText;
            return base.DescExtended(desc).Replace("&a", MySkill.Master.Info.Name).Replace("&b", target.Info.Name);
        }

        public override void SkillUseSingle(Skill SkillD, List<BattleChar> Targets)
        {
            base.SkillUseSingle(SkillD, Targets);
            target = Targets[0];
            if (target != null)
            {
                var list = new List<Skill>();
                bool hasTarget = false;
                foreach (var ally in BattleSystem.instance.AllyTeam.CharsViewable())
                {
                    var skill = Skill.TempSkill(ModItemKeys.Skill_S_SwapChar_Active, ally, ally.MyTeam);
                    var extended = skill.ExtendedFind<S_SwapChar_Active>();
                    if (extended != null)
                    {
                        extended.target = target;
                        if (extended.CanSwap()) hasTarget = true;
                    }
                    list.Add(skill);
                }
                if (hasTarget)
                {
                    BattleSystem.DelayInput(
                        BattleSystem.I_OtherSkillSelect(list,
                            new SkillButton.SkillClickDel(Del), ModLocalization.SwapCharacterSelectSwapText, false, false));
                }
                else
                {
                    target.BuffAdd(ModItemKeys.Buff_B_SwapChar_Tenacity, BattleSystem.instance.DummyChar);
                }
            }
        }

        private void Del(SkillButton MyButton)
        {
            target.BuffAdd(ModItemKeys.Buff_B_SwapChar_Tenacity, BChar);
            MyButton.Myskill.Master.BuffAdd(ModItemKeys.Buff_B_SwapChar_Tenacity, BChar);
            if (MyButton.Myskill.Master is BattleAlly a1 && target is BattleAlly a2)
            {
                var path = new GDESkillData(ModItemKeys.Skill_S_SwapChar_Swap).Particle_Path;
                var p1 = AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.Battle);
                p1.transform.position = a1.GetPos();
                var p2 = AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.Battle);
                p2.transform.position = a2.GetPos();
                BattleSystem.DelayInput(SwapUtils.SwapChars(a1, a2));
            }
        }
    }
}