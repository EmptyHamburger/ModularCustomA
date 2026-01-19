using HarmonyLib;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using BattleUI.Operation;
using BepInEx.Unity.IL2CPP.UnityEngine;

internal class EquipDefenseOperation
{
    [HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
    [HarmonyPrefix]
    [HarmonyBefore("ModularSkillScripts")]
    private static bool Prefix_NewOperationController_EquipDefense(bool equiped, SinActionModel sinAction)
    {
        BattleUnitModel unit = sinAction.actionSlot.Owner;

        LuaUnitDataKey.LuaUnitValues[new LuaUnitDataKey{unitPtr_intlong = unit.Pointer.ToInt64(), dataID = "AbsoluteMTCustomDefenseChangerDebounceCheck"}] = false;

        if (!unit.IsActionable()) return true;
        int actevent = MainClass.timingDict["SpecialAction"];
        bool returnval = true;
        foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList) {
            if (!passiveModel.CheckActiveCondition()) continue;
            long passiveModel_intlong = passiveModel.Pointer.ToInt64();
            if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;
                    
            foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong]) {
                if (!Input.GetKeyInt(modpa.SpecialKey)) continue;
                MTCustomScripts.Main.Instance.special_slotindex = sinAction.GetSlotIndex();
                // MainClass.Logg.LogInfo("FoundS modpassive - SPECIAL: " + modpa.passiveID);
                // MainClass.Logg.LogInfo("Triggered Key: " + modpa.SpecialKey.ToString());
                returnval = false;
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
                returnval = false;
                // MainClass.Logg.LogInfo("FoundS modpassive - SPECIAL: " + modpa.passiveID);
                // MainClass.Logg.LogInfo("Triggered Key: " + modpa.SpecialKey.ToString());
                // modpa.modsa_passiveModel = passiveModel;
                // modpa.Enact(passiveModel.Owner, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
            }
        }
        if (returnval) return true;
        return false;
    }
}