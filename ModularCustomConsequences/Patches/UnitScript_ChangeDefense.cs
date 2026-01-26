using HarmonyLib;
using ModularSkillScripts;
using Lua;
using ModularSkillScripts.Patches;
using BattleUI.Operation;
using BepInEx.Unity.IL2CPP.UnityEngine;

internal class Patch_DefenseChange
{
    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.GetOverwriteDefenseSkillID))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_GetOverwriteDefenseSkillID(BattleUnitModel unit, int slotIndex, ref int defenseSkillId, ref bool __result)
    {
        string Lkey = "AbsoluteMTCustomDefenseChangerSkillIdData";
        var dataKey = new LuaUnitDataKey
        {
            unitPtr_intlong = unit.Pointer.ToInt64(),
            dataID = Lkey
        };
        var skillId = LuaUnitDataKey.LuaUnitValues.TryGetValue(dataKey, out var value) ? value : LuaValue.Nil;
        if (skillId != LuaValue.Nil)
        {
            var debounceCheckKey = new LuaUnitDataKey
            {
                unitPtr_intlong = unit.Pointer.ToInt64(),
                dataID = "AbsoluteMTCustomDefenseChangerDebounceCheck"
            };
            LuaUnitDataKey.LuaUnitValues.TryGetValue(debounceCheckKey, out var debounceCheck);
            if (debounceCheck == true)
            {
                MTCustomScripts.Main.Logger.LogWarning("Defense changer script already activated, ran default vanilla script");
                return true;
            }
            if (skillId.TryRead<double>(out double DskillId))
            {
                MTCustomScripts.Main.Logger.LogMessage($"Defense skill id found '{(int) DskillId}'");
                defenseSkillId = (int) DskillId;
                __result = true;
                LuaUnitDataKey.LuaUnitValues[debounceCheckKey] = true;
                return false;
            }
            MTCustomScripts.Main.Logger.LogWarning("Defense skill id data is not an integer!");
        }
        MTCustomScripts.Main.Logger.LogWarning("Defense skill id data not found, ran default vanilla script");
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.CheckTransform))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_CheckTransform(BattleUnitModel unit, BATTLE_EVENT_TIMING timing)
    {
        var skillId = LuaUnitDataKey.LuaUnitValues.TryGetValue(new LuaUnitDataKey{unitPtr_intlong = unit.Pointer.ToInt64(), dataID = "AbsoluteMTCustomDefenseChangerSkillIdData"}, out var value) ? value : LuaValue.Nil;
        if (skillId != LuaValue.Nil) return false;
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.CheckTransformCondition))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_CheckTransformCondition(CombatUnitModel self)
    {
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.GetSkillIndexAdderForCountNumForRunTime))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_GetSkillIndexAdderForCountNumForRunTime(CombatUnitModel unit, int originIndex)
    {
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.OnRoundEnd))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_OnRoundEnd(BattleUnitModel unit, BATTLE_EVENT_TIMING timing)
    {
        var skillId = LuaUnitDataKey.LuaUnitValues.TryGetValue(new LuaUnitDataKey{unitPtr_intlong = unit.Pointer.ToInt64(), dataID = "AbsoluteMTCustomDefenseChangerSkillIdData"}, out var value) ? value : LuaValue.Nil;
        if (skillId != LuaValue.Nil) return false;
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.OnRoundStart))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_OnRoundStart(BattleUnitModel unit)
    {
        var skillId = LuaUnitDataKey.LuaUnitValues.TryGetValue(new LuaUnitDataKey{unitPtr_intlong = unit.Pointer.ToInt64(), dataID = "AbsoluteMTCustomDefenseChangerSkillIdData"}, out var value) ? value : LuaValue.Nil;
        if (skillId != LuaValue.Nil) return false;
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.SetPatternReload))]
    [HarmonyPrefix]
     private static bool Prefix_UnitScript_10913_SetPatternReload()
    {
        return true;
    }

    [HarmonyPatch(typeof(UnitScript_10913), nameof(UnitScript_10913.TryGetOriginalSkillByTier))]
    [HarmonyPrefix]
    private static bool Prefix_UnitScript_10913_TryGetOriginalSkillByTier(BattleUnitModel unit, int targetTier, ref int skillId)
    {
        var skillid = LuaUnitDataKey.LuaUnitValues.TryGetValue(new LuaUnitDataKey{unitPtr_intlong = unit.Pointer.ToInt64(), dataID = "AbsoluteMTCustomDefenseChangerSkillIdData"}, out var value) ? value : LuaValue.Nil;
        if (skillid != LuaValue.Nil) return false;
        return true;
    }
}