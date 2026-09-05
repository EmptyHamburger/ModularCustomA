using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;
using System.Text.RegularExpressions;
using Il2CppSystem.Collections.Generic;
using MTCustomScripts;

internal class Modular_EnactConsequence
{
    private static readonly Regex matchReg = Main.Instance.replaceStringRegex;


    [HarmonyPatch(typeof(ModularSA), "Consequence")]
    [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
    public static bool Prefix_ModularSA_Consequence(ref string section, ModularSA __instance)
    {
        try
        {
            section = matchReg.Replace(section, match =>
            {
                string matchValue = match.Groups[1].Value;
                string sourceType = match.Groups[2].Success ? match.Groups[2].Value : null;

                string outValue = Main.GetCustomMTData(__instance.modsa_unitModel.Pointer.ToInt64(), match.Groups[1].Value, sourceType);
                return outValue != null ? outValue : match.Groups[0].Value;
            });
        }
        catch (System.Exception ex) { MainClass.Logg.LogInfo(ex); }

        return true;
    }

    [HarmonyPatch(typeof(ModularSA), nameof(ModularSA.GetTargetModel))]
    [HarmonyPostfix, HarmonyPriority(Priority.Low)]
    public static void Prefix_ModularSA_GetTargetModel(string param, ModularSA __instance, ref BattleUnitModel __result)
    {
        if (__result != null) return;

        string paramCopy = param.ToLower();
        BattleObjectManager objectManager = SingletonBehavior<BattleObjectManager>.Instance;

        if (paramCopy.Contains("char"))
        {
            int notPosition = paramCopy.IndexOf("char") + 2;
            string mode = (paramCopy.Length >= notPosition) ? (paramCopy[notPosition - 1] + paramCopy[notPosition]).ToString() : "0";

            _ = int.TryParse(mode.TrimEnd('s'), out int charID);

            __result = objectManager.GetModelByCharacterID(charID);
        }
    }

    [HarmonyPatch(typeof(ModularSA), nameof(ModularSA.GetTargetModelList))]
    [HarmonyPostfix, HarmonyPriority(Priority.Low)]
    public static void Postfix_ModularSA_GetTargetModelList(string param, ModularSA __instance, ref List<BattleUnitModel> __result)
    {
        string paramCopy = param.ToLower();
        BattleObjectManager objectManager = SingletonBehavior<BattleObjectManager>.Instance;

        if (paramCopy.Contains("char"))
        {
            int notPosition = paramCopy.IndexOf("char") + 2;
            string mode = (paramCopy.Length >= notPosition) ? (paramCopy[notPosition - 1] + paramCopy[notPosition]).ToString() : "0";

            _ = int.TryParse(mode.TrimEnd('s'), out int charID);

            BattleUnitModel charUnit = objectManager.GetModelByCharacterID(charID);
            if (charUnit != null && !__result.Contains(charUnit)) __result.Add(charUnit);
        }

        if (paramCopy.Contains("not"))
        {
            List<BattleUnitModel> notList = new();

            int notPosition = paramCopy.IndexOf("not") + 1;
            char mode = (paramCopy.Length >= notPosition) ? paramCopy[notPosition] : '0';

            notList = mode switch
            {
                '1' => objectManager.GetAliveList(false, UNIT_FACTION.PLAYER),
                '2' => objectManager.GetAliveList(true, UNIT_FACTION.PLAYER),
                '3' => objectManager.GetAliveList(false, UNIT_FACTION.ENEMY),
                '4' => objectManager.GetAliveList(true, UNIT_FACTION.ENEMY),
                _ => objectManager.GetModelList(),
            };
            for (int i = 0; i < __result.Count; i++) if (notList.Contains(__result[i])) notList.Remove(__result[i]);

            __result = notList;
        }
    }


    [HarmonyPatch(typeof(ModularSA), "ProcessBatch")]
    [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
    public static void Prefix_ModularSA_Consequence(string batch, ModularSA __instance)
    {
        Regex waitingReg = Main.Instance.waitingRegex;
        Match waitingMatch = waitingReg.Match(batch);

        if (waitingMatch.Success)
        {
            string waitingMatchTime = waitingMatch.Groups[1].Value;
            int finalWait = __instance.GetNumFromParamString(waitingMatchTime);


            batch = batch.Replace(waitingMatchTime + ":", "");
            Il2CppSystem.Threading.Tasks.Task.Delay(finalWait);
        }
    }
}