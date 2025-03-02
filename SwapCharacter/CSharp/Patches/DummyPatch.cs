using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SwapCharacter
{
    [HarmonyPatch]
    public class DummyPatch
    {
        // Remove Hologram from skill collections
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(SKillCollection), "Init")]
        static IEnumerable<CodeInstruction> RemoveDummyCollection(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (i - 3 >= 0 &&
                    codes[i - 3].opcode == OpCodes.Ldloc_3 &&
                    codes[i - 2].opcode == OpCodes.Ldsfld &&
                    codes[i - 1].opcode == OpCodes.Call &&
                    (codes[i].opcode == OpCodes.Brtrue || codes[i].opcode == OpCodes.Brtrue_S))
                {
                    yield return codes[i - 3];
                    yield return new CodeInstruction(OpCodes.Ldsfld,
                        AccessTools.Field(typeof(ModItemKeys), nameof(ModItemKeys.Character_DummySummon)));
                    yield return codes[i - 1];
                    yield return codes[i];
                }
            }
        }

        // Remove temp allies from character views
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(CharStatV4), nameof(CharStatV4.Init))]
        static IEnumerable<CodeInstruction> RemoveDummyCharStat(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Is(OpCodes.Call, AccessTools.PropertyGetter(typeof(PlayData), nameof(PlayData.Battleallys))))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SwapExt), nameof(SwapExt.AlliesFilterTemp)));
                }
            }
        }

        // Hologram cannot be targeted by some boss skills
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(S_S4_King_P1_0), nameof(S_S4_King_P1_0.SkillUseSingle))]
        [HarmonyPatch(typeof(S_ProgramMaster_1), nameof(S_ProgramMaster_1.SkillUseSingle))]
        [HarmonyPatch(typeof(S_TheDealer_0), nameof(S_TheDealer_0.SkillUseSingle))]
        [HarmonyPatch(typeof(P_ProgramMaster_2nd), nameof(P_ProgramMaster_2nd.EnemyAttackScene), MethodType.Enumerator)]
        static IEnumerable<CodeInstruction> RemoveDummyEnemySkill(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Is(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BattleSystem), nameof(BattleSystem.AllyList))) ||
                    instruction.Is(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BattleTeam), nameof(BattleTeam.AliveChars))) ||
                    instruction.Is(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BattleTeam), nameof(BattleTeam.AliveChars_Vanish))))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SwapExt), nameof(SwapExt.CharsFilterDummy), generics: new Type[] { typeof(BattleAlly) }));
                }
            }
        }
    }
}
