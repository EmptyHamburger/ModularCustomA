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

public class PassiveDetail_Patches
{
	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnRoundStart_Before))]
	[HarmonyPostfix]
	public static void Postfix_PassiveDetail_OnRoundStart_Before(PassiveDetail __instance)
	{
        foreach (long key in SkillScriptInitPatch.modpaDict.Keys)
        {
			Il2CppSystem.Collections.Generic.List<ModularSA> value = SkillScriptInitPatch.modpaDict[key];
			foreach (ModularSA modular in value) modular.ResetAdders();
		}

		SkillScriptInitPatch.SimpleEnactPassive(__instance._owner, null, null, null, "BeforeRoundStart", BATTLE_EVENT_TIMING.NONE, __instance);
		foreach (SinActionModel sinAction in __instance._owner.GetSinActionList())
		{
			foreach (UnitSinModel sinModel in sinAction.currentSinList)
			{
				SkillModel skillModel = sinModel.GetSkill();
				if (skillModel == null) continue;
				long skillmodel_intlong = skillModel.Pointer.ToInt64();

				if (!SkillScriptInitPatch.modsaDict.ContainsKey(skillmodel_intlong)) continue;
				foreach (ModularSA modsa in SkillScriptInitPatch.modsaDict[skillmodel_intlong]) {
					//MainClass.Logg.LogInfo("Found modsa - RoundStart");
					modsa.Enact(__instance._owner, skillModel, null, null, MainClass.timingDict["BeforeRoundStart"], BATTLE_EVENT_TIMING.NONE);
				}
			}
		}
	}

	// [HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnKillTarget))]
	// [HarmonyPostfix]
	// public static void Postfix_PassiveDetail_OnKillTarget(BattleActionModel actionOrNull, BattleUnitModel target, DAMAGE_SOURCE_TYPE dmgSrcType, BATTLE_EVENT_TIMING timing, PassiveDetail __instance)
	// {
	// 	int actevent = MainClass.timingDict["EnemyKill"];
	// 	// MTCustomScripts.Main.Logger.LogFatal("ENEMY KILL PATCH RAN");
	// 	foreach (PassiveModel passiveModel in __instance.PassiveList)
	// 	{
	// 		if (!passiveModel.CheckActiveCondition()) continue;
	// 		long passiveModel_intlong = passiveModel.Pointer.ToInt64();
	// 		if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

	// 		foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
	// 		{
	// 			if (modpa.activationTiming != actevent) continue;
	// 			modpa.modsa_passiveModel = passiveModel;
	// 			modpa.modsa_victimModel = target;
	// 			modpa.modsa_killerModel = actionOrNull?._model;
	// 			modpa.Enact(actionOrNull?._model, actionOrNull?._skill, actionOrNull, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
	// 		}
	// 	}

	// 	foreach (PassiveModel passiveModel in __instance.EgoPassiveList)
	// 	{
	// 		if (!passiveModel.CheckActiveCondition()) continue;
	// 		long passiveModel_intlong = passiveModel.Pointer.ToInt64();
	// 		if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

	// 		foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
	// 		{
	// 			if (modpa.activationTiming != actevent) continue;
	// 			modpa.modsa_passiveModel = passiveModel;
	// 			modpa.modsa_victimModel = target;
	// 			modpa.modsa_killerModel = actionOrNull?._model;
	// 			modpa.Enact(actionOrNull?._model, actionOrNull?._skill, actionOrNull, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
	// 		}
	// 	}
	// }
}