using HarmonyLib;
using Lethe.Patches;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using MTCustomScripts.Utils;

namespace MTCustomScripts.Patches;

public class ResetDatas_Patch
{
    [HarmonyPatch(typeof(StageModel), nameof(StageModel.Init))]
    [HarmonyPrefix]
    public static void Prefix_StageModel_Init(StageModel __instance)
    {
        MTCustomScripts.Main.dl_activePathsDict.Clear();
        MTCustomScripts.Main.dl_overwritePathValue.Clear();
        MTCustomScripts.Main.gateSPDict.Clear();
        MTUtil._excludeRendererRefreshInstanceIDList.Clear();
    }

    [HarmonyPatch(typeof(StageModel), nameof(StageModel.OnStageEnd))]
    [HarmonyPrefix]
    public static void Prefix_StageModel_OnStageEnd(StageModel __instance)
    {
        MTCustomScripts.Main.dl_activePathsDict.Clear();
        MTCustomScripts.Main.dl_overwritePathValue.Clear();
        MTCustomScripts.Main.gateSPDict.Clear();
        MTUtil._excludeRendererRefreshInstanceIDList.Clear();
    }

    [HarmonyPatch(typeof(Data), nameof(Data.LoadCustomLocale), new[] { typeof(LOCALIZE_LANGUAGE) })]
    [HarmonyPrefix]
    public static void Postfix_Data_LoadCustomLocale(Data __instance)
    {
        MTCustomScripts.Main.dl_activePathsDict.Clear();
        MTCustomScripts.Main.dl_overwritePathValue.Clear();
    }
}
