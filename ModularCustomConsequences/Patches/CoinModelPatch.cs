using HarmonyLib;
using Il2CppSystem.Net;
using MTCustomScripts;
using ModularSkillScripts;
using System;
using ModularSkillScripts.Patches;

namespace MTCustomScripts.Patches;

internal class OneCoinLog_Patches
{
    [HarmonyPatch(typeof(OneCoinLog), nameof(OneCoinLog.SetAfterLog_Parrying))]
    [HarmonyPostfix]
    public static void Postfix_OneCoinLog_SetAfterLog_Parrying(BattleActionModel actorAction, BattleActionModel opponentAction, ParryingStatus parryingStatus, OneCoinLog __instance)
    {
        // MTCustomScripts.Main.Logger.LogFatal("Postfix_OneCoinLog_SetAfterLog_Parrying ran");
        // MTCustomScripts.Main.Logger.LogFatal($"Coin _isHead: {__instance._isHead}");
        // MTCustomScripts.Main.Logger.LogFatal($"Coin _coinLogIdx: {__instance._coinLogIdx}; _originCoinIdx: {__instance._originCoinIdx}; _realCoinIdx: {__instance._realCoinIdx}");
        // SkillPowerData skillPowerData = __instance._oneSkillPowerData;
        // MTCustomScripts.Main.Logger.LogFatal($"SkillPowerData adderResultSkillPower: {skillPowerData.adderResultSkillPower}; finalValue: {skillPowerData.finalValue}; modifiedSkillPower: {skillPowerData.modifiedSkillPower}; resultValue: {skillPowerData.resultValue}; vanillaSkillPower: {skillPowerData.vanillaSkillPower}");
        // CoinData coinData = skillPowerData.coinData;
        // MTCustomScripts.Main.Logger.LogFatal($"CoinData battleResult: {coinData.battleResult}");
        // OneCoinResult selfCoin = coinData.oneCoinResult;
        // OneCoinResult oppoCoin = coinData.opponentCoinResult;
        // MTCustomScripts.Main.Logger.LogFatal($"OneCoinResult-SELF accumulatedValue: {selfCoin.accumulatedValue}; addedResultScale: {selfCoin.addedResultScale}; afterDmg: {selfCoin.afterDmg}; beforeDmg: {selfCoin.beforeDmg}; idx: {selfCoin.idx}; operatorType: {selfCoin.operatorType}; prob: {selfCoin.prob}; scale: {selfCoin.scale};");
        // MTCustomScripts.Main.Logger.LogFatal($"OneCoinResult-OPPO accumulatedValue: {oppoCoin.accumulatedValue}; addedResultScale: {oppoCoin.addedResultScale}; afterDmg: {oppoCoin.afterDmg}; beforeDmg: {oppoCoin.beforeDmg}; idx: {oppoCoin.idx}; operatorType: {oppoCoin.operatorType}; prob: {oppoCoin.prob}; scale: {oppoCoin.scale};");

        int actevent = MainClass.timingDict["AfterCoinParrying"];
        BattleUnitModel unit = actorAction._model;

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
                modpa.modsa_coinModel = __instance._coin;
                modpa.Enact(unit, actorAction._skill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
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
                modpa.modsa_coinModel = __instance._coin;
                modpa.Enact(unit, actorAction._skill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
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
                modba.modsa_coinModel = __instance._coin;
                modba.Enact(unit, actorAction._skill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
            }
        }

        SkillModel skillModel = actorAction._skill;
        if (skillModel != null)
        {
            long skillmodel_intlong = skillModel.Pointer.ToInt64();
            if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
            {
                foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong].ToArray())
                {
                    if (modsa.activationTiming != actevent) continue;
                    modsa.modsa_coinModel = __instance._coin;
                    modsa.modsa_skillModel = skillModel;
                    modsa.Enact(unit, skillModel, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }
        }
    }
}