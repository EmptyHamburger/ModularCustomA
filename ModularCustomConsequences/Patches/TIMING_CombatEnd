using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;

namespace MTCustomScripts.Patches;

internal static class CombatEndState
{
	public static bool armed = false;
}

public class CombatEnd
{
	[HarmonyPatch(typeof(PassiveDetail), nameof(PassiveDetail.OnStartRunBattle))]
	[HarmonyPostfix]
	private static void Postfix_PassiveDetail_OnStartRunBattle()
	{
		CombatEndState.armed = true;
	}

	[HarmonyPatch(typeof(BattleActionModelManager), nameof(BattleActionModelManager.Run), new System.Type[] { typeof(BattleActionModel) })]
	[HarmonyPostfix]
	private static void Postfix_BattleActionModelManager_RunAction(BattleActionModelManager __instance)
	{
		if (!CombatEndState.armed) return;

		int remaining = __instance._actionList.Count;
		Main.Logger.LogMessage($"[CombatEnd] Action resolved, actions remaining = {remaining}");
		if (remaining > 0) return;

		CombatEndState.armed = false;
		Main.Logger.LogMessage("[CombatEnd] Action list empty, firing timing.");
		FireTiming();
	}

	private static void FireTiming()
	{
		int actevent = MainClass.timingDict["CombatEnd"];

		foreach (BattleUnitModel unit in SingletonBehavior<BattleObjectManager>.Instance.GetModelList())
		{
			foreach (BuffModel buf in unit.GetActivatedBuffModels())
			{
				foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
				{
					if (modba.activationTiming != actevent) continue;
					modba.modsa_buffModel = buf;
					modba.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}

			foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
				{
					if (modpa.activationTiming != actevent) continue;
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}

			foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
			{
				if (!passiveModel.CheckActiveCondition()) continue;
				foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel, false))
				{
					if (modpa.activationTiming != actevent) continue;
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}
		}
	}
}
