using HarmonyLib;
using Il2CppSystem.Net;
using MTCustomScripts;
using ModularSkillScripts;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MTCustomScripts.Patches;

internal class CharacterState_Patches
{
    [HarmonyPatch(typeof(CharacterState), nameof(CharacterState.SetModel))]
    [HarmonyPostfix]
    public static void Postfix_CharacterState_SetModel(BattleUnitModel model, CharacterState __instance)
    {
        MTCustomScripts.Main.Logger.LogMessage($"SetModel | Unit Name: {model.GetName()}");
        MTCustomScripts.Main.intPtrCharacterState_BattleUnitModel_Dict.Add(__instance.Pointer, model);
    }

    [HarmonyPatch(typeof(CharacterState), nameof(CharacterState.AddHitDamageInfoThisRound))]
    [HarmonyPostfix]
    public static void Postfix_CharacterState_AddHitDamageInfoThisRound(BattleActionModel attackerAction, CoinModel coin, int damage, CharacterState __instance)
    {
        MTCustomScripts.Main.Logger.LogMessage($"AddHitDamageInfoThisRound | Unit Name: {GetName(__instance)} | Attacker Name: {attackerAction._model.GetName()} | Damage: {damage}");
    }

    [HarmonyPatch(typeof(CharacterState), nameof(CharacterState.AddHitRealHpThisRound))]
    [HarmonyPostfix]
    public static void Postfix_CharacterState_AddHitRealHpThisRound(int damage, CharacterState __instance)
    {
        MTCustomScripts.Main.Logger.LogMessage($"AddHitRealHpThisRound | Unit Name: {GetName(__instance)} | Damage: {damage}");
    }
    
    [HarmonyPatch(typeof(CharacterState), nameof(CharacterState.AddHitThisRound))]
    [HarmonyPostfix]
    public static void Postfix_CharacterState_AddHitThisRound(int damage, DAMAGE_SOURCE_TYPE dmgSrcType, ATTRIBUTE_TYPE attributeType, ATK_BEHAVIOUR atkType, BUFF_UNIQUE_KEYWORD buffKeyword, int attackerInstanceID, CharacterState __instance)
    {
        MTCustomScripts.Main.Logger.LogMessage($"AddHitThisRound | Unit Name: {GetName(__instance)} | Damage: {damage} | DMG Source: {dmgSrcType} | Sin Type: {attributeType} | Atk Type: {atkType} | Buff: {buffKeyword} | Attacker Inst: {attackerInstanceID}");
    }

    public static string GetName(CharacterState state)
    {
        return (MTCustomScripts.Main.intPtrCharacterState_BattleUnitModel_Dict.TryGetValue(state.Pointer, out BattleUnitModel unit)) ? unit.GetName() : "NOT_FOUND";
    }
}