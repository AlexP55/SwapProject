using System.Collections;
using GameDataEditor;
using HarmonyLib;

namespace SwapCharacter
{
    [HarmonyPatch]
    public class SwapCharPatch
    {
        // Add text for Karela
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SkillExtended_S_S_TheLight_P_1), nameof(SkillExtended_S_S_TheLight_P_1.AfterText))]
        private static IEnumerator KaraelaTextPatch(IEnumerator result, SkillExtended_S_S_TheLight_P_1 __instance)
        {
            if (ModItemKeys.Character_SwapChar == __instance.BChar.Info.KeyData)
            {
                BattleSystem.instance.StartCoroutine(BattleText.InstBattleTextAlly_Co(__instance.BChar, new GDESpecialKeyData(ModItemKeys.SpecialKey_SPK_TheLight_Power_SwapChar).Name));
            }
            while (result.MoveNext())
            {
                yield return result.Current;
            }
            yield break;
        }
    }
}
