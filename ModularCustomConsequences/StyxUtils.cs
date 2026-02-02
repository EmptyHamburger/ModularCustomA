using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;
using System;

namespace MTCustomScripts
{
    public static class StyxUtils
    {
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

        public static Il2CppSystem.Collections.Generic.List<SkillModel> GetMultipleSkillModel(this ModularSA modular, Il2CppSystem.Collections.Generic.List<BattleUnitModel> selectedUnitList, string skillTarget)
        {
            Il2CppSystem.Collections.Generic.List<SkillModel> selectedSkillList = new Il2CppSystem.Collections.Generic.List<SkillModel>();
            System.Collections.Generic.List<BattleActionModel> selectedActionList = new System.Collections.Generic.List<BattleActionModel>();
            string[] splitSkillTarget = (skillTarget.Contains('|')) ? skillTarget.Split('|', System.StringSplitOptions.RemoveEmptyEntries) : new string[] { skillTarget };

            for (int i = 0; i < splitSkillTarget.Length; i++)
            {
                int preModularCount = selectedSkillList.Count;
                try
                {
                    if (splitSkillTarget[i].Equals("ModularSkill")) selectedSkillList.Add(modular?.modsa_skillModel);
                    if (splitSkillTarget[i].Equals("ModularSelfAction")) selectedSkillList.Add(modular?.modsa_selfAction?.Skill);
                    if (splitSkillTarget[i].Equals("ModularOppoAction")) selectedSkillList.Add(modular?.modsa_oppoAction?.Skill);
                }
                catch (System.Exception ex)
                {
                    MainClass.Logg.LogError($"GetMultipleSkillModel error for Part-1: {ex}");
                }

                if (preModularCount < selectedSkillList.Count) continue;

                foreach (BattleUnitModel selectedUnit in selectedUnitList)
                {
                    int skillId = 0;
                    string[] skillIdSplit = new string[1];

                    try
                    {

                        skillId = 0;
                        skillIdSplit = (splitSkillTarget[i].Contains('-')) ? splitSkillTarget[i].Substring(1).Split("-") : null;

                        if (splitSkillTarget[i].StartsWith("S", System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (skillIdSplit.Length == 2) skillId = selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillIdSplit[0].Substring(1)))[modular.GetNumFromParamString(skillIdSplit[1])];
                            else skillId = selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(splitSkillTarget[i].Substring(1)))[0];
                        }
                        else if (splitSkillTarget[i].StartsWith("D", System.StringComparison.OrdinalIgnoreCase)) skillId = selectedUnit.GetDefenseSkillIDList()[modular.GetNumFromParamString(splitSkillTarget[i].Substring(1))];
                        else if (splitSkillTarget[i].StartsWith("Current", System.StringComparison.OrdinalIgnoreCase) && selectedUnit._actionList.Count >= modular.GetNumFromParamString(skillIdSplit[1]) && skillIdSplit != null) skillId = selectedUnit._actionList[modular.GetNumFromParamString(skillIdSplit[1])].GetSkillID();

                        else if (splitSkillTarget[i].StartsWith("ActiveAction", System.StringComparison.OrdinalIgnoreCase) && skillIdSplit.Length >= 2)
                        {
                            skillIdSplit[0] = splitSkillTarget[i].Substring("ActiveAction".Length);
                            if (skillIdSplit[0].Equals("Index", System.StringComparison.OrdinalIgnoreCase) && selectedUnit._actionList.Count >= modular.GetNumFromParamString(skillIdSplit[1])) selectedSkillList.Add(selectedUnit._actionList[modular.GetNumFromParamString(skillIdSplit[1])].Skill);
                            else if (skillIdSplit[0].Equals("ID", System.StringComparison.OrdinalIgnoreCase)) selectedSkillList.Add(selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == modular.GetNumFromParamString(skillIdSplit[1])).Skill);
                            else if (skillIdSplit[0].Equals("D", System.StringComparison.OrdinalIgnoreCase)) selectedSkillList.Add(selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == selectedUnit.GetDefenseSkillIDList()[modular.GetNumFromParamString(skillIdSplit[1].Substring(1))]).Skill);
                            else if (skillIdSplit[0].Equals("S", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (skillIdSplit.Length == 3) selectedSkillList.Add(selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillIdSplit[1].Substring(1)))[modular.GetNumFromParamString(skillIdSplit[2])]).Skill);
                                else selectedSkillList.Add(selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(splitSkillTarget[i].Substring(1)))[0]).Skill);
                            }
                            else if (skillIdSplit[0].Equals("IDALL", System.StringComparison.OrdinalIgnoreCase)) selectedActionList.AddRange(selectedUnit._actionList.ToSystem().FindAll(x => x.GetSkillID() == modular.GetNumFromParamString(skillIdSplit[1])));
                            else if (skillIdSplit[0].Equals("DALL", System.StringComparison.OrdinalIgnoreCase)) selectedActionList.AddRange(selectedUnit._actionList.ToSystem().FindAll(x => x.GetSkillID() == selectedUnit.GetDefenseSkillIDList()[modular.GetNumFromParamString(skillIdSplit[1].Substring(3))]));
                            else if (skillIdSplit[0].Equals("SALL", System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (skillIdSplit.Length == 3) selectedActionList.AddRange(selectedUnit._actionList.ToSystem().FindAll(x => x.GetSkillID() == selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillIdSplit[1].Substring(3)))[modular.GetNumFromParamString(skillIdSplit[2])]));
                                else selectedActionList.AddRange(selectedUnit._actionList.ToSystem().FindAll(x => x.GetSkillID() == selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(splitSkillTarget[i].Substring(3)))[0]));
                            }
                        }
                        else skillId = modular.GetNumFromParamString(splitSkillTarget[i]);

                        if (selectedActionList.Count > 0)
                        {
                            foreach (BattleActionModel selectedAction in selectedActionList) selectedSkillList.Add(selectedAction.Skill);
                            selectedActionList.Clear();
                        }

                        if (skillId > 0) selectedSkillList.Add(selectedUnit.UnitDataModel.GetSkillModel(skillId));
                    }
                    catch (System.Exception ex)
                    {
                        MainClass.Logg.LogError($"GetSingleSkillModel error for Part-2: {ex}");
                    }
                }
            }

            return selectedSkillList;
        }

        public static SkillModel GetSingleSkillModel(this ModularSA modular, BattleUnitModel selectedUnit, string skillTarget)
        {
            SkillModel selectedSkill = null;

            try
            {
                if (skillTarget.Equals("ModularSkill")) selectedSkill = modular?.modsa_skillModel;
                if (skillTarget.Equals("ModularSelfAction")) selectedSkill = modular?.modsa_selfAction?.Skill;
                if (skillTarget.Equals("ModularOppoAction")) selectedSkill = modular?.modsa_oppoAction?.Skill;
            }
            catch (System.Exception ex)
            {
                MainClass.Logg.LogError($"GetSingleSkillModel error for Part-1: {ex}");
            }

            if (selectedSkill != null) return selectedSkill;

            int skillId = 0;
            string[] skillIdSplit = new string[1];

            try
            {

                skillId = 0;
                skillIdSplit = (skillTarget.Contains('-')) ? skillTarget.Substring(1).Split("-") : null;

                if (skillTarget.StartsWith("S", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (skillIdSplit.Length == 2) skillId = selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillIdSplit[0].Substring(1)))[modular.GetNumFromParamString(skillIdSplit[1])];
                    else skillId = selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillTarget.Substring(1)))[0];
                }
                else if (skillTarget.StartsWith("D", System.StringComparison.OrdinalIgnoreCase)) skillId = selectedUnit.GetDefenseSkillIDList()[modular.GetNumFromParamString(skillTarget.Substring(1))];
                else if (skillTarget.StartsWith("Current", System.StringComparison.OrdinalIgnoreCase) && selectedUnit._actionList.Count >= modular.GetNumFromParamString(skillIdSplit[1]) && skillIdSplit != null) skillId = selectedUnit._actionList[modular.GetNumFromParamString(skillIdSplit[1])].GetSkillID();

                else if (skillTarget.StartsWith("ActiveAction", System.StringComparison.OrdinalIgnoreCase) && skillIdSplit.Length >= 2)
                {
                    skillIdSplit[0] = skillTarget.Substring("ActiveAction".Length);
                    if (skillIdSplit[0].Equals("Index", System.StringComparison.OrdinalIgnoreCase) && selectedUnit._actionList.Count >= modular.GetNumFromParamString(skillIdSplit[1])) selectedSkill = selectedUnit._actionList[modular.GetNumFromParamString(skillIdSplit[1])].Skill;
                    else if (skillIdSplit[0].Equals("ID", System.StringComparison.OrdinalIgnoreCase)) selectedSkill = selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == modular.GetNumFromParamString(skillIdSplit[1])).Skill;
                    else if (skillIdSplit[0].Equals("D", System.StringComparison.OrdinalIgnoreCase)) selectedSkill = selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == selectedUnit.GetDefenseSkillIDList()[modular.GetNumFromParamString(skillIdSplit[1].Substring(1))]).Skill;
                    else if (skillIdSplit[0].Equals("S", System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (skillIdSplit.Length == 3) selectedSkill = selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillIdSplit[1].Substring(1)))[modular.GetNumFromParamString(skillIdSplit[2])]).Skill;
                        else selectedSkill = selectedUnit._actionList.ToSystem().Find(x => x.GetSkillID() == selectedUnit.GetSkillIdByTier(modular.GetNumFromParamString(skillTarget.Substring(1)))[0]).Skill;
                    }
                }

                else skillId = modular.GetNumFromParamString(skillTarget);

                if (skillId > 0) selectedSkill = selectedUnit.UnitDataModel.GetSkillModel(skillId);
            }
            catch (System.Exception ex)
            {
                MainClass.Logg.LogError($"GetSingleSkillModel error for Part-2: {ex}");
            }
            if (selectedSkill == null) MainClass.Logg.LogError($"selectedSkill is null || skillId={skillId} || skillTarget={skillTarget} || skillIdSplitIsNull={skillIdSplit == null}");
            return selectedSkill;
        }



        public static Il2CppSystem.Collections.Generic.List<CoinModel> GetCoinModelList(this ModularSA modular, SkillModel selectedSkill, string coinTarget, CoinModel selectedSkillCoin = null)
        {
            Il2CppSystem.Collections.Generic.List<CoinModel> selectedCoinsList = new Il2CppSystem.Collections.Generic.List<CoinModel>();
            System.Collections.Generic.List<CoinModel> temporarysCoinsList = new System.Collections.Generic.List<CoinModel>();
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
                    if (splitCoinTarget[i].StartsWith("SELF", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkillCoin);
                    if (splitCoinTarget[i].StartsWith("OTHERS", System.StringComparison.OrdinalIgnoreCase)) foreach (CoinModel modularCoin in selectedSkill.CoinList) if (modularCoin != selectedSkillCoin) selectedCoinsList.Add(modularCoin);
                    if (splitCoinTarget[i].StartsWith("ALL", System.StringComparison.OrdinalIgnoreCase)) foreach (CoinModel modularCoin in selectedSkill.CoinList) selectedCoinsList.Add(modularCoin);
                }
                catch (System.Exception ex)
                {
                    MainClass.Logg.LogError($"GetCoinModelList error at part-1: {ex}");
                }

                if (preModularList < selectedCoinsList.Count) continue;


                try
                {
                    string[] fragmentedSplitCoinTarget = (splitCoinTarget[i].Contains('-')) ? splitCoinTarget[i].Substring(1).Split("-") : null;

                    if (splitCoinTarget[i].StartsWith("ENDORIGIN", System.StringComparison.OrdinalIgnoreCase) && fragmentedSplitCoinTarget.Length == 2) selectedCoinsList.Add(selectedSkill.GetCoinByIndex(Math.Max(selectedSkill.CoinList.Count - modular.GetNumFromParamString(fragmentedSplitCoinTarget[1]), 0)));
                    else if (splitCoinTarget[i].StartsWith("STARTREAL", System.StringComparison.OrdinalIgnoreCase) && fragmentedSplitCoinTarget.Length == 2) selectedCoinsList.Add(selectedSkill.GetCoin(modular.GetNumFromParamString(splitCoinTarget[0])));
                    else if (splitCoinTarget[i].StartsWith("ENDREAL", System.StringComparison.OrdinalIgnoreCase) && fragmentedSplitCoinTarget.Length == 2) selectedCoinsList.Add(selectedSkill.GetCoin(Math.Max(selectedSkill.CoinList.Count - modular.GetNumFromParamString(fragmentedSplitCoinTarget[1]), 0)));

                    else if (splitCoinTarget[i].StartsWith("COLOR", StringComparison.OrdinalIgnoreCase) && Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(fragmentedSplitCoinTarget[1], true, out COIN_COLOR_TYPE parsedColor)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.GetCoinColor() == parsedColor));
                    else if (splitCoinTarget[i].StartsWith("COLORALL", StringComparison.OrdinalIgnoreCase) && Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(fragmentedSplitCoinTarget[1], true, out COIN_COLOR_TYPE parsedAllColor)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.GetCoinColor() == parsedAllColor);


                    if (bool.TryParse(fragmentedSplitCoinTarget[1].ToLower(), out bool parsedBool) && fragmentedSplitCoinTarget.Length == 2)
                    {
                        if (splitCoinTarget[i].StartsWith("REROLLED", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsReRolled() == parsedBool));
                        else if (splitCoinTarget[i].StartsWith("HEAD", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsHead() == parsedBool));
                        else if (splitCoinTarget[i].StartsWith("ACTIVE", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsActive() == parsedBool));
                        else if (splitCoinTarget[i].StartsWith("BLOODDINNER", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsAddBloodDinner() == parsedBool));
                        else if (splitCoinTarget[i].StartsWith("CONSUMEBULLET", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsConsumeBullet() == parsedBool));
                        else if (splitCoinTarget[i].StartsWith("USABLEDUEL", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsUsableInDuel == parsedBool));
                        else if (splitCoinTarget[i].StartsWith("DESTROYABLECOIN", System.StringComparison.OrdinalIgnoreCase)) selectedCoinsList.Add(selectedSkill.CoinList.ToSystem().Find(x => x.IsDestroyableCoin(x) == parsedBool));


                        else if (splitCoinTarget[i].StartsWith("REROLLLEDALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsReRolled() == parsedBool);
                        else if (splitCoinTarget[i].StartsWith("HEADALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsHead() == parsedBool);
                        else if (splitCoinTarget[i].StartsWith("ACTIVEALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsActive() == parsedBool);
                        else if (splitCoinTarget[i].StartsWith("BLOODDINNERALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsAddBloodDinner() == parsedBool);
                        else if (splitCoinTarget[i].StartsWith("CONSUMEBULLETALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsConsumeBullet() == parsedBool);
                        else if (splitCoinTarget[i].StartsWith("USABLEDUELALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsUsableInDuel == parsedBool);
                        else if (splitCoinTarget[i].StartsWith("DESTROYABLECOINALL", System.StringComparison.OrdinalIgnoreCase)) temporarysCoinsList = selectedSkill.CoinList.ToSystem().FindAll(x => x.IsDestroyableCoin(x) == parsedBool);
                    }
                    else selectedCoinsList.Add(selectedSkill.GetCoinByIndex(modular.GetNumFromParamString(splitCoinTarget[0])));

                    if (temporarysCoinsList.Count > 0)
                    {
                        foreach (CoinModel tempCoin in temporarysCoinsList) selectedCoinsList.Add(tempCoin);
                        temporarysCoinsList.Clear();
                    }
                }
                catch (System.Exception ex)
                {
                    MainClass.Logg.LogError($"GetCoinModelList error at part-2: {ex}");
                }
            }



            return selectedCoinsList;
        }

        public static void ProcessEnumOperation<T>(string value, System.Collections.Generic.List<T> list)  where T : unmanaged, Enum
        {
            if (string.IsNullOrEmpty(value) || value.Length < 2) return;

            char operation = char.ToLowerInvariant(value[0]);
            if (operation != 'a' && operation != 'r') return;

            string enumValue = value.Substring(1);
            if (Il2CppSystem.Enum.TryParse<T>(enumValue, true, out T enumResult))
            {
                if (operation == 'r') list.Remove(enumResult);
                else list.Add(enumResult);
            }
        }

        public static void TreatCoinAbilities(SkillCoinData coinData, string fullAbility)
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
    }
}