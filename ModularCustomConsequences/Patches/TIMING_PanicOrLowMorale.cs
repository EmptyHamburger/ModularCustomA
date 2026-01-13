using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;

internal class PanicOrLowMorale
{
    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnPanicOrLowMorale))]
    [HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
    public static void BattleUnitModel_OnPanicOrLowMorale_Postfix(PANIC_LEVEL level, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
    {
        if (level == PANIC_LEVEL.None) return;

        int actevent = (level == PANIC_LEVEL.Panic) ? MainClass.timingDict["OnPanic"] : MainClass.timingDict["OnLowMorale"];
        int actevent_otherPanicLowMorale = (level == PANIC_LEVEL.Panic) ? MainClass.timingDict["OnOtherPanic"] : MainClass.timingDict["OnOtherLowMorale"];

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
                    modpa.Enact(unit, null, null, null, actevent_otherPanicLowMorale, timing);
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
                    modpa.Enact(unit, null, null, null, actevent_otherPanicLowMorale, timing);
                }
            }
        }
    }
}
