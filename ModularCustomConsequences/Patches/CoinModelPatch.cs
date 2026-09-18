using HarmonyLib;
using Il2CppSystem.Net;
using MTCustomScripts;
using ModularSkillScripts;
using System;
using ModularSkillScripts.Patches;

namespace MTCustomScripts.Patches;

internal class CoinModel_Patches
{
    [HarmonyPatch(typeof(CoinModel), nameof(CoinModel.OnResult_OnParrying))]
    [HarmonyPostfix]
    public static void Postfix_CoinModel_OnResult_OnParrying(BattleActionModel action, BattleActionModel oppoAction, BATTLE_EVENT_TIMING timing, CoinModel __instance)
    {
        MTCustomScripts.Main.Logger.LogFatal("Postfix_CoinModel_OnResult_OnParrying ran");
        int actevent = MainClass.timingDict["AfterCoinRollParrying"];
        BattleUnitModel unit = action._model;

        if (unit == null) return;

        foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                if (modpa.activationTiming != actevent) continue;
                modpa.modsa_passiveModel = passiveModel;
                modpa.modsa_coinModel = __instance;
                modpa.Enact(unit, action._skill, action, oppoAction, actevent, timing);
            }
        }

        foreach(PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                if (modpa.activationTiming != actevent) continue;
                modpa.modsa_passiveModel = passiveModel;
                modpa.modsa_coinModel = __instance;
                modpa.Enact(unit, action._skill, action, oppoAction, actevent, timing);
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
                modba.modsa_coinModel = __instance;
                modba.Enact(unit, action._skill, action, oppoAction, actevent, timing);
            }
        }
    }
}