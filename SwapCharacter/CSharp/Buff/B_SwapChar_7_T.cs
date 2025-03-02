using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using GameDataEditor;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Protection Hologram
    /// When receiving damage higher than barrier of this buff, nullify the damage and remove the buff.
    /// You can click the buff icon to swapthis ally with another ally (the first use is free, after that costs 1 mana).
    /// </summary>
    public class B_SwapChar_7_T:Buff, IP_DamageTakeChange
    {
        public const float ratio = 67f;
        public bool free = true;

        public int DamageTakeChange(BattleChar Hit, BattleChar User, int Dmg, bool Cri, bool NODEF = false, bool NOEFFECT = false, bool Preview = false)
        {
            if (!NODEF && Dmg > BarrierHP)
            {
                if (!Preview)
                {
                    SelfDestroy();
                }
                return 0;
            }
            return Dmg;
        }

        public override void Init()
        {
            base.Init();
            BarrierHP += (int)Misc.PerToNum(Usestate_L.GetStat.maxhp, ratio);
            PlusStat.dod = -100;
            PlusStat.RES_CC = 100;
            PlusStat.RES_DOT = 30;
        }

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            Button button = BuffIcon.AddComponent<Button>();
            button.onClick.AddListener(SwapSelect);
        }

        public bool CanSwap()
        {
            return free || BattleSystem.instance.AllyTeam.AP > 0;
        }

        private void SwapSelect()
        {
            MasterAudio.PlaySound("WaitButton");
            if (CanSwap())
            {
                var list = new List<Skill>();
                bool hasTarget = false;
                foreach (var ally in BattleSystem.instance.AllyTeam.CharsViewable())
                {
                    var skill = Skill.TempSkill(ModItemKeys.Skill_S_SwapChar_Active, ally, ally.MyTeam);
                    var extended = skill.ExtendedFind<S_SwapChar_Active>();
                    if (extended != null)
                    {
                        extended.target = BChar as BattleAlly;
                        if (extended.CanSwap()) hasTarget = true;
                    }
                    list.Add(skill);
                }
                if (hasTarget)
                {
                    var text = ModLocalization.SwapCharacterSelectSwapCostText.Replace("&a", free ? "0" : "1");
                    BattleSystem.DelayInputAfter(
                        BattleSystem.I_OtherSkillSelect(list, new SkillButton.SkillClickDel(Del), text, true, false));
                }
            }
        }

        private void Del(SkillButton MyButton)
        {
            if (MyButton.Myskill.Master is BattleAlly ally && BChar is BattleAlly me)
            {
                BattleSystem.DelayInput(DelSwap(me, ally));
            }
        }

        private IEnumerator DelSwap(BattleAlly ally1, BattleAlly ally2)
        {
            if (CanSwap())
            {
                if (free) free = false;
                else BattleSystem.instance.AllyTeam.AP--;
                var path = new GDESkillData(ModItemKeys.Skill_S_SwapChar_Swap).Particle_Path;
                var p1 = AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.Battle);
                p1.transform.position = ally1.GetPos();
                var p2 = AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.Battle);
                p2.transform.position = ally2.GetPos();
                yield return SwapUtils.SwapChars(ally1, ally2);
            }
            yield break;
        }
    }
}