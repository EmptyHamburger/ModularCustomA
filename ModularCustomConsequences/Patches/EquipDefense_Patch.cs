using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using BattleUI.Operation;
using BepInEx.Unity.IL2CPP.UnityEngine;

internal class EquipDefenseOperation
{
    [HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
	[HarmonyPrefix]
	public static void Prefix_NewOperationController_EquipDefense(SinActionModel sinAction)
    {
        LuaUnitDataKey.LuaUnitValues[new LuaUnitDataKey{unitPtr_intlong = sinAction.actionSlot.Owner.Pointer.ToInt64(), dataID = "AbsoluteMTCustomDefenseChangerDebounceCheck"}] = false;
    }

    [HarmonyPatch(typeof(UniquePatches), nameof(UniquePatches.RunSpecialAction))]
    [HarmonyPostfix]
    public static void Postfix_UniquePatches_RunSpecialAction(SinActionModel sinAction, ref bool __result)
    {
        BattleUnitModel unit = sinAction.actionSlot.Owner;
        if (!unit.IsActionable()) __result = false;
        
        foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
                    
            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
                if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
                MTCustomScripts.Main.Instance.special_slotindex = sinAction.GetSlotIndex();
                // MainClass.Logg.LogInfo("FoundS modpassive - SPECIAL: " + modpa.passiveID);
                // MainClass.Logg.LogInfo("Triggered Key: " + modpa.SpecialKey.ToString());
                __result = true;
                // modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(passiveModel.Owner, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
            }
        }
        foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
        {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
            {
                if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
                MTCustomScripts.Main.Instance.special_slotindex = sinAction.GetSlotIndex();
                __result = true;
                // MainClass.Logg.LogInfo("FoundS modpassive - SPECIAL: " + modpa.passiveID);
                // MainClass.Logg.LogInfo("Triggered Key: " + modpa.SpecialKey.ToString());
                // modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(passiveModel.Owner, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
            }
        }
    }
}
