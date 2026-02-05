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
                if (lookupId == 0) return 0;
            }


            int finalResult = 0;
            try
            {
                ModularSystemAbility modularAbility;
                if (current == false && unit._systemAbilityDetail.HasSystemAbility((SYSTEM_ABILITY_KEYWORD)lookupId, out SystemAbility sa)) modularAbility = (sa as ModularSystemAbility);
                else if (current == true && unit._systemAbilityDetail.HasSystemAbility((SYSTEM_ABILITY_KEYWORD)lookupId, out System.Collections.Generic.List<SystemAbility> saList))
                {
                    modularAbility = (ModularSystemAbility)saList.Find(x => (x as ModularSystemAbility).currentModular == modular);
                }
                else return 0;

                if (circles[2] == "GetData") return (modularAbility.dataDictionary.TryGetValue(circles[3], out string storedData)) ? modular.GetNumFromParamString(storedData) : 0;


                object selectedItem = null;
                if (modularAbility.currentClassInfo.lookupDict.TryGetValue(circles[2], out var getter)) selectedItem = getter(modularAbility.currentClassInfo);
                if (selectedItem == null) return 0;

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
