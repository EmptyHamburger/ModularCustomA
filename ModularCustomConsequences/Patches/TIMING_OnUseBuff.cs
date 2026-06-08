using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;

namespace MTCustomScripts.Patches;

public class OnUseBuff
{
	[HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnUseBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_RightAfterGetAnyBuffMT(BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
	{
		int actevent = MainClass.timingDict["OnUseBuff"];

		MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
		MTCustomScripts.Main.Instance.gainbuff_stack = stack;
		MTCustomScripts.Main.Instance.gainbuff_turn = turn;

		foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
		{
			if (!passiveModel.CheckActiveCondition()) continue;
			long passiveModel_intlong = passiveModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

			foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
			{
				if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
				BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modpa.Pointer.ToInt64()];
				if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;

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
				if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
				BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modpa.Pointer.ToInt64()];
				if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;

				modpa.modsa_passiveModel = passiveModel;
				modpa.Enact(__instance, null, null, null, actevent, timing);
			}
		}

		foreach (BuffModel buffModel in __instance._buffDetail.GetActivatedBuffModelAll())
		{
			long buffmodel_intlong = buffModel.Pointer.ToInt64();
			if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

			foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
			{
				modba.modsa_buffModel = buffModel;
				modba.Enact(__instance, null, null, null, actevent, timing);
			}
		}
	}
}
