using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using MTCustomScripts;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace MTCustomScripts.Patches;

public class BattleActionModelManager_Patches
{
	// [HarmonyPatch(typeof(BattleActionModelManager), nameof(BattleActionModelManager.Parrying))]
	// [HarmonyPostfix]
	// public static void BattleActionModelManager_Parrying_Postfix(BattleActionModel actorAction, BattleActionModel oppoAction, BattleLog_Parrying parryingLog, ref ParryingStatus parryingStatus)
	// {
	// 	MTCustomScripts.Main.Logger.LogMessage("!!! Parrying PATCH RAN !!!");

	// 	MTCustomScripts.Main.Instance.forceEndDuel = false;
	// 	MTCustomScripts.Main.Instance.currentBattleLog_Parrying = parryingLog;

	// 	int actevent = MainClass.timingDict["Parrying"];

	// 	BattleUnitModel unit = actorAction.Model;

	// 	SkillModel skillModel = actorAction._skill;
	// 	long skillmodel_intlong = skillModel.Pointer.ToInt64();

	// 	if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
	// 	{
	// 		foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong].ToArray())
	// 		{
	// 			modsa.Enact(unit, skillModel, actorAction, oppoAction, actevent, BATTLE_EVENT_TIMING.NONE);
	// 		}
	// 	}

	// 	foreach(PassiveModel passiveModel in unit._passiveDetail.PassiveList.ToArray())
	// 	{
	// 		if (!passiveModel.CheckActiveCondition()) continue;
	// 		long passiveModel_intlong = passiveModel.Pointer.ToInt64();
	// 		if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

	// 		foreach(ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong].ToArray())
	// 		{
	// 			modpa.Enact(unit, skillModel, actorAction, oppoAction, actevent, BATTLE_EVENT_TIMING.NONE);
	// 		}
	// 	}

	// 	foreach(PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList.ToArray())
	// 	{
	// 		if (!passiveModel.CheckActiveCondition()) continue;
	// 		long passiveModel_intlong = passiveModel.Pointer.ToInt64();
	// 		if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

	// 		foreach(ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong].ToArray())
	// 		{
	// 			modpa.Enact(unit, skillModel, actorAction, oppoAction, actevent, BATTLE_EVENT_TIMING.NONE);
	// 		}
	// 	}

	// 	foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll().ToArray())
	// 	{
	// 		long buffmodel_intlong = buffModel.Pointer.ToInt64();
	// 		if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

	// 		foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong].ToArray())
	// 		{
	// 			modba.modsa_buffModel = buffModel;
	// 			modba.Enact(unit, skillModel, actorAction, oppoAction, actevent, BATTLE_EVENT_TIMING.NONE);
	// 		}
	// 	}
		
	// 	if (MTCustomScripts.Main.Instance.forceEndDuel)
	// 	{
	// 		parryingStatus.actorParryingLife = 0;
    //     	parryingStatus.opponentParryingLife = 0;
	// 	}
	// }

	// [HarmonyPostfix]
	// static void Postfix(BattleActionModel action, BattleActionModel oppoAction, ParryingStatus parryingStatus, int parryingMaxCount, ref bool __result)
	// {
	// 	MTCustomScripts.Main.Logger.LogMessage("!!! CanParryingContinue TARGETMETHODS PATCH RAN !!!");
	// 	bool flag = true;

	// 	if (!action.Model.IsActionable(action) || 
	// 		!oppoAction.Model.IsActionable(oppoAction) || 
	// 		parryingStatus.actorParryingLife <= 0 || 
	// 		parryingStatus.opponentParryingLife <= 0 || 
	// 		parryingMaxCount >= 999)
	// 	{
	// 		flag = false;
	// 	}

	// 	__result = flag;
	// }

	// [HarmonyPatch(typeof(BattleActionModelManager), "CanParryingContinue")]
	// [HarmonyPrefix]
	// public static bool BattleActionModelManager_CanParryingContinue(BattleActionModelManager __instance, BattleActionModel action, BattleActionModel oppoAction, ParryingStatus parryingStatus, int parryingMaxCount, ref bool __result)
	// {
	// 	MTCustomScripts.Main.Logger.LogMessage("!!! CanParryingContinue PATCH RAN !!!");
	// 	bool flag = true;

	// 	if (!action.Model.IsActionable(action) || 
	// 		!oppoAction.Model.IsActionable(oppoAction) || 
	// 		parryingStatus.actorParryingLife <= 0 || 
	// 		parryingStatus.opponentParryingLife <= 0 || 
	// 		parryingMaxCount >= 999)
	// 	{
	// 		flag = false;
	// 	}

	// 	__result = flag;

	// 	return false;
	// }

	[HarmonyPatch(typeof(BattleActionModelManager), "SortActions")]
	[HarmonyPostfix]
	public static void BattleActionModelManager_SortActions(BattleActionModelManager __instance)
	{
		MTCustomScripts.Main.Logger.LogMessage("!!! SortAction PATCH RAN !!!");
		Il2CppSystem.Collections.Generic.List<BattleActionModel> actionList = __instance._actionList;

		MTCustomScripts.Main.GetDatasFromActionListForAcquirers(actionList);

		int actevent = MainClass.timingDict["SortAction"];

		System.Collections.Generic.List<IntPtr> unitPtrIntList = new();

		foreach(BattleActionModel bam in actionList.ToArray())
		{
			SkillModel skillModel = bam._skill;
			long skillmodel_intlong = skillModel.Pointer.ToInt64();

			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong].ToArray())
				{
					modsa.Enact(bam.Model, skillModel, bam, bam.GetMainTargetSinAction()._currentBattleAction, actevent, BATTLE_EVENT_TIMING.NONE);
				}
			}

			BattleUnitModel unit = bam._model;
			if (unitPtrIntList.Contains(unit.Pointer)) continue;

			unitPtrIntList.Add(unit.Pointer);

			foreach(PassiveModel passiveModel in unit._passiveDetail.PassiveList.ToArray())
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

				foreach(ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
				{
					modpa.Enact(unit, skillModel, bam, bam.GetMainTargetSinAction()._currentBattleAction, actevent, BATTLE_EVENT_TIMING.NONE);
				}
			}

			foreach(PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList.ToArray())
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				long passiveModel_intlong = passiveModel.Pointer.ToInt64();
				if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

				foreach(ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
				{
					modpa.Enact(unit, skillModel, bam, bam.GetMainTargetSinAction()._currentBattleAction, actevent, BATTLE_EVENT_TIMING.NONE);
				}
			}

			foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll().ToArray())
			{
				long buffmodel_intlong = buffModel.Pointer.ToInt64();
				if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

				foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
				{
					modba.modsa_buffModel = buffModel;
					modba.Enact(unit, skillModel, bam, bam.GetMainTargetSinAction()._currentBattleAction, actevent, BATTLE_EVENT_TIMING.NONE);
				}
			}
		}

		MTCustomScripts.Main.GetDatasFromActionListForAcquirers(actionList);
	}
}