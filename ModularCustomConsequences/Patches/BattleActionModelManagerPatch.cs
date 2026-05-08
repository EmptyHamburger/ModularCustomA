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
	// 	// parryingStatus.actorParryingLife = 99;
    //     // parryingStatus.opponentParryingLife = 99;
	// }

	// [HarmonyTargetMethods]
	// static System.Collections.Generic.IEnumerable<MethodBase> TargetMethods()
	// {
	// 	yield return AccessTools.Method(typeof(BattleActionModelManager), "CanParryingContinue", new[] { 
    //         typeof(BattleActionModel), 
    //         typeof(BattleActionModel), 
    //         typeof(ParryingStatus), 
    //         typeof(int) 
    //     });
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

		foreach(BattleActionModel bam in actionList)
		{
			SkillModel skillModel = bam._skill;
			long skillmodel_intlong = skillModel.Pointer.ToInt64();

			int actevent = MainClass.timingDict["SortAction"];
			if (SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong))
			{
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong])
				{
					modsa.Enact(bam.Model, skillModel, bam, null, actevent, BATTLE_EVENT_TIMING.NONE);
				}
			}
		}
	}
}