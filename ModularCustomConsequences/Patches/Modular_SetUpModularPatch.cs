using HarmonyLib;
using Lethe.Patches;

namespace ModularSkillScripts.Patches;

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
            if (MainClass.logEnabled) MainClass.Logg.LogInfo("MT's PATCH: batch " + i.ToString() + ": " + batch);
            if (batch.StartsWith("TIMING:")) {
                string timingArg = batch.Remove(0, 7);
                string[] circles = timingArg.Split(__instance.parenthesisSeparator);
                string circle_0 = circles[0];
                if (MainClass.timingDict.ContainsKey(circle_0)) __instance.activationTiming = MainClass.timingDict[circle_0];
                    
                if (circles.Length > 1)
                {
                    string hitArgs = circles[1];
                //     if (hitArgs.Contains("Head"))  _onlyHeads = true;
                //     else if (hitArgs.Contains("Tail")) _onlyTails = true;

                //     if (hitArgs.Contains("NoCrit")) _onlyNonCrit = true;
                //     else if (hitArgs.Contains("Crit")) _onlyCrit = true;

                //     //if (hitArgs.Contains("Win")) _onlyClashWin = true;
                //     //else if (hitArgs.Contains("Lose")) _onlyClashLose = true;

                //     if (!Enum.TryParse(hitArgs, true, out KeyCode parsedKey))
                //     {
                //         parsedKey = KeyCode.LeftControl;
                //     }
                //     SpecialKey = parsedKey;
                //     MainClass.Logg.LogInfo("Parsed key and set to SpecialKey: " + hitArgs);
                    BUFF_UNIQUE_KEYWORD parsedKeyword = CustomBuffs.ParseBuffUniqueKeyword(hitArgs);
                    if (parsedKeyword.ToString() != hitArgs)
                    {
                        parsedKeyword = BUFF_UNIQUE_KEYWORD.None;
                    }
                    MTCustomScripts.Main.Instance.keywordTriggerDict[__instance.Pointer.ToInt64()] = parsedKeyword;
                    MainClass.Logg.LogInfo("Parsed keyword trigger for OnGainBuff: " + parsedKeyword.ToString());
                // }
                // if (circle_0 == "SpecialAction")
                // {
                //     MainClass.Logg.LogInfo("SpecialAction with no parsed key, default to LeftControl");
                }

                if (circle_0 == "OnGainBuff")
                {
                    MTCustomScripts.Main.Instance.keywordTriggerDict[__instance.Pointer.ToInt64()] = BUFF_UNIQUE_KEYWORD.None;
                }
            }
            // else if (batch.StartsWith("LUA:", StringComparison.OrdinalIgnoreCase))
            // {
            //     if (!String.IsNullOrWhiteSpace(modsa_loopString))
            //     {
            //         MainClass.Logg.LogError("LUA cannot be used with LOOP");
            //         return;
            //     }
            //     var luaScriptName = batch.Remove(0, 4);
            //     if (!LuaScript.loadedScripts.TryGetValue(luaScriptName, out modsa_luaScript))
            //     {
            //         MainClass.Logg.LogError("LUA script used but not found: " + luaScriptName);
            //         return;
            //     }
            //     luaFound = true;
            // }
            // else if (batch.StartsWith("LOOP:", StringComparison.OrdinalIgnoreCase))
            // {
            //     if (modsa_luaScript != null)
            //     {
            //         MainClass.Logg.LogError("LOOP cannot be used with LUA");
            //         return;
            //     }
            //     modsa_loopString = batch.Remove(0, 5);
            // }
            // else if (batch.StartsWith("LUAMAIN:", StringComparison.OrdinalIgnoreCase))
            // {
            //     modsa_luaScriptMainArgs = null;
            //     var luaMainName = batch.Remove(0, 8);
            //     string[] sectionArgs = luaMainName.Split(parenthesisSeparator);
            //     luaMainName = sectionArgs[0];
            //     if (sectionArgs.Length >= 2)
            //     {
            //         modsa_luaScriptMainArgs = sectionArgs[1].Split(',');
            //     }
            //     modsa_luaScriptMain = luaMainName;
            // }
            // else if (batch.Equals("RESETWHENUSE", StringComparison.OrdinalIgnoreCase)) resetWhenUse = true;
            // else if (batch.Equals("CLEARVALUES", StringComparison.OrdinalIgnoreCase)) clearValues = true;
            // else if (luaFound)
            // {
            //     MainClass.Logg.LogError("LUA cannot be used with other batches");
            //     return;
            // }
            // else batch_list.Add(batch);
        }
    }
}