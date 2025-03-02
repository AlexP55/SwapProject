using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SwapCharacter
{
    [HarmonyPatch]
    public class BattlePatch
    {
        // Six sense: apply nonvanishing "Identified!"
        [HarmonyPrefix]
        [HarmonyPatch(typeof(B_Control_P), nameof(B_Control_P.SkillUse))]
        static bool IdentifiedPrefix(B_Control_P __instance)
        {
            if (__instance.BChar.BuffFind(ModItemKeys.Buff_B_RelicSixSense_SwapChar)) return false;
            return true;
        }

        // Fix the issue that list is modified during iteration
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BattleSystem), nameof(BattleSystem.EnemyActionScene), MethodType.Enumerator)]
        static IEnumerable<CodeInstruction> EnemyActionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Is(OpCodes.Call, AccessTools.PropertyGetter(typeof(BattleSystem), nameof(BattleSystem.AllyList))))
                {
                    yield return new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(List<BattleAlly>), new Type[] { typeof(IEnumerable<BattleAlly>) }));
                }
            }
        }
    }
}
