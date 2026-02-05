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


            bool current = false;
            int lookupId = 0;
            if (circles[1].StartsWith("Current", StringComparison.OrdinalIgnoreCase))
            {
                current = true;
                circles[1] = circles[1].Substring(7);
            }

            if (current == false)
            {

                ModularSystemAbilityStaticData modularData = ModularSystemAbilityStaticDataList.Instance.GetData(circles[0]);
                if (modularData != null) lookupId = modularData.Id;
                else if (modularData == null)
                {
                    modularData = ModularSystemAbilityStaticDataList.Instance.GetData(circles[0]);
                    lookupId = modularData.Id;
                }
                if (lookupId == 0) return;
            }



            foreach (BattleUnitModel unit in unitList)
            {
                try
                {
                    ModularSystemAbility modularAbility;
                    if (current == false && unit._systemAbilityDetail.HasSystemAbility((SYSTEM_ABILITY_KEYWORD)lookupId, out SystemAbility sa)) modularAbility = (sa as ModularSystemAbility);
                    else if (current == true && unit._systemAbilityDetail.HasSystemAbility((SYSTEM_ABILITY_KEYWORD)lookupId, out System.Collections.Generic.List<SystemAbility> saList))
                    {
                        modularAbility = (ModularSystemAbility)saList.Find(x => (x as ModularSystemAbility).currentModular == modular);
                    }
                    else continue;

                    if (circles[2] == "SetData")
                    {
                        modularAbility.dataDictionary[circles[3]] = circles[4];
                        continue;
                    }


                    object selectedItem = null;
                    if (modularAbility.currentClassInfo.lookupDict.TryGetValue(circles[2], out var getter)) selectedItem = getter(modularAbility.currentClassInfo);
                    if (selectedItem == null) return;

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

                    modularAbility.editedParamList.Add(circles[2], selectedItem);
                }
                catch (System.Exception ex) { Main.Logger.LogError($"Unexpected error at SystemAbilityModularConsequence: {ex}"); }
            }
        }
    }
}