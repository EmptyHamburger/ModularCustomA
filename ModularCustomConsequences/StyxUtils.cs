using HarmonyLib;
using Lethe.Patches;

namespace MTCustomScripts
{
    public static class StyxUtils
    {
        public static Il2CppSystem.Predicate<T> GetSafePredicate<T>(this System.Predicate<T> systemPredicate)
        {
            System.IntPtr ptr = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(systemPredicate);

            return new Il2CppSystem.Predicate<T>(ptr);
        }

        public static SkillModel SafeGetAbility(Il2CppSystem.Predicate<BattleActionModel> actionPredicate, BattleUnitModel unit, int value)
        {
            SkillModel selectedSkill = null;

            if (unit._actionList.Find(actionPredicate) != null) selectedSkill = unit._actionList.Find(actionPredicate).Skill;
            else
                try { selectedSkill = unit._actionList[value].Skill; }
                catch (System.Exception ex)
                {
                    Main.Logger.LogError($"SafeGetAbility error: {ex}");
                }
            return selectedSkill;
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

                if (!stringAbilityArray[i].Contains(':'))
                {
                    currentAbility.scriptName = stringAbilityArray[i];
                    continue;
                }

                string[] fragmentedAbility = stringAbilityArray[i].Split(':');

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
