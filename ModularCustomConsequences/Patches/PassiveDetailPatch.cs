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
}