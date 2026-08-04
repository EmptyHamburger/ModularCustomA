using HarmonyLib;

namespace MTCustomScripts.Patches;

internal static class GateSP
{
    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnChangeMp))]
    [HarmonyPostfix]
    public static void Postfix_BattleUnitModel_OnChangeMP(int oldMp, int newMp, BattleUnitModel __instance)
    {
        if (!MTCustomScripts.Main.gateSPDict.TryGetValue(__instance.Pointer, out (int Min, int Max) gateData)) return;
        if (newMp > gateData.Max) __instance._changeStat.SetMp(gateData.Max, out int _);
        if (newMp < gateData.Min) __instance._changeStat.SetMp(gateData.Min, out int _);
    }
}