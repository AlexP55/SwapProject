using System.Linq;
using Dialogical;
using GameDataEditor;
using HarmonyLib;
using Spine.Unity;
using UnityEngine;

namespace SwapCharacter
{
    [HarmonyPatch]
    public class SpinePatch
    {
        public const string SpinePath = "swapcharspine";

        // Main town sprite
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ArkCode), "Start")]
        public static void ArkCode_Start_Patch(ArkCode __instance)
        {
            GameObject gameObject = __instance.UnlockMainNPCList.FirstOrDefault((GameObject target) => target.name == ModItemKeys.Character_SwapChar);
            if (gameObject == null)
            {
                GameObject template = __instance.UnlockMainNPCList[7];
                gameObject = Object.Instantiate(template, template.transform.parent);
                gameObject.name = ModItemKeys.Character_SwapChar;
                InitSkeletonAnimation(gameObject.GetComponentInChildren<SkeletonAnimation>());
                gameObject.GetComponent<Dialogue>().tree = GetDialogue(ModDialogueKeys.Ark_SwapChar);
                gameObject.transform.localPosition = new Vector3(1.1f, 10.6f, 0f);
                __instance.UnlockMainNPCList.Add(gameObject);
            }
            gameObject.SetActive(true);
        }

        // Archive sprite
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RootStoryScript), "PartySpine")]
        public static bool RootStoryScript_PartySpine_Patch(RootStoryScript __instance, int Num, string KeyData)
        {
            if (KeyData == ModItemKeys.Character_SwapChar)
            {
                SaveManager.NowData.statistics.GetCharData(KeyData);
                InitSkeletonAnimation(__instance.PartyMember_SK[Num]);
                __instance.PartyMember_EO[Num].TargetEvent.AddListener(delegate ()
                {
                    __instance.CallSkipNotice();
                });
                __instance.PartyMember_Dial[Num].tree = GetDialogue(ModDialogueKeys.ComingSoon);
                __instance.PartyMember_Dial[Num].DialogueEnd.AddListener(delegate ()
                {
                    __instance.DialogueEndDel();
                });
                __instance.PartyMember[Num].gameObject.SetActive(true);
                return false;
            }
            return true;
        }

        // Clock tower sprite
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClockTowerMap), "PartySpine")]
        public static bool ClockTowerMap_PartySpine_Patch(ClockTowerMap __instance, int Num, string KeyData)
        {
            if (KeyData == ModItemKeys.Character_SwapChar)
            {
                InitSkeletonAnimation(__instance.PartyMember_SK[Num]);
                __instance.PartyMember_Dial[Num].tree = GetDialogue(ModDialogueKeys.ClockTower_SwapChar);
                __instance.PartyMember[Num].gameObject.SetActive(true);
                return false;
            }
            return true;
        }

        // King room sprite
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KingRoomScript), "PartySpine")]
        public static bool KingRoomScript_PartySpine_Patch(KingRoomScript __instance, int Num, string KeyData)
        {
            if (KeyData == ModItemKeys.Character_SwapChar)
            {
                InitSkeletonAnimation(__instance.PartyMember_SK[Num]);
                __instance.PartyMember_Dial[Num].tree = GetDialogue(ModDialogueKeys.KingNPC_SwapChar);
                __instance.PartyMember[Num].gameObject.SetActive(true);
                return false;
            }
            return true;
        }

        // Master room (before Azar) sprite
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MasterSDMap), "PartySpine")]
        public static bool MasterSDMap_PartySpine_Patch(MasterSDMap __instance, int Num, string KeyData)
        {
            if (KeyData == ModItemKeys.Character_SwapChar)
            {
                Statistics_Character charData = SaveManager.NowData.statistics.GetCharData(KeyData);
                InitSkeletonAnimation(__instance.PartyMember_SK[Num]);
                if (__instance.CheckCrimsonDialogue(KeyData))
                {
                    __instance.PartyMember_Dial[Num].tree = GetGameDialogue("Assets/Dialogical/Dialogues/NeedMoreFriendShip.asset");
                    __instance.PartyMember_DialEnd[Num].tree = GetGameDialogue("Assets/Dialogical/Dialogues/NeedMoreFriendShip.asset");
                    __instance.PartyTalkMark[Num].SetActive(false);
                }
                else
                {
                    __instance.PartyMember_Dial[Num].tree = GetDialogue(ModDialogueKeys.ComingSoon);
                    __instance.PartyMember_Dial[Num].IsSkip = true;
                    __instance.PartyMember_Dial[Num].DialogueEnd.AddListener(delegate ()
                    {
                        __instance.PartyTalkMark[Num].SetActive(false);
                        UIManager.InstantiateActiveAddressable(UIManager.inst.CompleteUI, AddressableLoadManager.ManageType.Stage).GetComponent<FriendShipCompleteUI>().MaxLevel(ModItemKeys.Character_SwapChar);
                        PlayData.IsSkip = false;
                    });
                    __instance.PartyMember_DialEnd[Num].tree = GetDialogue(ModDialogueKeys.ComingSoon);
                    __instance.PartyTalkMark[Num].SetActive(charData.FriendshipLV != 4);
                }
                __instance.PartyMember[Num].gameObject.SetActive(true);
                return false;
            }
            return true;
        }

        // Master room (before first PM) sprite
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MasterSDMap2), "PartySpine")]
        public static bool MasterSDMap2_PartySpine_Patch(MasterSDMap2 __instance, int Num, string KeyData)
        {
            if (KeyData == ModItemKeys.Character_SwapChar)
            {
                SaveManager.NowData.statistics.GetCharData(KeyData);
                InitSkeletonAnimation(__instance.PartyMember_SK[Num]);
                if (__instance.CheckCrimsonDialogue(KeyData))
                {
                    __instance.PartyMember_Dial[Num].tree = GetGameDialogue("Assets/Dialogical/Dialogues/NeedMoreFriendShip.asset");
                }
                else
                {
                    __instance.PartyMember_Dial[Num].tree = GetDialogue(ModDialogueKeys.ComingSoon);
                }
                __instance.PartyMember[Num].gameObject.SetActive(true);
                return false;
            }
            return true;
        }

        // Master room (before second PM) sprite
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MasterSDMap3), "PartySpine")]
        public static bool MasterSDMap3_PartySpine_Patch(MasterSDMap3 __instance, int Num, string KeyData)
        {
            if (KeyData == ModItemKeys.Character_SwapChar)
            {
                SaveManager.NowData.statistics.GetCharData(KeyData);
                InitSkeletonAnimation(__instance.PartyMember_SK[Num]);
                if (__instance.CheckCrimsonDialogue(KeyData))
                {
                    __instance.PartyMember_Dial[Num].ParameterStr = new GDECharacterData(KeyData).name;
                    __instance.PartyMember_Dial[Num].tree = GetGameDialogue("Assets/Dialogical/Dialogues/NeedMoreFriendShip.asset");
                    __instance.PartyMember_Dial[Num].DialogueEnd.RemoveAllListeners();
                }
                else
                {
                    __instance.PartyMember_Dial[Num].tree = GetDialogue(ModDialogueKeys.ComingSoon);
                    __instance.PartyMember_Dial[Num].DialogueEnd.AddListener(delegate ()
                    {
                        PlayData.IsSkip = false;
                    });
                }
                __instance.PartyMember[Num].gameObject.SetActive(true);
                return false;
            }
            return true;
        }

        public static SkeletonDataAsset GetNowSkeletonData()
        {
            string assetPath = "Assets/Sprites/Chibi 1/Chibi 1_SkeletonData.asset";
            GDECharacter_SkinData skinData = CharacterSkinData.GetSkinData(ModItemKeys.Character_SwapChar);
            if (skinData != null)
            {
                if (skinData.Key == ModItemKeys.Character_Skin_SwapChar_skin1)
                {
                    assetPath = "Assets/Sprites/Chibi 2/Chibi 2_SkeletonData.asset";
                }
                else if (skinData.Key == ModItemKeys.Character_Skin_SwapChar_skin2)
                {
                    assetPath = "Assets/Sprites/Chibi 3/Chibi 3_SkeletonData.asset";
                }
            }
            return SwapExt.GetAssetsAsync<SkeletonDataAsset>(assetPath, SpinePath);
        }

        public static void InitSkeletonAnimation(SkeletonAnimation animation)
        {
            animation.skeletonDataAsset = GetNowSkeletonData();
            animation.initialSkinName = "default";
            animation.Initialize(true);
            animation.loop = true;
            animation.AnimationName = "Idle";
        }

        public static DialogueTree GetDialogue(string key)
        {
            string path = "Assets/ModAssets/ModDialogues/" + key + ".asset";
            return SwapExt.GetAssetsAsync<DialogueTree>(path);
        }

        public static DialogueTree GetGameDialogue(string path)
        {
            return AddressableLoadManager.LoadAsyncCompletion<DialogueTree>(path, AddressableLoadManager.ManageType.Stage);
        }
    }
}
