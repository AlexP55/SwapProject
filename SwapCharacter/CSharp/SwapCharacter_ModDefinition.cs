using System.Collections.Generic;
using ChronoArkMod;
using System.Linq;
using UnityEngine;
using ChronoArkMod.ModData;
using ChronoArkMod.Template;
using Dialogical;
using System;

namespace SwapCharacter
{
    public class SwapCharacter_ModDefinition:ModDefinition
    {
        // Tutorial trigger IReturn
        public override List<object> BattleSystem_ModIReturn()
        {
            var list = base.BattleSystem_ModIReturn();
            list.Add(new SwapTutorialTrigger());
            return list;
        }

        public override List<object> BattleChar_ModIReturn(Type type, BattleChar battleChar)
        {
            var list = base.BattleChar_ModIReturn(type, battleChar);
            list.Add(new ParticleRecorder());
            return list;
        }
    }

    /// <summary>
    /// Add dialogue data to the character JSON 
    /// </summary>
    public class SwapCharCharacter: CustomCharacterGDE<SwapCharacter_ModDefinition>
    {
        public override ModGDEInfo.LoadingType GetLoadingType()
        {
            return ModGDEInfo.LoadingType.Add;
        }

        public override string Key()
        {
            return ModItemKeys.Character_SwapChar;
        }

        public override void SetValue()
        {
            Dialogue_NomalGiftTalk = GetDialoguePath(ModDialogueKeys.FS_SwapChar_Gift_Normal);
            Dialogue_GoodGiftTalks = new List<string>
            {
                GetDialoguePath(ModDialogueKeys.FS_SwapChar_Gift_Violin),
                GetDialoguePath(ModDialogueKeys.FS_SwapChar_Gift_Knife),
                GetDialoguePath(ModDialogueKeys.FS_SwapChar_Gift_Tool)
            };
            Dialogue_FriendShipLVTalks = new List<string>
            {
                GetDialoguePath(ModDialogueKeys.ComingSoon),
                GetDialoguePath(ModDialogueKeys.ComingSoon),
                GetDialoguePath(ModDialogueKeys.ComingSoon)
            };
            Dialogue_LastStory = GetDialoguePath(ModDialogueKeys.LastStory_SwapChar);
            Dialogue_TrueEnd = new List<string>
            {
                GetDialoguePath(ModDialogueKeys.ComingSoon),
                GetDialoguePath(ModDialogueKeys.ComingSoon)
            };
            Dialogue_Final = GetDialoguePath(ModDialogueKeys.ComingSoon);
        }

        public static string GetDialoguePath(string key)
        {
            string path = "Assets/ModAssets/ModDialogues/" + key + ".asset";
            var modInfo = SwapCharacter_Plugin.ThisMod;
            return modInfo.assetInfo.ObjectFromAsset<DialogueTree>(modInfo.DefaultAssetBundlePath, path);
        }
    }

    /// <summary>
    /// Tutorial trigger passive
    /// </summary>
    public class SwapTutorialTrigger : IP_BattleStart_Ones, IP_BuffAdd
    {
        public static List<string> SwapBuffs = new List<string> {
            ModItemKeys.Buff_B_SwapChar_2_S,
            ModItemKeys.Buff_B_SwapChar_9_T,
            ModItemKeys.Buff_B_SwapChar_11_T,
            ModItemKeys.Buff_B_SwapChar_Rare_1_T
        };

        public static bool IsSwapBuff(Buff buff) => SwapBuffs.Contains(buff.BuffData.Key);

        public void BattleStart(BattleSystem Ins)
        {
            if (SwapCharacter_Plugin.SwapTutorial && !Ins.NoTutorial && 
                Ins.AllyList.Any(ally => ally.Info.Passive is P_SwapChar))
            {
                InitTutorial("Assets/ModAssets/SwapTutorial.asset");
                SwapCharacter_Plugin.SwapTutorial = false;
            }
        }

        public void Buffadded(BattleChar BuffUser, BattleChar BuffTaker, Buff addedbuff)
        {
            if (SwapCharacter_Plugin.BuffTutorial && !BattleSystem.instance.NoTutorial &&
                BuffTaker.Info.Ally && IsSwapBuff(addedbuff))
            {
                if (BattleSystem.instance.AllyList.Any(bc => bc.Buffs.Any(
                    buff => IsSwapBuff(buff) && (buff.BuffData.Key != addedbuff.BuffData.Key || buff.BChar != BuffTaker))))
                {
                    InitTutorial("Assets/ModAssets/BuffTutorial.asset");
                    SwapCharacter_Plugin.BuffTutorial = false;
                }
            }
        }

        public static void InitTutorial(string tutorialPath)
        {
            var tutorial = SwapExt.GetAssets<Tutorial>(tutorialPath);
            if (tutorial == null)
            {
                Debug.Log("Tutorial not found");
                return;
            }
            var obj = UIManager.InstantiateActiveAddressable(UIManager.inst.AR_TutorialUI, AddressableLoadManager.ManageType.Stage).GetComponent<TutorialObject>();
            obj.transform.Find("Window").localPosition += new Vector3(0, 100, 0);
            obj.transform.Find("Window/VideoImage").localPosition += new Vector3(0, 60, 0);
            obj.transform.Find("Window/TextPos").localPosition += new Vector3(0, 120, 0);
            obj.gameObject.AddComponent<TutorialLocalizer>().Init(obj);
            obj.Init(tutorial);
        }
    }

    /// <summary>
    /// Particle record passive
    /// </summary>
    public class ParticleRecorder : IP_ParticleOut_Before
    {
        public void ParticleOut_Before(Skill SkillD, List<BattleChar> Targets)
        {
            if (SkillD.IsDamage && Targets.Any(bc => bc.Info.Ally)) ParticleLogs.AddLog(SkillD.Master, SkillD, Targets);
        }
    }
}