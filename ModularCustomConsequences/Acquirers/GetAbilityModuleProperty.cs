using ModularSkillScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetAbilityModuleProperty : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: Single-Target
             * var_2: System-Ability
             * var_3: Data-Category
             * var_4: Data-Type
             */

            if (circles.Length < 5) return 0;

            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            if (unit == null) return 0;


            int lookupId = 0;
            ModularSystemAbilityStaticData modularData = ModularSystemAbilityStaticDataList.Instance.GetData(circles[0]);
            if (modularData != null) lookupId = modularData.Id;
            else if (modularData == null)
            {
                modularData = ModularSystemAbilityStaticDataList.Instance.GetData(circles[0]);
                lookupId = modularData.Id;
            }
            if (lookupId == 0) return 0;

            int finalResult = 0;
            try
            {
                if (!unit._systemAbilityDetail.HasSystemAbility((SYSTEM_ABILITY_KEYWORD)lookupId, out SystemAbility sa)) return 0;
                ModularSystemAbility modularAbility = (sa as ModularSystemAbility);

                if (circles[2] == "GetData") return (modularAbility.dataDictionary.TryGetValue(circles[3], out string storedData)) ? modular.GetNumFromParamString(storedData) : 0;

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
                            finalResult = dataBundle.permanentData;
                            break;
                        case "temporarydata":
                            finalResult = dataBundle.temporaryData;
                            break;
                        case "permanentbanneddmgsource":
                            finalResult = dataBundle.permanentBannedSourceTypeList.Count;
                            break;
                        case "temporarybanneddmgsource":
                            finalResult = dataBundle.temporaryBannedSourceTypeList.Count;
                            break;
                        case "permanentbannedbuffkeyword":
                            finalResult = dataBundle.permanentBannedBuffKeywordList.Count;
                            break;
                        case "temporarybannedbuffkeyword":
                            finalResult = dataBundle.temporaryBannedBuffKeywordList.Count;
                            break;
                        default:
                            Main.Logger.LogError($"Bro you had ONE JOB, {circles[3]} is NOT A VALID ARGUMENT");
                            break;
                    }
                }

                else if (selectedItem is System.Collections.Generic.Dictionary<System.Enum, int> enumDict)
                {
                    string[] splitDictEntry = circles[4].Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (System.Enum.TryParse<ATK_BEHAVIOUR>(splitDictEntry[0], true, out ATK_BEHAVIOUR atkResult)) enumDict.TryGetValue(atkResult, out finalResult);
                    else if (System.Enum.TryParse<ATTRIBUTE_TYPE>(splitDictEntry[0], true, out ATTRIBUTE_TYPE attributeResult)) enumDict.TryGetValue(attributeResult, out finalResult);
                    else Main.Logger.LogError($"Fatal error on ENUM end: {enumDict.Values.Any().GetType()}");
                }
            }
            catch (System.Exception ex) { Main.Logger.LogError($"Unexpected error at SystemAbilityModularAcquirer: {ex}"); }

            return finalResult;
        }
    }
}
