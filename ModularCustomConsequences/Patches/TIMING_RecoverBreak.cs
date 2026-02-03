using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
namespace MTCustomScripts.Patches
{
    internal class RecoverBreak
    {
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
