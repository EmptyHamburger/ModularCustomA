using System.Reflection;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace MTCustomScripts.Patches;

internal static class SkillAbilityMang_Patch
{
    [HarmonyPatch(typeof(SkillModel), nameof(SkillModel.OnStartTurn_BeforeLog))]
    [HarmonyPostfix]
    public static void Postfix_SkillModel_OnStartTurn_BeforeLog(SkillModel __instance)
    {
        foreach(var skillAbility in __instance._skillAbilityList)
        {
            if (skillAbility.TryCast<ICreateMang>() is ICreateMang iCreateMang)
            {
                Main.Logger.LogMessage($"Mang with Bool check: {(iCreateMang.IsCreateMang() ? iCreateMang.GetCreateMangCount() : 0)}");
            }
        }
    }

    [HarmonyPatch(typeof(CharacterAppearanceMangController), nameof(CharacterAppearanceMangController.OnSkillStart))]
    [HarmonyPrefix]
    public static void Prefix_CharacterAppearanceMangController_OnSkillStart(CharacterAppearanceMangController __instance)
    {
        if (__instance.mangControllers == null)
        {
            Main.Logger.LogFatal("PREFIX .mangControllers not found");
            return;
        }

        foreach(MangSkillController mangController in __instance.mangControllers)
        {
            if (MTCustomScripts.Main.intPtrMangAddOn_ActiveMangCountOnSkillStart.TryGetValue(mangController.Pointer, out int activeMangCount))
            {
                if (mangController.mangList == null)
                {
                    Main.Logger.LogMessage($"PREFIX .mangList is NULL");
                }
                Main.Logger.LogMessage($"PREFIX mangObj Count: {mangController.mangList.Count}");
                if (mangController._guideObj == null)
                {
                    Main.Logger.LogMessage($"PREFIX _guideObj is NULL");
                }

                mangController.ActivateMangBySkillID(activeMangCount);
                Main.Logger.LogMessage($"PREFIX Activated {activeMangCount} Mang");
            }
        }
    }

    [HarmonyPatch(typeof(CharacterAppearanceMangController), nameof(CharacterAppearanceMangController.OnSkillStart))]
    [HarmonyPostfix]
    public static void Postfix_CharacterAppearanceMangController_OnSkillStart(CharacterAppearanceMangController __instance)
    {
        if (__instance.mangControllers == null)
        {
            Main.Logger.LogFatal("POSTFIX .mangControllers not found");
            return;
        }

        foreach(MangSkillController mangController in __instance.mangControllers)
        {
            if (MTCustomScripts.Main.intPtrMangAddOn_ActiveMangCountOnSkillStart.TryGetValue(mangController.Pointer, out int activeMangCount))
            {
                if (mangController.mangList == null)
                {
                    Main.Logger.LogMessage($"POSTFIX .mangList is NULL");
                }
                Main.Logger.LogMessage($"POSTFIX mangObj Count: {mangController.mangList.Count}");
                if (mangController._guideObj == null)
                {
                    Main.Logger.LogMessage($"POSTFIX _guideObj is NULL");
                }

                mangController.ActivateMangBySkillID(activeMangCount);
                Main.Logger.LogMessage($"POSTFIX Activated {activeMangCount} Mang");
            }
        }
    }
}