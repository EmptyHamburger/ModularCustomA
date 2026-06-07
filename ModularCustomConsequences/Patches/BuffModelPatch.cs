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
    public static string GetModifiedLocale(BuffModel buffModel, string vanillaLocale)
    {
        if (string.IsNullOrEmpty(vanillaLocale)) return vanillaLocale;
        if (!Regex.IsMatch(vanillaLocale, @"<!([^>]+)>")) return vanillaLocale;

        vanillaLocale = Regex.Replace(vanillaLocale, @"<!([^>]+)>", match =>
        {
            string propertyName = match.Groups[1].Value;
            string modelName = "";
            if (propertyName.StartsWith("inst") && int.TryParse(propertyName.Substring(4), out int instID))
            modelName = SingletonBehavior<BattleObjectManager>.Instance.GetModel(instID).GetName().Replace("\n", " ");

            if (modelName != "") return modelName;
            
            return propertyName switch
            {
                "POTENCY0" => buffModel.GetStack(0).ToString(),
                "POTENCY1" => buffModel.GetStack(1).ToString(),
                "POTENCY2" => buffModel.GetAllStack().ToString(),
                "COUNT0" => buffModel.GetTurn(0).ToString(),
                "COUNT1" => buffModel.GetTurn(1).ToString(),
                "COUNT2" => buffModel.GetAllTurn().ToString(),
                "NAME" => buffModel.GetName(),
                _ => propertyName
            };
        });
        
        return vanillaLocale;
    }

    [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetDesc))]
    [HarmonyPostfix]
    public static void Postfix_BuffModel_GetDesc(BuffModel __instance, ref string __result)
    {
        __result = GetModifiedLocale(__instance, __result);
    }

    [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetFlavorText))]
    [HarmonyPostfix]
    public static void Postfix_BuffModel_GetFlavorText(BuffModel __instance, ref string __result)
    {
        __result = GetModifiedLocale(__instance, __result);
    }
}