using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using MTCustomScripts;

namespace ModularSkillScripts.Patches;

internal class LoseAnyBuff
{
    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterLosingBuff))]
    [HarmonyPostfix]
    private static void Postfix_BattleUnitModel_RightAfterLosingBuff(int loseStack, int loseCount, BuffInfo loseBuffInfo, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
    {
        // MainClass.Logg.LogInfo("Patch timing: RightAfterGetAnyBuff");

        // MainClass.Logg.LogWarning(keyword);
        // MainClass.Logg.LogWarning(stack);
        // MainClass.Logg.LogWarning(turn);
        // MainClass.Logg.LogWarning(activeRound);
        // MainClass.Logg.LogWarning(srcType.ToString());
        // if (actionOrNull != null) MainClass.Logg.LogWarning(actionOrNull.GetSkillID());

        int actevent = MainClass.timingDict["OnLoseBuff"];

        foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modpa.Pointer.ToInt64())) continue;
                BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modpa.Pointer.ToInt64()];
                if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != loseBuffInfo.GetKeyword())) continue;

                MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);

                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                MTCustomScripts.Main.Instance.gainbuff_keyword = loseBuffInfo.GetKeyword();
                MTCustomScripts.Main.Instance.gainbuff_stack = loseStack;
                MTCustomScripts.Main.Instance.gainbuff_turn = loseCount;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = loseBuffInfo._activeRound;
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
                if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != loseBuffInfo.GetKeyword())) continue;

                MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);

                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                MTCustomScripts.Main.Instance.gainbuff_keyword = loseBuffInfo.GetKeyword();
                MTCustomScripts.Main.Instance.gainbuff_stack = loseStack;
                MTCustomScripts.Main.Instance.gainbuff_turn = loseCount;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = loseBuffInfo._activeRound;
                modpa.Enact(__instance, null, null, null, actevent, timing);
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightBeforeLosingBuff))]
    [HarmonyPostfix]
    private static void Postfix_BattleUnitModel_RightBeforeLosingBuff(BUFF_UNIQUE_KEYWORD keyword, int stack, int count, BATTLE_EVENT_TIMING timing, BattleUnitModel __instance)
    {
        // MainClass.Logg.LogInfo("Patch timing: RightAfterGetAnyBuff");

        // MainClass.Logg.LogWarning(keyword);
        // MainClass.Logg.LogWarning(stack);
        // MainClass.Logg.LogWarning(turn);
        // MainClass.Logg.LogWarning(activeRound);
        // MainClass.Logg.LogWarning(srcType.ToString());
        // if (actionOrNull != null) MainClass.Logg.LogWarning(actionOrNull.GetSkillID());

        int actevent = MainClass.timingDict["OnBeforeLoseBuff"];

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

                MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);

                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                MTCustomScripts.Main.Instance.gainbuff_turn = count;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = 0;
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

                MainClass.Logg.LogInfo("Founds modpassive - GainBuff timing: " + modpa.passiveID);

                modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(__instance, null, null, actionOrNull, actevent, timing);
                MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                MTCustomScripts.Main.Instance.gainbuff_turn = count;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = (__instance._buffDetail.GetActivatedBuff(keyword) != null) ? __instance._buffDetail.GetActivatedBuff(keyword)._activeRound : __instance._buffDetail.GetReadyBuff(keyword)._activeRound;
                modpa.Enact(__instance, null, null, null, actevent, timing);
            }
        }
    }
}