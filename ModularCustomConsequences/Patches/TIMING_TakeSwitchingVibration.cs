using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;

internal class TIMING_Vibration
{
    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnTakePiledUpVibrationToSpecial_Try))]
    [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
    public static void BattleUnitModel_OnTakePiledVibrationTry_Postfix(BattleUnitModel giverOrNull, BUFF_UNIQUE_KEYWORD originalKeyword, BUFF_UNIQUE_KEYWORD newKeyword, bool isSucceed, bool isTryToPileUp, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
    {
        if (isTryToPileUp)
        {
            int actevent = (isSucceed) ? MainClass.timingDict["OnTakePiledVibration"] : MainClass.timingDict["OnTakePiledVibrationTry"];
            int actevent_others = (isSucceed) ? MainClass.timingDict["OnOtherTakePiledVibration"] : MainClass.timingDict["OnOtherTakePiledVibrationTry"];

            foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    if (giverOrNull != null) modpa.modsa_target_list.Add(giverOrNull);
                    modpa.Enact(__instance, null, null, null, actevent, timing);
                }
            }

            foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    if (giverOrNull != null) modpa.modsa_target_list.Add(giverOrNull);
                    modpa.Enact(__instance, null, null, null, actevent, timing);
                }
            }


            BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
            foreach (BattleUnitModel unit in battleObjManager_inst.GetAliveListExceptSelf(__instance, false, false))
            {
                foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        modpa.modsa_passiveModel = passiveModel;
                        modpa.modsa_target_list.Clear();
                        modpa.modsa_target_list.Add(__instance);
                        modpa.Enact(unit, null, null, null, actevent_others, timing);
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
                        modpa.modsa_target_list.Clear();
                        modpa.modsa_target_list.Add(__instance);
                        modpa.Enact(unit, null, null, null, actevent_others, timing);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnTakeSwitchingVibrationToSpecial_Try))]
    [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
    public static void BattleUnitModel_OnTakeSwitchingVibrationTry_Postfix(BattleUnitModel giverOrNull, BUFF_UNIQUE_KEYWORD originalKeyword, BUFF_UNIQUE_KEYWORD newKeyword, bool isSucceed, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
    {
        int actevent = (isSucceed) ? MainClass.timingDict["OnTakeSwitchingVibration"] : MainClass.timingDict["OnTakeSwitchingVibrationTry"];
        int actevent_others = (isSucceed) ? MainClass.timingDict["OnOtherTakeSwitchingVibration"] : MainClass.timingDict["OnOtherTakeSwitchingVibrationTry"];

        foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                modpa.modsa_passiveModel = passiveModel;
                if (giverOrNull != null) modpa.modsa_target_list.Add(giverOrNull);
                modpa.Enact(__instance, null, null, null, actevent, timing);
            }
        }

        foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                modpa.modsa_passiveModel = passiveModel;
                if (giverOrNull != null) modpa.modsa_target_list.Add(giverOrNull);
                modpa.Enact(__instance, null, null, null, actevent, timing);
            }
        }


        BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
        foreach (BattleUnitModel unit in battleObjManager_inst.GetAliveListExceptSelf(__instance, false, false))
        {
            foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.modsa_target_list.Clear();
                    modpa.modsa_target_list.Add(__instance);
                    modpa.Enact(unit, null, null, null, actevent_others, timing);
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
                    modpa.modsa_target_list.Clear();
                    modpa.modsa_target_list.Add(__instance);
                    modpa.Enact(unit, null, null, null, actevent_others, timing);
                }
            }
        }
    }
}
