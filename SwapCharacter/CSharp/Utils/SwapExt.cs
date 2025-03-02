using System;
using System.Collections.Generic;
using System.Linq;
using ChronoArkMod;
using GameDataEditor;
using HarmonyLib;
using UnityEngine.UI;

namespace SwapCharacter
{
    public static class SwapExt
    {
        public static string GetTranslation(this string key)
        {
            try
            {
                return ModManager.getModInfo(SwapCharacter_Plugin.modname).localizationInfo.
                    SystemLocalizationUpdate(SwapCharacter_Plugin.modname + "/" + key);
            }
            catch
            {
            }
            return key;
        }

        public static T GetOrAddCustomValue<T>(this TempSaveData save) where T : CustomValue
        {
            var result = save.GetCustomValue<T>();
            if (result == null)
            {
                result = Activator.CreateInstance<T>();
                save.AddCustomValue(result);
            }
            return result;
        }

        public static void GetSpriteByPath(this Image img, string path)
        {
            var address = SwapCharacter_Plugin.ThisMod.assetInfo.ImageFromFile(path);
            img.GetSpriteByAddress(address);
        }

        public static void GetSpriteByAddress(this Image img, string address, 
            AddressableLoadManager.ManageType type = AddressableLoadManager.ManageType.Stage)
        {
            AddressableLoadManager.LoadAsyncAction(address, type, img);
        }

        public static T GetAssets<T>(string path, string assetBundlePatch = null) where T : UnityEngine.Object
        {
            var mod = SwapCharacter_Plugin.ThisMod;
            if (string.IsNullOrEmpty(assetBundlePatch)) assetBundlePatch = mod.DefaultAssetBundlePath;
            var address = mod.assetInfo.ObjectFromAsset<T>(assetBundlePatch, path);
            return AddressableLoadManager.LoadAddressableAsset<T>(address);
        }

        public static T GetAssetsAsync<T>(string path, string assetBundlePatch = null, 
            AddressableLoadManager.ManageType type = AddressableLoadManager.ManageType.Stage) where T : UnityEngine.Object
        {
            var mod = SwapCharacter_Plugin.ThisMod;
            if (string.IsNullOrEmpty(assetBundlePatch)) assetBundlePatch = mod.DefaultAssetBundlePath;
            var address = mod.assetInfo.ObjectFromAsset<T>(assetBundlePatch, path);
            return AddressableLoadManager.LoadAsyncCompletion<T>(address, type);
        }

        public static bool CanSelect(this BattleChar bc)
        {
            return bc != null && !bc.IsDead && !bc.GetStat.Vanish && !bc.Dummy;
        }

        public static List<BattleChar> CharsViewable(this BattleTeam team)
        {
            return team.Chars.FindAll(bc => bc.UI.gameObject.activeSelf);
        }

        public static List<T> CharsFilterDummy<T>(this List<T> chars) where T : BattleChar
        {
            return chars.FindAll(ally => ally.Info.KeyData != ModItemKeys.Character_DummySummon);
        }

        public static List<BattleAlly> AlliesFilterTemp(this List<BattleAlly> allies)
        {
            return allies.FindAll(ally => ally.Info.GetData.Role.Key != GDEItemKeys.CharRole_Role_TempAlly);
        }

        public static void AddParryBuff(this BattleChar battleChar, Skill skill)
        {
            if (battleChar.BuffAdd(ModItemKeys.Buff_B_SwapChar_Parry, battleChar) is B_SwapChar_Parry parryBuff)
            {
                parryBuff.skillParry = skill;
            }
        }

        public static List<CastingSkill> GetBeforeSkills(this CastingSkill skill)
        {
            var skills = skill.Usestate.GetCastingSkills();
            var ind = skills.IndexOf(skill);
            if (ind < 0) return skills;
            return skills.GetRange(0, ind);
        }

        public static List<CastingSkill> GetCastingSkills(this BattleChar bc)
        {
            if (BattleSystem.instance == null) return new List<CastingSkill>();
            return BattleSystem.instance.EnemyCastSkills.Where(s => s.Usestate == bc).ToList();
        }

        /// <summary>
        /// This TargetSelect use the below AggroReturn
        /// </summary>
        public static List<BattleChar> TargetSelectCasting(this AI ai, CastingSkill skill)
        {
            return TargetPatch.TargetSelectCasting(ai, skill);
        }

        /// <summary>
        /// This AggroReturn will look at all casting skills before to decide the target
        /// If there are n stacks of taunted debuff, only first n skills will be taunted
        /// </summary>
        public static BattleChar AggroReturnCasting(this AI ai, BattleAlly Except = null, CastingSkill skill = null)
        {
            var beforeSkills = skill != null ? skill.GetBeforeSkills() : ai.BChar.GetCastingSkills();
            if (beforeSkills.Count <= 0) return ai.AgrroReturn(Except);
            var tauntBuffs = ai.BChar.Buffs.FindAll(buff => !buff.DestroyBuff && buff is B_Taunt);
            if (tauntBuffs.Count <= 0) return TargetPatch.AggroReturnNoTaunt(ai, Except);
            var tempBuffs = tauntBuffs.Select(buff => buff.StackInfo.ToList()).ToList();
            foreach (var cast in beforeSkills.Where(sk => sk.skill.IsDamage))
            {
                var targets = cast.AITargetReturn();
                foreach (var stacks in tempBuffs)
                {
                    if (stacks.Count > 0 && targets.Contains(stacks[0].UseState)) stacks.RemoveAt(0);
                }
            }
            var effectBuff = tempBuffs.FirstOrDefault(stacks => stacks.Count > 0);
            if (effectBuff != null && Except != effectBuff[0].UseState && !effectBuff[0].UseState.GetStat.Vanish)
            {
                return effectBuff[0].UseState;
            }
            return TargetPatch.AggroReturnNoTaunt(ai, Except);
        }

        /// <summary>
        /// This TargetReturn only consider skills
        /// </summary>
        public static List<BattleChar> AITargetReturn(this CastingSkill Cast)
        {
            foreach (Skill_Extended skill_Extended in Cast.skill.AllExtendeds)
            {
                List<BattleChar> list = (skill_Extended as IP_TargetAI)?.TargetAI(Cast.Usestate as BattleEnemy);
                if (list != null && list.Count > 0) return list;
            }
            if (Cast.skill.AllExtendeds.Any(ex => ex.EnemyTargetAIOnly))
            {
                return new List<BattleChar>();
            }
            if (Cast.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_all_enemy || Cast.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_random_enemy)
            {
                return BattleSystem.instance.AllyTeam.AliveChars;
            }
            if (Cast.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_all_ally)
            {
                return BattleSystem.instance.EnemyTeam.AliveChars;
            }
            if (Cast.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_all)
            {
                return BattleSystem.instance.GetGlobalChar();
            }
            if (Cast.TargetPlus != null)
            {
                return new List<BattleChar> { Cast.Target, Cast.TargetPlus };
            }
            if (Cast.Target != null)
            {
                return new List<BattleChar> { Cast.Target };
            }
            return new List<BattleChar>();
        }

        public static T GetField<T>(this object obj, string fieldName)
        {
            return Traverse.Create(obj).Field<T>(fieldName).Value;
        }

        public static void SetField<T>(this object obj, string fieldName, T value)
        {
            Traverse.Create(obj).Field<T>(fieldName).Value = value;
        }
    }
}
