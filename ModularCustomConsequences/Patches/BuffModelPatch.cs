using HarmonyLib;
using Il2CppSystem.Net;
using MTCustomScripts;
using ModularSkillScripts;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
internal class BuffModel_Patches
{
    [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetDesc))]
    [HarmonyPostfix]
    public static void Postfix_BuffModel_GetDesc(BuffModel __instance, ref string __result)
    {
        __result = MTCustomScripts.Main.DynamicLocale_GetModifiedLocale(__instance, __result);
    }

    [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetFlavorText))]
    [HarmonyPostfix]
    public static void Postfix_BuffModel_GetFlavorText(BuffModel __instance, ref string __result)
    {
        __result = MTCustomScripts.Main.DynamicLocale_GetModifiedLocale(__instance, __result);
        // [0](this modder [0]([0](MT)[1](Styx)[2](pluh)[3](mellohi)) and a tester [1]([0](Blob)[1](Artik))[2]( and this user [0]([0](Shane)[1](Gun)[2](Divorce)[3](Lily))))[1]( noone)
    }
}