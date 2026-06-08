using HarmonyLib;
using Lethe.Patches;

namespace MTCustomScripts.Patches;

public class StageModel_Patch
{
	[HarmonyPatch(typeof(StageModel), nameof(StageModel.Init))]
    [HarmonyPrefix]
    public static void Prefix_StageModel_Init(StageModel __instance)
    {
        MTCustomScripts.Main.dl_activePathsDict.Clear();
    }

    [HarmonyPatch(typeof(StageModel), nameof(StageModel.OnStageEnd))]
    [HarmonyPrefix]
    public static void Prefix_StageModel_OnStageEnd(StageModel __instance)
    {
        MTCustomScripts.Main.dl_activePathsDict.Clear();
    }

    [HarmonyPatch(typeof(Data), nameof(Data.LoadCustomLocale), new[] { typeof(LOCALIZE_LANGUAGE) })]
    [HarmonyPrefix]
    public static void Postfix_Data_LoadCustomLocale(Data __instance)
    {
        MTCustomScripts.Main.dl_activePathsDict.Clear();
    }
}
