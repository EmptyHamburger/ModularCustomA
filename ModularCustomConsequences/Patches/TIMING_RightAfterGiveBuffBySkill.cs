using HarmonyLib;
using MTCustomScripts;
using Il2CppSystem.Collections.Generic;
using Lethe.Patches;

namespace ModularSkillScripts.Patches;

internal class RightAfterGiveBuffBySkill
{
    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterGiveBuffBySkill))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_RightAfterGiveBuffBySkill(BattleActionModel action, SkillModel skill, BUFF_UNIQUE_KEYWORD bufKeyword, int originalStack, int originalTurn, int resultStack, int resultTurn, BattleUnitModel target, BATTLE_EVENT_TIMING timing, Il2CppSystem.Nullable<bool> isCritical, BattleUnitModel __instance)
    {
        MainClass.Logg.LogInfo("Patch timing: OnInflictBuff");

        MainClass.Logg.LogWarning(bufKeyword);
        MainClass.Logg.LogWarning(originalStack);
        MainClass.Logg.LogWarning(resultStack);
        MainClass.Logg.LogWarning(originalTurn);
        MainClass.Logg.LogWarning(resultTurn);
        // MainClass.Logg.LogWarning(activeRound);
        if (isCritical != null) MainClass.Logg.LogWarning(isCritical);

        int actevent = MainClass.timingDict["OnInflictBuff"];

        foreach(PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                MainClass.Logg.LogInfo("Founds modpassive - OnInflictBuff timing: " + modpa.passiveID);
                modpa.modsa_passiveModel = passiveModel;
                // MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                // MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                // MTCustomScripts.Main.Instance.gainbuff_turn = turn;
                // MTCustomScripts.Main.Instance.gainbuff_activeRound = activeRound;
                // MTCustomScripts.Main.Instance.gainbuff_source = srcType;
                modpa.Enact(__instance, skill, action, null, actevent, timing);
            }
        }

        foreach(PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                MainClass.Logg.LogInfo("Founds modpassive - OnInflictBuff timing: " + modpa.passiveID);
                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                // MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                // MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                // MTCustomScripts.Main.Instance.gainbuff_turn = turn;
                // MTCustomScripts.Main.Instance.gainbuff_activeRound = activeRound;
                // MTCustomScripts.Main.Instance.gainbuff_source = srcType;
                modpa.Enact(__instance, skill, action, null, actevent, timing);
            }
        }
    }
}