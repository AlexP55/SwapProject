using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Hold the Line!
    /// When an enemy targets an ally, swap with the ally and heal by a (c% max HP).
    /// When targeted by an enemy, attack before with b damage and remove one stack.
    /// Can be toggled by clicking the buff icon.
    /// </summary>
    public class B_SwapChar_Rare_1_T:Buff, IP_TargetedAlly
    {
        public static string CounterSkill = ModItemKeys.Skill_S_SwapChar_Rare_1_Counter;
        public static float HealRatio = 15f;
        public bool on = true;

        public override string DescExtended()
        {
            Skill skill = Skill.TempSkill(CounterSkill, BChar, BChar.MyTeam);
            return base.DescExtended().Replace("&a", ((int)Misc.PerToNum(BChar.GetStat.maxhp, HealRatio)).ToString()).
                Replace("&b", skill.TargetDamage.ToString()).Replace("&c", HealRatio.ToString());
        }

        public IEnumerator Targeted(BattleChar Attacker, List<BattleChar> SaveTargets, Skill skill)
        {
            if (!on) yield break;
            if (SaveTargets.Count > 0 && !SaveTargets.Contains(BChar))
            {
                yield return SwapUtils.SwapChars(BChar as BattleAlly, SaveTargets[0] as BattleAlly);
                SaveTargets[0] = BChar;
                BChar.Heal(BChar, (int)Misc.PerToNum(BChar.GetStat.maxhp, HealRatio), false);
            }
            if (SaveTargets.Contains(BChar))
            {
                BChar.AddParryBuff(skill);
                Skill counter = Skill.TempSkill(CounterSkill, BChar, BChar.MyTeam);
                counter.PlusHit = true;
                counter.FreeUse = true;
                BChar.ParticleOut(counter, Attacker);
                SelfStackDestroy();
                yield return new WaitForSeconds(0.2f);
            }
            yield break;
        }

        public override void BuffOneAwake()
        {
            base.BuffOneAwake();
            if (BuffIcon.GetComponent<Button>() == null)
            {
                Button button = BuffIcon.AddComponent<Button>();
                button.onClick.AddListener(ToggleStatus);
            }
            BattleSystem.instance.StartCoroutine(SetBEDel());
        }

        public void ToggleStatus()
        {
            MasterAudio.PlaySound("WaitButton");
            on = !on;
            if (on)
            {
                Sprite.GetSpriteByAddress(BuffData.Icon_Path);
                BE?.gameObject?.SetActive(true);
            }
            else
            {
                var key = BuffData.Key;
                var path = key + "/" + key + "_Icon_Off.png";
                Sprite.GetSpriteByPath(path);
                BE?.gameObject?.SetActive(false);
            }
        }

        private IEnumerator SetBEDel()
        {
            yield return new WaitUntil(() => BE != null);
            BE.transform.localScale = new Vector3(40f, 40f, 1f);
            BE.transform.localPosition = new Vector3(0f, 150f, 0f);
            yield break;
        }
    }
}