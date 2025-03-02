using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SwapCharacter
{
    [HarmonyPatch]
    public class TargetPatch
    {
        // EnemyActionScene: Remove the check of "Identified!" debuff
        // This will stop the dynamical change of target and make the target based on exactly the CastingSkill memory
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BattleSystem), nameof(BattleSystem.EnemyActionScene), MethodType.Enumerator)]
        static IEnumerable<CodeInstruction> EnemyTargetPatch(IEnumerable<CodeInstruction> instructions)
        {
            // this makes enemies as if they are always identified to keep their targets from changing
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Is(OpCodes.Callvirt, AccessTools.Method(typeof(BattleChar), nameof(BattleChar.BuffFind))))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                }
            }
        }

        // AgrroReturn: Remove the check of taunted debuffs and create it as a custom function
        // This custom function returns the targets regardless of taunted debuffs
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(AI), nameof(AI.AgrroReturn))]
        public static BattleChar AggroReturnNoTaunt(AI instance, BattleAlly Except = null)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                    if (instruction.Is(OpCodes.Callvirt, AccessTools.Method(typeof(BattleChar), nameof(BattleChar.BuffFind))))
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    }
                }
            }

            // make compiler happy
            _ = Transpiler(null);
            throw new NotImplementedException("It's a stub");
        }

        // TargetSelect: Replace the original AggroReturn with the new one above and create it as a custom function
        // This custom function will consider the skills in casting list so n stacks of taunted debuffs will only taunt first n skills
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(AI), nameof(AI.TargetSelect))]
        public static List<BattleChar> TargetSelectCasting(AI instance, CastingSkill CSkill)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // this makes enemy skills reflect aggro/taunted based on their existing skills
                foreach (var instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Ldarg_1)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CastingSkill), nameof(CastingSkill.skill)));
                    }
                    else if (instruction.Is(OpCodes.Call, AccessTools.Method(typeof(AI), nameof(AI.AgrroReturn))))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        instruction.operand = AccessTools.Method(typeof(SwapExt), nameof(SwapExt.AggroReturnCasting));
                        yield return instruction;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }

            // make compiler happy
            _ = Transpiler(null);
            throw new NotImplementedException("It's a stub");
        }

        // TargetSelect: Replace the original AggroReturn with the new one
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(AI), nameof(AI.TargetSelect))]
        [HarmonyPatch(typeof(AI_Shiranui), nameof(AI_Shiranui.TargetSelect))]
        static IEnumerable<CodeInstruction> TargetSelectPatch(IEnumerable<CodeInstruction> instructions)
        {
            // this makes enemy skills reflect aggro/taunted based on their existing skills
            foreach (var instruction in instructions)
            {
                if (instruction.Is(OpCodes.Call, AccessTools.Method(typeof(AI), nameof(AI.AgrroReturn))))
                {
                    yield return new CodeInstruction(OpCodes.Ldnull);
                    instruction.operand = AccessTools.Method(typeof(SwapExt), nameof(SwapExt.AggroReturnCasting));
                    yield return instruction;
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        // Any place with TargetSelect: Replace the original TargetSelect with the new one
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(BattleEnemy), nameof(BattleEnemy.init))]
        [HarmonyPatch(typeof(BattleSystem), nameof(BattleSystem.EnemyTurn), MethodType.Enumerator)]
        [HarmonyPatch(typeof(Buff), nameof(Buff.TauntSkill))]
        static IEnumerable<CodeInstruction> SubTargetSelectPatch(IEnumerable<CodeInstruction> instructions)
        {
            // this makes enemy skills retargeting reflect aggro/taunted based on their existing skills
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (i + 1 < codes.Count &&
                    codes[i].Is(OpCodes.Ldfld, AccessTools.Field(typeof(CastingSkill), nameof(CastingSkill.skill))) &&
                    codes[i + 1].Is(OpCodes.Callvirt, AccessTools.Method(typeof(AI), nameof(AI.TargetSelect))))
                {
                    codes[i].opcode = OpCodes.Nop;
                    codes[i].operand = null;
                    codes[i + 1].opcode = OpCodes.Call;
                    codes[i + 1].operand = AccessTools.Method(typeof(SwapExt), nameof(SwapExt.TargetSelectCasting));
                }
                yield return codes[i];
            }
        }
    }
}
