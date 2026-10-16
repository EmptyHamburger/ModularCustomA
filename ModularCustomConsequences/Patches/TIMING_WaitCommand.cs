using HarmonyLib;
using Lethe.Patches;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using MTCustomScripts.Utils;

namespace MTCustomScripts.Patches;

public class WaitCommand_Patches
{
    [HarmonyPatch(typeof(StageController), nameof(StageController.FixedUpdate))]
    [HarmonyPrefix]
    public static void Prefix_StageController_FixedUpdate(StageController __instance)
    {
        if (__instance._phase == STAGE_PHASE.WAIT_COMMAND_BEFORE)
        {
            List<BattleUnitModel> unitList = SingletonBehavior<BattleObjectManager>.Instance.GetModelList();
            int actevent = MainClass.timingDict["WaitCommand"];
            List<BattleActionModel> bamList = Singleton<BattleActionModelManager>.Instance.GetActionList();
            foreach (BattleUnitModel unit in unitList)
            {
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
                        modsa.Enact(bam._model, skill, bam, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }
            }

            MTUtil.UpdateState();
        }
    }
}
