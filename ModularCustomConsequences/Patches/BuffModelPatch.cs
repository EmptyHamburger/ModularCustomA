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
        // MTCustomScripts.Main.Logger.LogFatal("DYNAMIC LOCALE PATCH RAN");
        if (string.IsNullOrEmpty(__result)) return;
        if (!Regex.IsMatch(__result, @"<!([^>]+)>")) return;

        __result = Regex.Replace(__result, @"<!([^>]+)>", match =>
        {
            string propertyName = match.Groups[1].Value;
            string modelName = "";
            if (propertyName.StartsWith("inst") && int.TryParse(propertyName.Substring(4), out int instID))
            modelName = SingletonBehavior<BattleObjectManager>.Instance.GetModel(instID).GetName().Replace("\n", " ");

            if (modelName != "") return modelName;
            
            return propertyName switch
            {
                "POTENCY0" => __instance.GetStack(0).ToString(),
                "POTENCY1" => __instance.GetStack(1).ToString(),
                "POTENCY2" => __instance.GetAllStack().ToString(),
                "COUNT0" => __instance.GetTurn(0).ToString(),
                "COUNT1" => __instance.GetTurn(1).ToString(),
                "COUNT2" => __instance.GetAllTurn().ToString(),
                "NAME" => __instance.GetName(),
                _ => propertyName
            };
        });
    }
}