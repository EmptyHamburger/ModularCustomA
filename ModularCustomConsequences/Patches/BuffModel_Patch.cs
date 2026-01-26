using HarmonyLib;

internal class BuffModelPatch
{
    [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetOverwritePanicType))]
    [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
    public static void BuffModel_OverwritePanicType_Postfix(BuffModel __instance, ref PANIC_TYPE __result)
    {
        if (MTCustomScripts.Main.testStuffStorage.overrideBuffPanicDict.TryGetValue(__instance, out PANIC_TYPE overridePanic)) __result = overridePanic;
    }
}