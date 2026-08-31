using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;

namespace MTCustomScripts.Patches;

internal class Modular_SetupModular
{
    [HarmonyPatch(typeof(ModularSA), nameof(ModularSA.SetupModular))]
	[HarmonyPrefix]
	private static void Prefix_ModularSA_SetupModular(string instructions, ModularSA __instance)
    {
        MainClass.Logg.LogWarning("SetUpModular Patch ran");
        instructions = MainClass.sWhitespace.Replace(instructions, "");
        string[] batches = instructions.Split('/');
        // bool luaFound = false;

        for (int i = 0; i < batches.Length; i++) {
            string batch = batches[i];
            // if (MainClass.logEnabled) MainClass.Logg.LogInfo("MT's PATCH: batch " + i.ToString() + ": " + batch);
            if (batch.StartsWith("TIMING:")) {
                string timingArg = batch.Remove(0, 7);
                string[] circles = timingArg.Split(__instance.parenthesisSeparator);
                string circle_0 = circles[0];
                if (MainClass.timingDict.TryGetValue(circle_0, out int value)) __instance.activationTiming = value;
                // if (activationTiming == FakePowerPatches.actevent_FakePower) EXPECTED = true;

                if (circles.Length > 1)
                {
                    string hitArgs = circles[1];
                    // if (hitArgs.Contains("Head")) _onlyHeads = true;
                    // else if (hitArgs.Contains("Tail")) _onlyTails = true;

                    // if (hitArgs.Contains("NoCrit")) _onlyNonCrit = true;
                    // else if (hitArgs.Contains("Crit")) _onlyCrit = true;
                    // BUFF_UNIQUE_KEYWORD parsedKeyword = CustomBuffs.ParseBuffUniqueKeyword(hitArgs);
                    if (!Il2CppSystem.Enum.TryParse(hitArgs, out BUFF_UNIQUE_KEYWORD parsedKeyword)) parsedKeyword = BUFF_UNIQUE_KEYWORD.None;
                    __instance.keywordTrigger = parsedKeyword;
                    MTCustomScripts.Main.Instance.keywordTriggerDict[__instance.Pointer.ToInt64()] = parsedKeyword;
                    MainClass.Logg.LogInfo($"Parsed keyword trigger for OnGainBuff: {parsedKeyword.ToString()}; Input: {hitArgs}");

					if (!bool.TryParse(hitArgs, out bool result)) result = false;
					MTCustomScripts.Main.Instance.equipdefense_refreshslotview = result;
					// }
					// if (circle_0 == "SpecialAction")
					// {
					//     MainClass.Logg.LogInfo("SpecialAction with no parsed key, default to LeftControl");
				}
                else if (circle_0 == "OnGainBuff")
                {
                    MainClass.Logg.LogInfo("OnGainBuff with no keyword detected, default to None");
                    MTCustomScripts.Main.Instance.keywordTriggerDict[__instance.Pointer.ToInt64()] = BUFF_UNIQUE_KEYWORD.None;
                }
            }
        }
    }
}
