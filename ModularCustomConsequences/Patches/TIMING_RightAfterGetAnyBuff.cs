using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;
using ModularSkillScripts;
using MTCustomScripts;

namespace ModularSkillScripts.Patches;

internal class RightAfterGetAnyBuff
{
    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterGetAnyBuff))]
	[HarmonyPostfix]
	private static void Postfix_BattleUnitModel_RightAfterGetAnyBuff(BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, int activeRound, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, int overStack, int overTurn, BattleUnitModel __instance)
    {
        // MainClass.Logg.LogInfo("Patch timing: RightAfterGetAnyBuff");

        // MainClass.Logg.LogWarning(keyword);
        // MainClass.Logg.LogWarning(stack);
        // MainClass.Logg.LogWarning(turn);
        // MainClass.Logg.LogWarning(activeRound);
        // MainClass.Logg.LogWarning(srcType.ToString());
        // if (actionOrNull != null) MainClass.Logg.LogWarning(actionOrNull.GetSkillID());
        
        int actevent = MainClass.timingDict["OnGainBuff"];

        foreach(PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
                BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modpa.Pointer.ToInt64()];
                if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;

                MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);

                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                MTCustomScripts.Main.Instance.gainbuff_turn = turn;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = activeRound;
                MTCustomScripts.Main.Instance.gainbuff_source = srcType;
                modpa.Enact(__instance, null, null, null, actevent, timing);
            }
        }

        foreach(PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
                BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modpa.Pointer.ToInt64()];
                if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;

                MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);
                
                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                MTCustomScripts.Main.Instance.gainbuff_turn = turn;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = activeRound;
                MTCustomScripts.Main.Instance.gainbuff_source = srcType;
                modpa.Enact(__instance, null, null, null, actevent, timing);
            }
        }
    }
}