using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;
using BattleUI;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;
using MTCustomScripts;
using System;

namespace MTCustomScripts.Patches;
internal class SinActionModelPatches
{
    [HarmonyPatch(typeof(SinActionModel), nameof(SinActionModel.DeSelectSin))]
    [HarmonyPostfix]
    public static void Postfix_SinActionModel_DeSelectSin(SinActionModel __instance)
    {
        // try
        // {
        //     SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.GetSinActionSlot(sam)?._firstSinSlot?._effectManager?._skillEffectList?.Clear();
        //     SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.GetSinActionSlot(sam)?._secondSinSlot?._effectManager?._skillEffectList?.Clear();
        // }
        // catch { }
        List<BattleUnitModel> unitList = SingletonBehavior<BattleObjectManager>.Instance.GetModelList();
        int actevent = MainClass.timingDict["OnDeSelectSin"];
        MTCustomScripts.Main.lastSinSlotIndex = __instance.GetSlotIndex();
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
    }

    [HarmonyPatch(typeof(SinActionModel), nameof(SinActionModel.SelectSin), new Type[]{typeof(UnitSinModel), typeof(SinActionModel)})]
    [HarmonyPostfix]
    public static void Postfix_SinActionModel_SelectSinOne(UnitSinModel sin, SinActionModel targetSinAction, SinActionModel __instance)
    {
        BattleUnitModel selector = __instance._unitModel;
        BattleUnitModel target = targetSinAction?._unitModel;

        if (selector != null)
        {
            int actevent_OnSlotSelectsTarget = MainClass.timingDict["OnSlotSelectsTarget"];
            SkillModel skillModel = __instance._currentBattleAction._skill;
            long skillmodel_intlong = skillModel.Pointer.ToInt64();
            if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong].ToArray())
				{
					modsa.Enact(selector, skillModel, __instance._currentBattleAction, targetSinAction?._currentBattleAction, actevent_OnSlotSelectsTarget, BATTLE_EVENT_TIMING.NONE);
				}
			}

            foreach (PassiveModel passiveModel in selector._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(selector, skillModel, __instance._currentBattleAction, targetSinAction?._currentBattleAction, actevent_OnSlotSelectsTarget, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (PassiveModel passiveModel in selector._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(selector, skillModel, __instance._currentBattleAction, targetSinAction?._currentBattleAction, actevent_OnSlotSelectsTarget, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (BuffModel buffModel in selector._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    modba.Enact(selector, skillModel, __instance._currentBattleAction, targetSinAction?._currentBattleAction, actevent_OnSlotSelectsTarget, BATTLE_EVENT_TIMING.NONE);
                }
            }
        }

        if (target != null)
        {
            int actevent_OnSlotSelectedAsTarget = MainClass.timingDict["OnSlotSelectedAsTarget"];
            SkillModel skillModel = targetSinAction?._currentBattleAction?._skill;
            if (skillModel != null)
            {
                long skillmodel_intlong = skillModel.Pointer.ToInt64();
                if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
                {
                    foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong].ToArray())
                    {
                        modsa.Enact(target, skillModel, targetSinAction._currentBattleAction, __instance._currentBattleAction, actevent_OnSlotSelectedAsTarget, BATTLE_EVENT_TIMING.NONE);
                    }
                }
            }

            foreach (PassiveModel passiveModel in target._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(target, skillModel, targetSinAction?._currentBattleAction, __instance._currentBattleAction, actevent_OnSlotSelectedAsTarget, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (PassiveModel passiveModel in target._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(target, skillModel, targetSinAction?._currentBattleAction, __instance._currentBattleAction, actevent_OnSlotSelectedAsTarget, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (BuffModel buffModel in target._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    modba.Enact(target, skillModel, targetSinAction?._currentBattleAction, __instance._currentBattleAction, actevent_OnSlotSelectedAsTarget, BATTLE_EVENT_TIMING.NONE);
                }
            }
        }
    }
}