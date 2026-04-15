using BattleUI;
using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using Utils;

namespace MTCustomScripts.Patches;

public class OnEquipDefense
{
	[HarmonyPatch(typeof(NewOperationController), nameof(NewOperationController.EquipDefense))]
	[HarmonyPostfix]
	private static void Postfix_NewOperationController_EquipDefense(ref bool equiped, SinActionModel sinAction, NewOperationController __instance)
	{
		if (equiped)
		{
			int actevent = MainClass.timingDict["OnEquipDefense"];
			SkillModel defenseSkill = sinAction.currentSinList[0].GetSkill();
			BattleActionModel actionModel = sinAction.currentSinList[0].GetBattleActionModel();
			BattleUnitModel unitModel = actionModel.Model;

			foreach (BuffModel buf in actionModel.Model.GetActivatedBuffModels())
			{
				foreach (ModularSA modba in SkillScriptInitPatch.GetAllModbaFromBuffModel(buf))
				{
					modba.modsa_buffModel = buf;
					modba.Enact(unitModel, defenseSkill, actionModel, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}

			foreach (PassiveModel passiveModel in unitModel._passiveDetail.PassiveList.CopyList())
			{
				foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(passiveModel))
				{
					modpa.modsa_passiveModel = passiveModel;
					modpa.Enact(unitModel, defenseSkill, actionModel, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}

			foreach (EgoPassiveModel egoPassiveModel in unitModel._passiveDetail.EgoPassiveList.CopyList())
			{
				foreach (ModularSA modpa in SkillScriptInitPatch.GetAllModpaFromPasmodel(egoPassiveModel, false))
				{
					modpa.modsa_passiveModel = egoPassiveModel;
					modpa.Enact(unitModel, defenseSkill, actionModel, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}
			SupportPasPatch.SupportPassiveInit(SkillScriptInitPatch.modpaDict);
			foreach (SupporterPassiveModel supportPassive in MainClass.activeSupporterPassiveList)
			{
				List<ModularSA> modpaList = SkillScriptInitPatch.GetAllModpaFromPasmodelSupport(supportPassive);
				for (int i = 0; i < modpaList.Count; i++)
				{
					supportPassive._script._owner = unitModel;
					modpaList[i].Enact(unitModel, defenseSkill, actionModel, null, actevent, BATTLE_EVENT_TIMING.ALL_TIMING);
				}
			}

			NewOperationSinActionSlot newOperationSinActionSlot = null;
			foreach (NewOperationSinActionSlot slot in __instance._sinActionSlotList)
			{
				if (slot.SinAction == sinAction)
				{
					newOperationSinActionSlot = slot;
					break;
				}
			}
			if (newOperationSinActionSlot != null)
			{
				newOperationSinActionSlot.SetData(sinAction, SingletonBehavior<BattleObjectManager>.Instance.GetView(sinAction.UnitModel), false);
				if (!Singleton<StageController>.Instance.IsAbnormalityBattle())
				{
					__instance.UpdateAllSlotForNormal(false);
				}
				NewOperationSinSlot defSlot = newOperationSinActionSlot.GetDefSlot();
				defSlot?.RevelEquipedDefEffect();
			}

			if (sinAction.IsPrevSlotEgoBySwapDefense())
			{
				SingletonBehavior<BattleUIRoot>.Instance.UpdateEvilStockByCommand();
				List<NewOperationSinActionSlot> aliveSinActionSlotList = __instance.GetAliveSinActionSlotList();
				for (int i = 0; i < aliveSinActionSlotList.Count; i++)
				{
					aliveSinActionSlotList[i].UpdatePortraitSimpleEgo();
				}
			}
		}
	}
}
