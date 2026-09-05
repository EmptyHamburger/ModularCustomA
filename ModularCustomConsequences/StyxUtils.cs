using HarmonyLib;
using Lethe;
using Lethe.Patches;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;
using Utils;
using static BattleUI.Abnormality.AbnormalityPartSkills;

namespace MTCustomScripts
{
    public static class StyxUtils
    {
        public static int MinMax(int original, int max = 0, int min = 0)
        {
            return Math.Max(Math.Min(max, original), min);
        }

        public static void UpdateStackValue(this BuffModel selectedBuff, int vanillaValue, int modifyValue, bool isAdd, int optionalNum, BattleUnitModel buffOwner = null)
        {
            if (isAdd)
            {
                if (vanillaValue != 1)
                {
                    selectedBuff._vanilaMaxStack += modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._vanilaMaxStack += modifyValue;
                }
                if (vanillaValue != 2)
                {
                    selectedBuff._maxStackAdder += modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._maxStack += modifyValue;
                }
            }
            else
            {
                if (vanillaValue != 1)
                {
                    selectedBuff._vanilaMaxStack = modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._vanilaMaxStack = modifyValue;
                }
                if (vanillaValue != 2)
                {
                    selectedBuff._maxStackAdder = modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._maxStack = modifyValue;
                }
            }

            if (optionalNum != 1) selectedBuff.SetMaxStackAndTurn(buffOwner);
        }

        public static void UpdateCountValue(this BuffModel selectedBuff, int vanillaValue, int modifyValue, bool isAdd, int optionalNum, BattleUnitModel buffOwner = null)
        {
            if (isAdd)
            {
                if (vanillaValue != 1)
                {
                    selectedBuff._vanilaMaxTurn += modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._vanilaMaxTurn += modifyValue;
                }
                if (vanillaValue != 2)
                {
                    selectedBuff._maxTurnAdder += modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._maxTurn += modifyValue;
                }
            }
            else
            {
                if (vanillaValue != 1)
                {
                    selectedBuff._vanilaMaxTurn = modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._vanilaMaxTurn = modifyValue;
                }
                if (vanillaValue != 2)
                {
                    selectedBuff._maxTurnAdder = modifyValue;
                    if (optionalNum != 2) selectedBuff._buffInfo._maxTurn = modifyValue;
                }
            }

            if (optionalNum != 1) selectedBuff.SetMaxStackAndTurn(buffOwner);
        }


        public static System.Collections.Generic.List<T> ToSystem<T>(this Il2CppSystem.Collections.Generic.List<T> il2cppList)
        {
            var count = il2cppList.Count;
            var array = new T[count];
            for (int i = 0; i < count; i++) array[i] = il2cppList[i];
            return new System.Collections.Generic.List<T>(array);
        }

        public static Il2CppSystem.Collections.Generic.List<T> ToIl2Cpp<T>(this System.Collections.Generic.List<T> systemList)
        {
            if (systemList == null) return null;
            int count = systemList.Count;
            var il2cppList = new Il2CppSystem.Collections.Generic.List<T>(count);
            for (int i = 0; i < count; i++) il2cppList.Add(systemList[i]);
            return il2cppList;
        }

        public static Il2CppSystem.Collections.Generic.List<T> ToDistinctIL2CPP<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            var result = new Il2CppSystem.Collections.Generic.List<T>(list.Count);
            var seen = new HashSet<T>();

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (seen.Add(item)) result.Add(item);
            }

            return result;
        }

        public static SkillModel GetSkill(this Il2CppSystem.Collections.Generic.List<BattleActionModel> actionList, int id)
        {
            SkillModel finalSkill = null;
            for (int i = 0; i < actionList.Count; i++)
            {
                finalSkill = actionList[i].Skill;
                if (finalSkill.GetID() == id) break;
                else finalSkill = null;
            }


            return finalSkill;
        }


        public static System.Collections.Generic.List<SkillModel> GetSkillList(this Il2CppSystem.Collections.Generic.List<BattleActionModel> actionList, int id, int max = 999)
        {
            System.Collections.Generic.List<SkillModel> finalSkillList = new(max);
            for (int i = 0; i < actionList.Count; i++)
            {
                SkillModel skill = actionList[i].Skill;
                if (skill.GetID() == id) finalSkillList.Add(skill);

                if (finalSkillList.Count >= max) break;
            }
            

            return finalSkillList;
        }

        public static System.Collections.Generic.List<SkillModel> GetMultipleSkillModel(this ModularSA modular, Il2CppSystem.Collections.Generic.List<BattleUnitModel> selectedUnitList, string skillTarget)
        {
            List<SkillModel> selectedSkillList = new();
            skillTarget = skillTarget.ToLowerInvariant();
            string[] splitSkillTarget = skillTarget.Split('|', StringSplitOptions.RemoveEmptyEntries);
            string methodSegment = "NotBeganEvenSection";

            for (int i = 0; i < splitSkillTarget.Length; i++)
            {
                try
                {
                    methodSegment = "Modular";

                    string currentSkillTarget = splitSkillTarget[i];

                    if (currentSkillTarget == "modularskill")
                    {
                        selectedSkillList.Add(modular?.modsa_skillModel);
                        continue;
                    }
                    else if (currentSkillTarget == "modularselfaction")
                    {
                        selectedSkillList.Add(modular?.modsa_selfAction?.Skill);
                        continue;
                    }
                    else if (currentSkillTarget == "modularoppoaction")
                    {
                        selectedSkillList.Add(modular?.modsa_oppoAction?.Skill);
                        continue;
                    }


                    methodSegment = currentSkillTarget + " / " + "Parsing and ID";
                    string[] skillSplit = currentSkillTarget.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    bool hasSplit = (skillSplit.Length > 2);


                    int startData = -1;
                    int firstExtractedInt = -1;
                    List<int> notedSkillId = new();

                    foreach (BattleUnitModel unit in selectedUnitList)
                    {
                        methodSegment = currentSkillTarget + " / " + skillSplit[0];
                        if (!hasSplit) selectedSkillList.Add(unit.UnitDataModel.GetSkillModel(modular.GetNumFromParamString(currentSkillTarget)));
                        else if (!currentSkillTarget.Contains("all")) selectedSkillList.Add(modular.GetSingleSkillModel(unit, currentSkillTarget));
                        else
                        {
                            if (startData == -1) startData = (skillSplit[0] == "activeaction" && skillSplit.Length >= 3) ? 1 : 0;
                            if (firstExtractedInt == -1) firstExtractedInt = modular.GetNumFromParamString(skillSplit[startData + 1]);

                            switch (skillSplit[startData])
                            {
                                case "idall":
                                    notedSkillId.Add(firstExtractedInt);
                                    break;


                                case "dall":
                                    notedSkillId.Add(unit.GetDefenseSkillIDList()[firstExtractedInt]);
                                    break;

                                case "sall":
                                    if (skillSplit.Length > startData + 2) notedSkillId.Add(unit.GetSkillIdByTier(firstExtractedInt)[modular.GetNumFromParamString(skillSplit[startData + 2])]);
                                    else notedSkillId.Add(unit.GetSkillIdByTier(firstExtractedInt)[0]);
                                    break;

                                case "atkall":
                                    foreach (SkillModel skill in unit.GetSkillList()) if (skill.IsAttack()) notedSkillId.Add(skill.GetID());
                                    break;

                                case "defall":
                                    notedSkillId.AddRange((IEnumerable<int>)unit.GetDefenseSkillIDList());
                                    break;

                                case "all":
                                    foreach (SkillModel skill in unit.GetSkillList()) notedSkillId.Add(skill.GetID());
                                    break;
                            }
                        }

                        if (notedSkillId.Count <= 0) continue;

                        foreach (int skillId in notedSkillId.Distinct())
                        {
                            if (startData > 0) selectedSkillList.AddRange(unit._sortedActionList.GetSkillList(skillId));
                            else selectedSkillList.Add(unit.UnitDataModel.GetSkillModel(skillId));
                        }
                        notedSkillId.Clear();
                    }

                }
                catch (System.Exception ex)
                {
                    MainClass.Logg.LogError($"GetSingleSkillModel error for {methodSegment} Targetting: {ex}");
                }

            }

            return selectedSkillList.Distinct().ToList();
        }

        public static SkillModel GetSingleSkillModel(this ModularSA modular, BattleUnitModel selectedUnit, string skillTarget)
        {
            SkillModel selectedSkill = null;
            skillTarget = skillTarget.ToLowerInvariant();
            string methodSegment = "Modular";

            try
            {
                if (skillTarget == "modularskill") selectedSkill = modular?.modsa_skillModel;
                else if (skillTarget == "modularselfaction") selectedSkill = modular?.modsa_selfAction?.Skill;
                else if (skillTarget == "modularoppoaction") selectedSkill = modular?.modsa_oppoAction?.Skill;


                if (selectedSkill != null || selectedUnit == null) return selectedSkill;
                methodSegment = "Parsing and ID";

                string[] skillSplit = (skillTarget.Contains('-')) ? skillTarget.Split('-', StringSplitOptions.RemoveEmptyEntries) : null;
                if (skillSplit == null || skillSplit.Length < 2) return selectedUnit.UnitDataModel.GetSkillModel(modular.GetNumFromParamString(skillTarget));


                methodSegment = skillSplit[0];
                int startData = (methodSegment == "activeaction" && skillSplit.Length >= 3) ? 1 : 0;
                int skillId = -1;
                int firstExtractedInt = modular.GetNumFromParamString(skillSplit[startData + 1]);

                switch (skillSplit[startData])
                {
                    case "s":
                        if (skillSplit.Length < startData + 2) skillId = selectedUnit.GetSkillIdByTier(firstExtractedInt)[0];
                        else skillId = selectedUnit.GetSkillIdByTier(firstExtractedInt)[modular.GetNumFromParamString(skillSplit[startData + 2])];
                        break;


                    case "d":
                        skillId = selectedUnit.GetDefenseSkillIDList()[firstExtractedInt];
                        break;


                    case "current":
                        foreach (BattleActionModel action in selectedUnit.GetSortedActionList()) if (!action.isDoneWithAction)
                        {
                            selectedSkill = action.Skill;
                            break;
                        }
                        break;


                    case "id":
                        skillId = firstExtractedInt;
                        break;

                    case "index":
                        selectedSkill = selectedUnit._actionList[MinMax(firstExtractedInt, selectedUnit._actionList.Count)].Skill;
                        break;

                    default:
                        break;
                }

                if (startData > 0 && skillId >= 0) selectedSkill = selectedUnit._sortedActionList.GetSkill(skillId);
                else if (selectedSkill == null) selectedSkill = selectedUnit.UnitDataModel.GetSkillModel(skillId);

                if (selectedSkill == null) MainClass.Logg.LogError($"selectedSkill is null || skillId={skillId} || skillTarget={skillTarget} || SkillDataArray={skillSplit == null}");
            }
            catch (System.Exception ex)
            {
                MainClass.Logg.LogError($"GetSingleSkillModel error for {methodSegment} Targetting: {ex}");
            }

            return selectedSkill;
        }



        public static Il2CppSystem.Collections.Generic.List<CoinModel> GetMultipleCoin(this ModularSA modular, SkillModel selectedSkill, string coinTarget, CoinModel selectedSkillCoin = null)
        {
            Il2CppSystem.Collections.Generic.List<CoinModel> selectedCoinsList = new();
            coinTarget = coinTarget.ToLower();

            if (selectedSkillCoin == null)
            {
                if (modular.modsa_coinModel != null) selectedSkillCoin = modular.modsa_coinModel;
                else selectedSkillCoin = selectedSkill.GetCoinByIndex(0);
            }

            string[] splitCoinTarget = (coinTarget.Contains('|')) ? coinTarget.Split('|', System.StringSplitOptions.RemoveEmptyEntries) : new string[] { coinTarget };


            for (int i = 0; i < splitCoinTarget.Length; i++)
            {

                int preModularList = selectedCoinsList.Count;

                try
                {
                    if (splitCoinTarget[i] == "self" && selectedSkillCoin != null) selectedCoinsList.Add(selectedSkillCoin);
                    if (splitCoinTarget[i] == "others" && selectedSkillCoin != null) foreach (CoinModel modularCoin in selectedSkill.CoinList) if (modularCoin != selectedSkillCoin) selectedCoinsList.Add(modularCoin);
                    if (splitCoinTarget[i] == "all") foreach (CoinModel modularCoin in selectedSkill.CoinList) selectedCoinsList.Add(modularCoin);
                }
                catch (System.Exception ex)
                {
                    MainClass.Logg.LogError($"GetCoinModelList error at part-1: {ex}");
                }

                if (preModularList < selectedCoinsList.Count) continue;

                try
                {
                    string[] fragmentedSplitCoinTarget = (splitCoinTarget[i].Contains('-')) ? splitCoinTarget[i].Split("-") : null;
                    if (fragmentedSplitCoinTarget.Length < 2) continue;
                    int universalID = (fragmentedSplitCoinTarget.Length >= 2) ? modular.GetNumFromParamString(fragmentedSplitCoinTarget[1]) : 0;
                    bool lookupAll = fragmentedSplitCoinTarget[0].EndsWith("all");

                    if (Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(fragmentedSplitCoinTarget[1], true, out COIN_COLOR_TYPE parsedColor))
                    {
                        for (int y = 0; y < selectedSkill.CoinList.Count; y++)
                        {
                            CoinModel currentColorCoin = selectedSkill.CoinList[y];
                            if (currentColorCoin.GetCoinColor() == parsedColor)
                            {
                                selectedCoinsList.Add(currentColorCoin);
                                if (!lookupAll) break;
                            }
                        }

                    }

                    if (bool.TryParse(fragmentedSplitCoinTarget[1], out bool parsedBool))
                    {
                        var coinList = selectedSkill.CoinList;

                        for (int y = 0; y < coinList.Count; y++)
                        {
                            CoinModel coin = coinList[y];
                            bool match = false;

                            switch (fragmentedSplitCoinTarget[0])
                            {
                                case "rerolled":
                                case "rerolledall":
                                    match = coin.IsReRolled() == parsedBool;
                                    break;

                                case "head":
                                case "headall":
                                    match = coin.IsHead() == parsedBool;
                                    break;

                                case "active":
                                case "activeall":
                                    match = coin.IsActive() == parsedBool;
                                    break;

                                case "blooddinner":
                                case "blooddinnerall":
                                    match = coin.IsAddBloodDinner() == parsedBool;
                                    break;

                                case "consumebullet":
                                case "consumebulletall":
                                    match = coin.IsConsumeBullet() == parsedBool;
                                    break;

                                case "usableduel":
                                case "usableduelall":
                                    match = coin.IsUsableInDuel == parsedBool;
                                    break;

                                case "destroyablecoin":
                                case "destroyablecoinall":
                                    match = coin.IsDestroyableCoin(coin) == parsedBool;
                                    break;
                            }

                            if (!match) continue;

                            if (lookupAll) selectedCoinsList.Add(coin);
                            else
                            {
                                selectedCoinsList.Add(coin);
                                break;
                            }
                        }
                    }

                    if (preModularList < selectedCoinsList.Count) continue;

                    switch (fragmentedSplitCoinTarget[0])
                    {

                        case "endorigin":
                            selectedCoinsList.Add(selectedSkill.GetCoinByIndex(MinMax(universalID, selectedSkill.CoinList.Count)));
                            break;

                        case "startreal":
                            selectedCoinsList.Add(selectedSkill.GetCoin(universalID));
                            break;

                        case "endreal":
                            selectedCoinsList.Add(selectedSkill.GetCoin(MinMax(universalID, selectedSkill.CoinList.Count)));
                            break;

                        default:
                            selectedCoinsList.Add(selectedSkill.GetCoinByIndex(universalID));
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    MainClass.Logg.LogError($"GetCoinModelList error at part-2: {ex}");
                }
            }



            return selectedCoinsList.ToDistinctIL2CPP();
        }

        public static CoinModel GetSingleCoin(this ModularSA modular, SkillModel selectedSkill, string coinTarget, CoinModel selectedSkillCoin = null)
        {
            coinTarget = coinTarget.ToLower();

            if (selectedSkillCoin == null)
            {
                if (modular.modsa_coinModel != null) selectedSkillCoin = modular.modsa_coinModel;
                else selectedSkillCoin = selectedSkill.GetCoinByIndex(0);
            }

            if (coinTarget == "self" && selectedSkillCoin != null) return selectedSkillCoin;

            CoinModel finalCoin = null;

            try
            {
                string[] fragmentedCoinTarget = (coinTarget.Contains('-')) ? coinTarget.Split("-") : null;
                if (fragmentedCoinTarget.Length < 2) return null;
                int universalID = (fragmentedCoinTarget.Length >= 2) ? modular.GetNumFromParamString(fragmentedCoinTarget[1]) : 0;

                if (Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(fragmentedCoinTarget[1], true, out COIN_COLOR_TYPE parsedColor))
                {
                    for (int y = 0; y < selectedSkill.CoinList.Count; y++)
                    {
                        CoinModel currentColorCoin = selectedSkill.CoinList[y];
                        if (currentColorCoin.GetCoinColor() == parsedColor) finalCoin = currentColorCoin;
                    }

                }

                if (bool.TryParse(fragmentedCoinTarget[1], out bool parsedBool))
                {
                    var coinList = selectedSkill.CoinList;

                    for (int y = 0; y < coinList.Count; y++)
                    {
                        CoinModel coin = coinList[y];
                        bool match = false;

                        switch (fragmentedCoinTarget[0])
                        {
                            case "rerolled":
                                match = coin.IsReRolled() == parsedBool;
                                break;

                            case "head":
                                match = coin.IsHead() == parsedBool;
                                break;

                            case "active":
                                match = coin.IsActive() == parsedBool;
                                break;

                            case "blooddinner":
                                match = coin.IsAddBloodDinner() == parsedBool;
                                break;

                            case "consumebullet":
                                match = coin.IsConsumeBullet() == parsedBool;
                                break;

                            case "usableduel":
                                match = coin.IsUsableInDuel == parsedBool;
                                break;

                            case "destroyablecoin":
                                match = coin.IsDestroyableCoin(coin) == parsedBool;
                                break;
                        }

                        if (!match) continue;
                        finalCoin = coin;
                    }
                }

                finalCoin = fragmentedCoinTarget[0] switch
                {
                    "endorigin" => selectedSkill.GetCoinByIndex(MinMax(universalID, selectedSkill.CoinList.Count)),
                    "startreal" => selectedSkill.GetCoin(universalID),
                    "endreal" => selectedSkill.GetCoin(MinMax(universalID, selectedSkill.CoinList.Count)),
                    _ => selectedSkill.GetCoinByIndex(universalID),
                };
            }
            catch (System.Exception ex)
            {
                MainClass.Logg.LogError($"GetCoinModelList error at part-2: {ex}");
            }



            return finalCoin;
        }

        public static void GenericModularPatches(BattleUnitModel __instance, int actevent, int actevent_other, BATTLE_EVENT_TIMING timing, SkillModel skillModel_inst = null, BattleActionModel selfAction = null, BattleActionModel oppoAction = null, BattleUnitModel killer = null)
        {
            foreach (PassiveModel passiveModel in __instance._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    if (killer != null) modpa.modsa_killerModel = killer;
                    modpa.Enact(__instance, skillModel_inst, selfAction, oppoAction, actevent, timing);
                }
            }

            foreach (PassiveModel passiveModel in __instance._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    if (killer != null) modpa.modsa_killerModel = killer;
                    modpa.Enact(__instance, skillModel_inst, selfAction, oppoAction, actevent, timing);
                }
            }

            foreach (BuffModel buffModel in __instance._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    if (killer != null) modba.modsa_killerModel = killer;
                    modba.Enact(__instance, skillModel_inst, selfAction, oppoAction, actevent, timing);
                }
            }

            if (actevent_other == 0) return;

            BattleObjectManager battleObjManager_inst = SingletonBehavior<BattleObjectManager>.Instance;
            foreach (BattleUnitModel unit in battleObjManager_inst.GetAliveListExceptSelf(__instance, false, false))
            {
                foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        modpa.modsa_passiveModel = passiveModel;
                        if (killer != null) modpa.modsa_killerModel = killer;
                        modpa.modsa_victimModel = __instance;
                        modpa.Enact(unit, skillModel_inst, selfAction, oppoAction, actevent, timing);
                    }
                }
                foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
                {
                    if (!passiveModel.CheckActiveCondition()) continue;
                    long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                    foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                    {
                        modpa.modsa_passiveModel = passiveModel;
                        if (killer != null) modpa.modsa_killerModel = killer;
                        modpa.modsa_victimModel = __instance;
                        modpa.Enact(unit, skillModel_inst, selfAction, oppoAction, actevent, timing);
                    }
                }
                foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll())
                {
                    long buffmodel_intlong = buffModel.Pointer.ToInt64();
                    if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                    foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                    {
                        modba.modsa_buffModel = buffModel;
                        if (killer != null) modba.modsa_killerModel = killer;
                        modba.modsa_victimModel = __instance;
                        modba.Enact(unit, skillModel_inst, selfAction, oppoAction, actevent, timing);
                    }
                }
            }
        }

        /*
        public static void TreatCoinAbilities(string fullAbility)
        {
            string[] stringAbilityArray = new string[0];

            if (!fullAbility.Contains('|'))
            {
                stringAbilityArray.AddToArray<string>(fullAbility);
            }

            stringAbilityArray.AddRangeToArray<string>(fullAbility.Split(['|'], System.StringSplitOptions.RemoveEmptyEntries));

            for (int i = 0; i < stringAbilityArray.Length; i++)
            {
                AbilityData currentAbility = new AbilityData();
                currentAbility.buffData = new BuffReferenceData();
                currentAbility.conditionalData = new ConditionalData();
                currentAbility.conditionalData.conditionBuffData = new BuffReferenceData();
                currentAbility.conditionalData.resultBuffData = new BuffReferenceData();


                if (!stringAbilityArray[i].Contains(';'))
                {
                    currentAbility.scriptName = stringAbilityArray[i];
                    continue;
                }

                string[] fragmentedAbility = stringAbilityArray[i].Split(';');

                int abilityTurnLimit = 0;
                int abilityBuffStack = 0;
                int abilityBuffTurn = 0;
                int abilityBuffActiveRound = 0;
                int abilityBuffLimit = 0;
                int abilityConditionalIntValue = 0;
                int abilityConditionalIntResultValue = 0;

                float abilityValue = 0;
                float abilityBuffFloatValue = 0f;
                float abilityConditionalFloatValue = 0f;
                float abilityConditionalFloatResultValue = 0f;



                foreach (string ability in fragmentedAbility)
                {
                    if (ability.StartsWith("ScriptName", System.StringComparison.OrdinalIgnoreCase)) currentAbility.scriptName = ability.Substring("ScriptName".Length + 1);
                    else if (ability.StartsWith("BuffOwner", System.StringComparison.OrdinalIgnoreCase)) currentAbility.buffData.buffOwner = ability.Substring("BuffOwner".Length + 1);
                    else if (ability.StartsWith("BuffKeyword", System.StringComparison.OrdinalIgnoreCase)) currentAbility.buffData.buffKeyword = ability.Substring("BuffKeyword".Length + 1);
                    else if (ability.StartsWith("BuffTarget", System.StringComparison.OrdinalIgnoreCase)) currentAbility.buffData.target = ability.Substring("BuffTarget".Length + 1);
                    else if (ability.StartsWith("ConditionalCategory", System.StringComparison.OrdinalIgnoreCase)) currentAbility.conditionalData.category = ability.Substring("ConditionalCategory".Length + 1);
                    else if (ability.StartsWith("ConditionalType", System.StringComparison.OrdinalIgnoreCase)) currentAbility.conditionalData.type = ability.Substring("ConditionalType".Length + 1);
                    else if (ability.StartsWith("ConditionalValue", System.StringComparison.OrdinalIgnoreCase)) currentAbility.conditionalData.value = ability.Substring("ConditionalValue".Length + 1);


                    else if (ability.StartsWith("TurnLimit", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("TurnLimit".Length + 1), out abilityTurnLimit);
                    else if (ability.StartsWith("BuffStack", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("BuffStack".Length + 1), out abilityBuffStack);
                    else if (ability.StartsWith("BuffTurn", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("BuffTurn".Length + 1), out abilityBuffTurn);
                    else if (ability.StartsWith("BuffActiveRound", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("BuffActiveRound".Length + 1), out abilityBuffActiveRound);
                    else if (ability.StartsWith("BuffLimit", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("BuffLimit".Length + 1), out abilityBuffLimit);
                    else if (ability.StartsWith("ConditionalIntegerResultValue", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("ConditionalIntegerValue".Length + 1), out abilityConditionalIntResultValue);
                    else if (ability.StartsWith("ConditionalIntegerValue", System.StringComparison.OrdinalIgnoreCase)) int.TryParse(ability.Substring("ConditionalIntegerValue".Length + 1), out abilityConditionalIntValue);


                    else if (ability.StartsWith("Value", System.StringComparison.OrdinalIgnoreCase)) float.TryParse(ability.Substring("Value".Length + 1), out abilityValue);
                    else if (ability.StartsWith("BuffFloatValue", System.StringComparison.OrdinalIgnoreCase)) float.TryParse(ability.Substring("BuffFloatValue".Length + 1), out abilityBuffFloatValue);
                    else if (ability.StartsWith("ConditionalFloatValue", System.StringComparison.OrdinalIgnoreCase)) float.TryParse(ability.Substring("ConditionalFloatValue".Length + 1), out abilityConditionalFloatValue);
                    else if (ability.StartsWith("ConditionalFloatResultValue", System.StringComparison.OrdinalIgnoreCase)) float.TryParse(ability.Substring("ConditionalFloatValue".Length + 1), out abilityConditionalFloatResultValue);
                }


                currentAbility.value = abilityValue;
                currentAbility.turnLimit = abilityTurnLimit;
                currentAbility.buffData.stack = abilityBuffStack;
                currentAbility.buffData.turn = abilityBuffTurn;
                currentAbility.buffData.activeRound = abilityBuffActiveRound;
                currentAbility.buffData.limit = abilityBuffLimit;
                currentAbility.conditionalData._intValue = new Il2CppSystem.Nullable<int>(abilityConditionalIntValue);
                currentAbility.conditionalData._intResultValue = new Il2CppSystem.Nullable<int>(abilityConditionalIntResultValue);
                currentAbility.conditionalData._floatValue = new Il2CppSystem.Nullable<float>(abilityConditionalFloatValue);
                currentAbility.conditionalData._floatResultValue = new Il2CppSystem.Nullable<float>(abilityConditionalFloatResultValue);


                BUFF_UNIQUE_KEYWORD buffKeyword = CustomBuffs.ParseBuffUniqueKeyword(currentAbility.buffData.buffKeyword);
                if (buffKeyword == BUFF_UNIQUE_KEYWORD.None) currentAbility.buffData.buffKeyword = string.Empty;
                currentAbility.buffData._buffKeyword = buffKeyword;

                coinData.abilityScriptList.Add(currentAbility);
            }
        }
        */

        public static BuffReferenceData TreatBuffData(string fullAbility)
        {
            BuffReferenceData buffData = new();
            string[] fragmentedBuff = fullAbility.Split([';'], StringSplitOptions.RemoveEmptyEntries);

            int buffStack = 0;
            int buffTurn = 0;
            int buffActiveRound = 0;
            int buffLimit = 0;
            float buffValue = 0f;

            foreach (string value in fragmentedBuff)
            {
                if (value.StartsWith("BuffOwner", StringComparison.OrdinalIgnoreCase) && value.Length > 10) buffData.buffOwner = value[10..];
                else if (value.StartsWith("BuffTarget", StringComparison.OrdinalIgnoreCase) && value.Length > 11) buffData.target = value[11..];
                else if (value.StartsWith("BuffKeyword", StringComparison.OrdinalIgnoreCase) && value.Length > 12) buffData.buffKeyword = value[12..];

                else if (value.StartsWith("BuffStack", StringComparison.OrdinalIgnoreCase) && value.Length > 9) _ = int.TryParse(value[9..], out buffStack);
                else if (value.StartsWith("BuffTurn", StringComparison.OrdinalIgnoreCase) && value.Length > 8) _ = int.TryParse(value[8..], out buffTurn);
                else if (value.StartsWith("BuffActiveRound", StringComparison.OrdinalIgnoreCase) && value.Length > 15) _ = int.TryParse(value[15..], out buffActiveRound);
                else if (value.StartsWith("BuffLimit", StringComparison.OrdinalIgnoreCase) && value.Length > 9) _ = int.TryParse(value[9..], out buffLimit);
                else if (value.StartsWith("BuffValue", StringComparison.OrdinalIgnoreCase) && value.Length > 9) _ = float.TryParse(value[9..], out buffValue);
            }


            buffData.stack = buffStack;
            buffData.turn = buffTurn;
            buffData.activeRound = buffActiveRound;
            buffData.limit = buffLimit;
            buffData.value = buffValue;


            BUFF_UNIQUE_KEYWORD buffKeyword = CustomBuffs.ParseBuffUniqueKeyword(buffData.buffKeyword);
            if (buffKeyword == BUFF_UNIQUE_KEYWORD.None) buffData.buffKeyword = string.Empty;
            buffData._buffKeyword = buffKeyword;

            return buffData;
        }
    }
}