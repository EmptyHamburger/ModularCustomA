using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using BattleUI;
using BattleUI.Operation;

namespace MTCustomScripts.Patches;

internal static class SlotHoveringPatch
{
    [HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.ShowSkillInfoByOperation))]
    [HarmonyPostfix]
    public static void Postfix_NewOperationController_ShowSkillInfoByOperation(SinActionModel sinAction, UnitSinModel unitsin)
    {
        // MTCustomScripts.Main.Logger.LogFatal($"Postfix_NewOperationController_ShowSkillInfoByOperation");
        MTCustomScripts.Main.currentSinActionModelPlayer = sinAction;
        BattleUnitModel unit = sinAction._unitModel;
        int actevent = MainClass.timingDict["OnDashboardSkillHover"];

        if (unit != null)
        {
            foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    if (modpa.activationTiming != actevent) continue;
                    
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(unit, sinAction?._currentBattleAction?._skill, sinAction?._currentBattleAction, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    if (modpa.activationTiming != actevent) continue;

                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(unit, sinAction?._currentBattleAction?._skill, sinAction?._currentBattleAction, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    if (modba.activationTiming != actevent) continue;

                    modba.modsa_buffModel = buffModel;
                    modba.Enact(unit, sinAction?._currentBattleAction?._skill, sinAction?._currentBattleAction, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }

        SkillModel skill = sinAction?._currentBattleAction?.Skill;
        if (skill != null)
        {
            long intLong = skill.Pointer.ToInt64();
            if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong))
            {
                foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong])
                {
                    if (modsa.activationTiming != actevent) continue;

                    modsa.Enact(unit, skill, sinAction?._currentBattleAction, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }

        BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
        objManager.UpdatePassiveState();
        objManager.OnRoundStart_View_AfterChoice();
        objManager.UpdateViewState(false, false);

        foreach (BattleUnitView unitView in objManager.GetAliveViewList())
        {
            unitView.RefreshAppearanceRenderer(true);
        }
    }
    // SingletonBehavior<BattleUIRoot>.Instance.ShowExpectedSkillInfoByOverAction(abActionSlot.currentSelectSin, abActionSlot);

    [HarmonyPatch(typeof(BattleUIRoot), nameof(BattleUIRoot.ShowExpectedSkillInfoByOverAction))]
    [HarmonyPostfix]
    public static void Postfix_BattleUIRoot_ShowExpectedSkillInfoByOverAction(UnitSinModel sin, SinActionModel targetSinAction, BattleUIRoot __instance)
    {
        // MTCustomScripts.Main.Logger.LogFatal($"Postfix_BattleUIRoot_ShowExpectedSkillInfoByOverAction");

        SinActionModel targeterSAM = MTCustomScripts.Main.currentSinActionModelPlayer;

        if (targeterSAM != null)
        {
            int actevent = MainClass.timingDict["OnSlotHoverTarget"];
            BattleUnitModel unit = targeterSAM._unitModel;
            if (unit != null)
            {
                foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        if (modpa.activationTiming != actevent) continue;
                        
                        modpa.modsa_passiveModel = passiveModel;
                        modpa.Enact(unit, targeterSAM?._currentBattleAction?._skill, targeterSAM?._currentBattleAction, targetSinAction?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }

                foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        if (modpa.activationTiming != actevent) continue;

                        modpa.modsa_passiveModel = passiveModel;
                        modpa.Enact(unit, targeterSAM?._currentBattleAction?._skill, targeterSAM?._currentBattleAction, targetSinAction?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }

                foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll())
                {
                    long buffmodel_intlong = buffModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                    foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                    {
                        if (modba.activationTiming != actevent) continue;

                        modba.modsa_buffModel = buffModel;
                        modba.Enact(unit, targeterSAM?._currentBattleAction?._skill, targeterSAM?._currentBattleAction, targetSinAction?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }
            }
            
            SkillModel skill = targeterSAM?._currentBattleAction?.Skill;
            if (skill != null)
            {
                long intLong = skill.Pointer.ToInt64();
                if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong))
                {
                    foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong])
                    {
                        if (modsa.activationTiming != actevent) continue;

                        modsa.Enact(unit, skill, targeterSAM?._currentBattleAction, targetSinAction?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }
            }
        }

        if (targetSinAction != null)
        {
            int actevent = MainClass.timingDict["OnSlotHoveredBy"];
            BattleUnitModel unit = targetSinAction._unitModel;
            if (unit != null)
            {
                foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        if (modpa.activationTiming != actevent) continue;
                        
                        modpa.modsa_passiveModel = passiveModel;
                        modpa.Enact(unit, targetSinAction?._currentBattleAction?._skill, targetSinAction?._currentBattleAction, targeterSAM?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }

                foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        if (modpa.activationTiming != actevent) continue;

                        modpa.modsa_passiveModel = passiveModel;
                        modpa.Enact(unit, targetSinAction?._currentBattleAction?._skill, targetSinAction?._currentBattleAction, targeterSAM?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }

                foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll())
                {
                    long buffmodel_intlong = buffModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                    foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                    {
                        if (modba.activationTiming != actevent) continue;

                        modba.modsa_buffModel = buffModel;
                        modba.Enact(unit, targetSinAction?._currentBattleAction?._skill, targetSinAction?._currentBattleAction, targeterSAM?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }
            }
            
            SkillModel skill = targetSinAction?._currentBattleAction?.Skill;
            if (skill != null)
            {
                long intLong = skill.Pointer.ToInt64();
                if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong))
                {
                    foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong])
                    {
                        if (modsa.activationTiming != actevent) continue;

                        modsa.Enact(unit, skill, targetSinAction?._currentBattleAction, targeterSAM?._currentBattleAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                    }
                }
            }
        }

        BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
        objManager.UpdatePassiveState();
        objManager.OnRoundStart_View_AfterChoice();
        objManager.UpdateViewState(false, false);

        foreach (BattleUnitView unitView in objManager.GetAliveViewList())
        {
            unitView.RefreshAppearanceRenderer(true);
        }
    }
}