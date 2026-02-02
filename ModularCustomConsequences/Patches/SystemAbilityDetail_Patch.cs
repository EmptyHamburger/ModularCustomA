using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem;

namespace MTCustomScripts.Patches
{
    public class SystemAbilityDetail_Patch
    {
        public static bool CheckOverwriteAbility(SYSTEM_ABILITY_KEYWORD systemKeyword, out CustomSystemAbility __result)
        {
            int newKeywordValue = (int)systemKeyword;

            if (!Il2CppSystem.Enum.IsDefined(SystemAbilityKeywordEnumType, newKeywordValue) && CustomSystemAbilities_Main.customSystemAbilityDict.TryGetValue(newKeywordValue, out CustomSystemAbility customSystemAbility))
            {
                Main.Logger.LogInfo($"Succesfully recovered copy of customAbility with ID={customSystemAbility.GetCustomIdentifier()} and name={customSystemAbility.GetCustomNameId()}");
                __result = customSystemAbility.Copy();
                return false;
            }

            __result = null;
            return true;
        }


        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.AddAbilityThisRound))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_AddAbilityThisRound(int unitInstanceID, SYSTEM_ABILITY_KEYWORD newKeyword, int stack, int turn, SystemAbilityDetail __instance, ref SystemAbility __result)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
            customAbility.SetStack(stack, turn);
            customAbility.Init(unitInstanceID);
            __result = customAbility;
            return false;
        }

        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.AddAbilityNextRound))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_AddAbilityNextRound(int unitInstanceID, SYSTEM_ABILITY_KEYWORD newKeyword, int stack, int turn, SystemAbilityDetail __instance, ref SystemAbility __result)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
            customAbility.SetStack(stack, turn);
            customAbility.Init(unitInstanceID);
            __result = customAbility;
            return false;
        }

        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.CreateNewSystemAbility))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_CreateNewSystemAbility(SYSTEM_ABILITY_KEYWORD newKeyword, int stack, int turn, SystemAbilityDetail __instance, ref SystemAbility __result)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
            customAbility.SetStack(stack, turn);
            __result = customAbility;
            return false;
        }

        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.DestoryAbility))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_DestoryAbility(SYSTEM_ABILITY_KEYWORD newKeyword, SystemAbilityDetail __instance)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
                SystemAbility currentActiveCustom = __instance._activatedAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
                if (currentActiveCustom != null)
                {
                    currentActiveCustom.Destroy();
                    __instance._activatedAbilityList.Remove(currentActiveCustom);
                }

                SystemAbility currentNextCustom = __instance._nextTurnAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
                if (currentNextCustom != null)
                {
                    currentNextCustom.Destroy();
                    __instance._nextTurnAbilityList.Remove(currentNextCustom);
                }
            return false;
        }

        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.DestroyAbility))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_DestroyAbility(SYSTEM_ABILITY_KEYWORD newKeyword, SystemAbilityDetail __instance)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
                SystemAbility currentActiveCustom = __instance._activatedAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
                if (currentActiveCustom != null)
                {
                    currentActiveCustom.Destroy();
                    __instance._activatedAbilityList.Remove(currentActiveCustom);
                }

                SystemAbility currentNextCustom = __instance._nextTurnAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
                if (currentNextCustom != null)
                {
                    currentNextCustom.Destroy();
                    __instance._nextTurnAbilityList.Remove(currentNextCustom);
                }
            return false;
        }

        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.FindOrAddAbilityThisRound))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_FindOrAddAbilityThisRound(int instanceID, SYSTEM_ABILITY_KEYWORD newKeyword, int stack, int turn, SystemAbilityDetail __instance, ref SystemAbility __result)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
            SystemAbility currentActiveCustom = __instance._activatedAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
            if (currentActiveCustom != null) __result = currentActiveCustom;
            else
            {
                customAbility.SetStack(stack, turn);
                customAbility.Init(instanceID);
                __result = customAbility;
            }
            return false;
        }

        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.FindOrAddAbilityNextRound))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_FindOrAddAbilityNextRound(int instanceID, SYSTEM_ABILITY_KEYWORD newKeyword, int stack, int turn, SystemAbilityDetail __instance, ref SystemAbility __result)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
            SystemAbility nextTurnCustom = __instance._nextTurnAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
            if (nextTurnCustom != null) __result = nextTurnCustom;
            else
            {
                customAbility.SetStack(stack, turn);
                customAbility.Init(instanceID);
                __result = customAbility;
            }
            return false;
        }


        [HarmonyPatch(typeof(SystemAbilityDetail), nameof(SystemAbilityDetail.HasAbility))]
        [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
        public static bool Prefix_SystemAbilityDetail_HasAbility(SYSTEM_ABILITY_KEYWORD newKeyword, SystemAbilityDetail __instance, ref bool __result)
        {
            if (!CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility)) return true;
            SystemAbility currentActiveCustom = __instance._activatedAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
            if (currentActiveCustom != null) __result = true;
            else
            {
                SystemAbility nextTurnCustom = __instance._nextTurnAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == (customAbility as CustomSystemAbility).GetCustomIdentifier());
                if (nextTurnCustom != null) __result = true;
                else return true;
            }

            return false;
        }


        public static Il2CppSystem.Type SystemAbilityKeywordEnumType = Il2CppSystem.Type.GetType(nameof(SYSTEM_ABILITY_KEYWORD), true);
    }
}
