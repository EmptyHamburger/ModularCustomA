using HarmonyLib;
using Lethe.Patches;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using MTCustomScripts.Utils;
using BattleUI.BattleUnit;
using MTCustomScripts;

namespace MTCustomScripts.Patches;

public class OpenInfo_Patches
{
    [HarmonyPatch(typeof(BattleUnitUIManager), nameof(BattleUnitUIManager.OpenInformation))]
    [HarmonyPrefix]
    public static bool Prefix_BattleUnitUIManager_OpenInformation(bool isTutorial, BattleUnitUIManager __instance)
    {
        MTCustomScripts.Main._TIMING_OpenInfo_RunOriginMethod = true;
        int actevent = MainClass.timingDict["OpenInfo"];
        BattleUnitModel unit = __instance._model;
        if (unit == null) return true;

        foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
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
                modba.modsa_buffModel = buffModel;
                modba.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
            }
        }

        List<BattleActionModel> bamList = Singleton<BattleActionModelManager>.Instance.GetActionList();        
        foreach(BattleActionModel bam in bamList)
        {
            if (bam._model == null) continue;
            if (bam._model._instanceID != unit._instanceID) continue;
            SkillModel skill = bam.Skill;
            if (skill == null) continue;
            long intLong = skill.Pointer.ToInt64();
            if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong))
            {
                foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong])
                {
                    modsa.Enact(bam._model, skill, bam, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }

        MTUtil.UpdateState();
        
        return MTCustomScripts.Main._TIMING_OpenInfo_RunOriginMethod;
    }
}
