using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;
using BattleUI;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts.Patches;
using MTCustomScripts;

namespace MTCustomScripts.Patches;
internal class SinActionModelPatches
{
    [HarmonyPatch(typeof(SinActionModel), nameof(SinActionModel.DeSelectSin))]
    [HarmonyPostfix]
    public static void Postfix_SinActionModel_DeSelectSin(SinActionModel __instance)
    {
        // try
        // {
        //     SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.GetSinActionSlot(sam)?._firstSinSlot?._effectManager?._skillEffectList?.Clear();
        //     SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.GetSinActionSlot(sam)?._secondSinSlot?._effectManager?._skillEffectList?.Clear();
        // }
        // catch { }
        List<BattleUnitModel> unitList = SingletonBehavior<BattleObjectManager>.Instance.GetModelList();
        int actevent = MainClass.timingDict["OnDeSelectSin"];
        MTCustomScripts.Main.lastSinSlotIndex = __instance.GetSlotIndex();
        foreach (BattleUnitModel unit in unitList)
        {
            foreach (PassiveModel passiveModel in unit._passiveDetail.PassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (PassiveModel passiveModel in unit._passiveDetail.EgoPassiveList)
            {
                if (!passiveModel.CheckActiveCondition()) continue;
                long passiveModel_intlong = passiveModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modpaDict.ContainsKey(passiveModel_intlong)) continue;

                foreach (ModularSA modpa in SkillScriptInitPatch.modpaDict[passiveModel_intlong])
                {
                    modpa.modsa_passiveModel = passiveModel;
                    modpa.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }

            foreach (BuffModel buffModel in unit._buffDetail.GetActivatedBuffModelAll())
            {
                long buffmodel_intlong = buffModel.Pointer.ToInt64();
                if (!SkillScriptInitPatch.modbaDict.ContainsKey(buffmodel_intlong)) continue;

                foreach (ModularSA modba in SkillScriptInitPatch.modbaDict[buffmodel_intlong])
                {
                    modba.modsa_buffModel = buffModel;
                    modba.Enact(unit, null, null, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
                }
            }
        }
    }
}