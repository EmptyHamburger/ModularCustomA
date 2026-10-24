using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using MTCustomScripts;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Utils;

namespace MTCustomScripts.Patches;

public class BattleLog_Parrying_Patch
{

    [HarmonyPatch(typeof(BattleLog_Parrying), nameof(BattleLog_Parrying.SetAfterLog), new Type[]{typeof(BattleActionModel), typeof(BattleActionModel), typeof(ParryingStatus)})]
	[HarmonyPostfix]
	public static void Postfix_BattleLog_Parrying_SetAfterLog(BattleActionModel actorAction, BattleActionModel opponentAction, ParryingStatus parryingStatus, BattleLog_Parrying __instance)
	{
		// MTCustomScripts.Main.Logger.LogFatalError($"[BATTLELOG_SetAfterLog] actor Result: {__instance.GetAllCoinTotalResult(actorAction._model._instanceID)}; opponent Result: {__instance.GetAllCoinTotalResult(opponentAction._model._instanceID)}");
        //Runs after AfterDuelClash from Modular

        MTCustomScripts.Main.currentBattleLog_Parrying = __instance;

		int actevent = MainClass.timingDict["AfterDuelClashLog"];

		BattleUnitModel actorUnit = actorAction._model;
        BattleUnitModel opponentUnit = opponentAction._model;

        if (actorUnit != null)
        {
            foreach (PassiveModel passiveModel in actorUnit._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(actorUnit, actorAction._skill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (PassiveModel passiveModel in actorUnit._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(actorUnit, actorAction._skill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (BuffModel buffModel in actorUnit._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    modba.Enact(actorUnit, actorAction._skill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }
        }

        if (opponentUnit != null)
        {
            foreach (PassiveModel passiveModel in opponentUnit._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(opponentUnit, opponentAction._skill, opponentAction, actorAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (PassiveModel passiveModel in opponentUnit._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(opponentUnit, opponentAction._skill, opponentAction, actorAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }

            foreach (BuffModel buffModel in opponentUnit._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    modba.Enact(opponentUnit, opponentAction._skill, opponentAction, actorAction, actevent, BATTLE_EVENT_TIMING.NONE);
                }
            }
        }

        SkillModel actorSkill = actorAction._skill;
        if (actorSkill != null)
        {
            long intLong2 = actorSkill.Pointer.ToInt64();
            if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong2))
            {
                foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong2])
                {
                    modsa.Enact(actorUnit, actorSkill, actorAction, opponentAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }

        SkillModel opponentSkill = opponentAction._skill;
        if (opponentSkill == null) return;
        long intLong = opponentSkill.Pointer.ToInt64();
        if (SkillScriptInitPatch.modsaDict.ContainsKey(intLong))
        {
            foreach(ModularSA modsa in SkillScriptInitPatch.modsaDict[intLong])
            {
                modsa.Enact(opponentUnit, opponentSkill, opponentAction, actorAction, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
            }
        }

        MTCustomScripts.Main.currentBattleLog_Parrying = null;
	}   
}