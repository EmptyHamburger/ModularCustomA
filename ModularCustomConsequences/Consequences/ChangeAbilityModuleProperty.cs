using ModularSkillScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceChangeAbilityModuleProperty : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: Multi-Target
             * var_2: System-Ability
             * var_3: Data-Category
             * var_4: Data-Type
             * var_5: New-Value
             */

            if (circles.Length < 5) return;

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList.Count <= 0) return;


            int lookupId = 0;
            ModularSystemAbilityStaticData modularData = ModularSystemAbilityStaticDataList.Instance.GetData(circles[0]);
            if (modularData != null) lookupId = modularData.Id;
            else if (modularData == null)
            {
                modularData = ModularSystemAbilityStaticDataList.Instance.GetData(circles[0]);
                lookupId = modularData.Id;
            }
            if (lookupId == 0) return;


            foreach (BattleUnitModel unit in unitList)
            {
                try
                {
                    if (!unit._systemAbilityDetail.HasSystemAbility((SYSTEM_ABILITY_KEYWORD)lookupId, out SystemAbility sa)) continue;
                    ModularSystemAbility modularAbility = (sa as ModularSystemAbility);

                    if (circles[2] == "SetData")
                    {
                        modularAbility.dataDictionary[circles[3]] = circles[4];
                        continue;
                    }


                    object selectedItem = null;

                    string dataCategory = circles[2].ToLower();
                    if (dataCategory == "skillpoweradder") selectedItem = modularAbility.currentClassInfo.getSkillPowerAdder;
                    else if (dataCategory == "skillpowerresultadder") selectedItem = modularAbility.currentClassInfo.getSkillPowerResultAdder;
                    else if (dataCategory == "parryingresultadder") selectedItem = modularAbility.currentClassInfo.getParryingResultAdder;
                    else if (dataCategory == "coinscaleadder") selectedItem = modularAbility.currentClassInfo.getCoinScaleAdder;
                    else if (dataCategory == "coinscalemultiplier") selectedItem = modularAbility.currentClassInfo.getCoinScaleMultiplier;
                    else if (dataCategory == "permanentatkresistadder") selectedItem = modularAbility.currentClassInfo.permanentAtkResistAdderDict;
                    else if (dataCategory == "temporaryatkresistadder") selectedItem = modularAbility.currentClassInfo.temporaryAtkResistAdderDict;
                    else if (dataCategory == "permanentsinresistadder") selectedItem = modularAbility.currentClassInfo.permanentSinResistAdderDict;
                    else if (dataCategory == "temporarysinresistadder") selectedItem = modularAbility.currentClassInfo.temporarySinResistAdderDict;
                    else if (dataCategory == "attackdmgadder") selectedItem = modularAbility.currentClassInfo.getAttackDmgAdder;
                    else if (dataCategory == "attackdmgmultiplier") selectedItem = modularAbility.currentClassInfo.getAttackDmgMultiplier;
                    else if (dataCategory == "takeattackdmgadder") selectedItem = modularAbility.currentClassInfo.getTakeAttackDmgAdder;
                    else if (dataCategory == "takeattackdmgmultiplier") selectedItem = modularAbility.currentClassInfo.getTakeAttackDmgMultiplier;
                    else if (dataCategory == "takempdmgadder") selectedItem = modularAbility.currentClassInfo.getTakeMpDmgAdder;
                    else if (dataCategory == "mentalsystemresultincreaseadder") selectedItem = modularAbility.currentClassInfo.getMentalSystemResultIncreaseAdder;
                    else if (dataCategory == "mentalsystemresultdecreaseadder") selectedItem = modularAbility.currentClassInfo.getMentalSystemResultDecreaseAdder;
                    else if (dataCategory == "forcedcoinresult") selectedItem = modularAbility.currentClassInfo.getForcedCoinResult;
                    else if (dataCategory == "ignoresinbuffhpdamage") selectedItem = modularAbility.currentClassInfo.ignoreSinBuffHpDamage;
                    else if (dataCategory == "permanentegoresourceadder") selectedItem = modularAbility.currentClassInfo.permanentEgoResourceAdderDict;
                    else if (dataCategory == "temporaryegoresourceadder") selectedItem = modularAbility.currentClassInfo.temporaryEgoResourceAdderDict;

                    if (selectedItem is ModularSystemAbilityStaticData_BundledParam dataBundle)
                    {
                        switch (circles[3].ToLower())
                        {
                            case "permanentdata":
                                dataBundle.permanentData = modular.GetNumFromParamString(circles[4]);
                                break;
                            case "temporarydata":
                                dataBundle.temporaryData = modular.GetNumFromParamString(circles[4]);
                                break;
                            case "permanentbanneddmgsource":
                                StyxUtils.ProcessEnumOperation<DAMAGE_SOURCE_TYPE>(circles[4], dataBundle.permanentBannedSourceTypeList);
                                break;
                            case "temporarybanneddmgsource":
                                StyxUtils.ProcessEnumOperation<DAMAGE_SOURCE_TYPE>(circles[4], dataBundle.temporaryBannedSourceTypeList);
                                break;
                            case "permanentbannedbuffkeyword":
                                StyxUtils.ProcessEnumOperation<BUFF_UNIQUE_KEYWORD>(circles[4], dataBundle.permanentBannedBuffKeywordList);
                                break;
                            case "temporarybannedbuffkeyword":
                                StyxUtils.ProcessEnumOperation<BUFF_UNIQUE_KEYWORD>(circles[4], dataBundle.temporaryBannedBuffKeywordList);
                                break;
                            default:
                                Main.Logger.LogError($"Bro you had ONE JOB, {circles[3]} is NOT A VALID ARGUMENT");
                                break;
                        }
                    }

                    else if (selectedItem is System.Collections.Generic.Dictionary<System.Enum, int> enumDict)
                    {
                        string[] splitDictEntry = circles[4].Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                        if (System.Enum.TryParse<ATK_BEHAVIOUR>(splitDictEntry[0], true, out ATK_BEHAVIOUR atkResult)) enumDict[atkResult] = modular.GetNumFromParamString(splitDictEntry[1]);
                        else if (System.Enum.TryParse<ATTRIBUTE_TYPE>(splitDictEntry[0], true, out ATTRIBUTE_TYPE attributeResult)) enumDict[attributeResult] = modular.GetNumFromParamString(splitDictEntry[1]);
                        else Main.Logger.LogError($"Fatal error on ENUM end: {enumDict.Values.Any().GetType()}");
                    }

                    modularAbility.editedParamList.Add(dataCategory, selectedItem);
                }
                catch (System.Exception ex) { Main.Logger.LogError($"Unexpected error at SystemAbilityModularConsequence: {ex}"); }
            }
        }
    }
}