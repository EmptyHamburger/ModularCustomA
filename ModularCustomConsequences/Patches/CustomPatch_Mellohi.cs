using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;

namespace MTCustomScripts.Patches;

public class CustomPatch_Mellohi
{
	[HarmonyPatch(typeof(StageController), nameof(StageController.StartRoundAfterAbnormalityChoice_Init))]
	[HarmonyPostfix]
	private static void Postfix_StageController_StartRoundAfterAbnormalityChoice_Init()
	{
		foreach (KeyValuePair<int, BattleObjectManager.BattleUnit> allUnit in SingletonBehavior<BattleObjectManager>.Instance._allUnitDictionary)
		{
			BuffDetail __instance = allUnit.Value?.Model._buffDetail;
			foreach (BuffModel buffModel in __instance.GetActivatedBuffModelAll())
			{
				foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buffModel))
				{
					modba.modsa_buffModel = buffModel;
					modba.Enact(allUnit.Value.Model, null, null, null, MainClass.timingDict["AfterSlots"], BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}
		}
	}
}
