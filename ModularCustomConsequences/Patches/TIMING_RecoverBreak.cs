using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using BepInEx.Logging;
namespace MTCustomScripts.Patches
{
    internal class RecoverBreak
    {
        [HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.StartBehaviourAction))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        private static void StartBehaviourAction(BattleUnitView __instance)
        {
            var skillID = __instance.GetCurrentSkillViewer().curSkillID;
            MainClass.Logg.Log(LogLevel.Debug, "11");
            var skillData = Singleton<StaticDataManager>.Instance._skillList.GetData(skillID);
            MainClass.Logg.Log(LogLevel.Debug, "22");
            var model = __instance._unitModel.UnitDataModel;
            MainClass.Logg.LogInfo($"SBA, skill = {skillID}, model level = {model.Level}, model sync level = {model.SyncLevel}");

            var skillModel = new SkillModel(skillData, model.Level, model.SyncLevel);
            MainClass.Logg.Log(LogLevel.Debug, "33");
            skillModel.Init(); // needed to get noticed by modular skill timing?
            long skillmodel_intlong = skillModel.Pointer.ToInt64();
            MainClass.Logg.Log(LogLevel.Debug, "44");
            if (!SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) return;
            foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
            {
                modsa.Enact(__instance._unitModel, skillModel, null, null, MainClass.timingDict["StartVisualSkillUse"], BATTLE_EVENT_TIMING.ALL_TIMING);
                MainClass.Logg.Log(LogLevel.Debug, "55");
            }
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnRecoverBreak))]
        [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
        public static void BattleUnitModel_OnRecoverBreak_Postfix(BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
        {
            int actevent = MainClass.timingDict["OnRecoverBreak"];
            int actevent_onRecoveryBreak = MainClass.timingDict["OnOtherRecoverBreak"];

            foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
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
                        modpa.Enact(unit, null, null, null, actevent_onRecoveryBreak, timing);
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
                        modpa.Enact(unit, null, null, null, actevent_onRecoveryBreak, timing);
                    }
                }
            }
        }
    }
}
