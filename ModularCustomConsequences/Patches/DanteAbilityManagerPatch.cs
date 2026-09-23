using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using BattleUI;

namespace MTCustomScripts.Patches;

internal static class DuranteManager
{
    // [HarmonyPatch(typeof(DanteAbilityUIController), nameof(DanteAbilityUIController.AddActivatedDanteAbility))]
    // [HarmonyPostfix]
    // public static void Postfix_DanteAbilityUIController_AddActivatedDanteAbility()
    // {
    //     MTCustomScripts.Main.Logger.LogFatal("DanteAbilityUIController.AddActivatedDanteAbility");
    // }

    // [HarmonyPatch(typeof(DanteAbilityUIController), nameof(DanteAbilityUIController.OnClickConfirm))]
    // [HarmonyPostfix]
    // public static void Postfix_OnClickConfirm()
    // {
    //     MTCustomScripts.Main.Logger.LogFatal("DanteAbilityUIController.OnClickConfirm");
    // }

    [HarmonyPatch(typeof(DanteAbilityManager), nameof(DanteAbilityManager.ActivateDanteAbility))]
    [HarmonyPostfix]
    public static void Postfix_DanteAbility_ActivateDanteAbility(SEPIRA level, int id)
    {
        // MTCustomScripts.Main.Logger.LogFatal("DanteAbilityManager.ActivateDanteAbility");
        // SEPIRA sepira = __instance._classInfo._sepira;
        List<BattleUnitModel> unitList = SingletonBehavior<BattleObjectManager>.Instance.GetModelList();
        int actevent = MainClass.timingDict["OnActivateDurante"];
        List<BattleActionModel> bamList = Singleton<BattleActionModelManager>.Instance.GetActionList();
        MTCustomScripts.Main.Instance.durante_keyword = level;
        foreach (BattleUnitModel unit in unitList)
        {
            foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    if (!MTCustomScripts.Main.Instance.duranteTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
                    if (modpa.activationTiming != actevent) continue;
                    SEPIRA trigger = MTCustomScripts.Main.Instance.duranteTriggerDict[modpa.Pointer.ToInt64()];
                    if ((trigger != SEPIRA.NONE) && (trigger != level)) continue;
                    
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    if (!MTCustomScripts.Main.Instance.duranteTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
                    if (modpa.activationTiming != actevent) continue;
                    SEPIRA trigger = MTCustomScripts.Main.Instance.duranteTriggerDict[modpa.Pointer.ToInt64()];
                    if ((trigger != SEPIRA.NONE) && (trigger != level)) continue;

                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    if (!MTCustomScripts.Main.Instance.duranteTriggerDict.ContainsKey(modba.Pointer.ToInt64())) continue;
                    if (modba.activationTiming != actevent) continue;
                    SEPIRA trigger = MTCustomScripts.Main.Instance.duranteTriggerDict[modba.Pointer.ToInt64()];
                    if ((trigger != SEPIRA.NONE) && (trigger != level)) continue;

                    modba.modsa_buffModel = buffModel;
                    modba.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }
        
        foreach(BattleActionModel bam in bamList)
        {
            SkillModel skill = bam.Skill;
            if (skill == null) continue;
            long intLong = skill.Pointer.ToInt64();
            if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong))
            {
                foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong])
                {
                    if (!MTCustomScripts.Main.Instance.duranteTriggerDict.ContainsKey(modsa.Pointer.ToInt64())) continue;
                    if (modsa.activationTiming != actevent) continue;
                    SEPIRA trigger = MTCustomScripts.Main.Instance.duranteTriggerDict[modsa.Pointer.ToInt64()];
                    if ((trigger != SEPIRA.NONE) && (trigger != level)) continue;

                    modsa.Enact(bam._model, skill, bam, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }
    }
}