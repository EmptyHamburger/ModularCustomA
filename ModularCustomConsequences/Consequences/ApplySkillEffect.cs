using ModularSkillScripts;
using Lethe.Patches;
using System;
using BattleUI;
using BattleUI.Operation;
using UnityEngine;
namespace MTCustomScripts.Consequences;

public class ConsequenceApplySkillEffect : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return;

        int slotIdx = modular.GetNumFromParamString(circles[1]);
        string topOrBottom = circles[2];
        OPERATION_SKILL_EFFECT_TYPE effectType;
        Enum.TryParse(circles[3], true, out effectType);
        bool isActive = modular.GetBoolFromParamString(circles[4]);
        int alphaValue = 1;
        if (circles.Length > 5) alphaValue = modular.GetNumFromParamString(circles[5]);
        // int x = 1;
        // int y = 1;
        // int z = 1;
        // if (circles.Length > 6)
        // {
        //     x = modular.GetNumFromParamString(circles[6]);
        //     y = modular.GetNumFromParamString(circles[7]);
        //     z = modular.GetNumFromParamString(circles[8]);
        // }

        foreach(BattleUnitModel unit in targetList)
        {
            NewOperationSinActionSlot nosas = SingletonBehavior<BattleUIRoot>.Instance.NewOperationController.GetSinActionSlot(unit.GetSinActionList()[slotIdx]);

            switch (topOrBottom)
            {
            case "Top":
                nosas._secondSinSlot._effectManager.SetActiveEffect_OneType(effectType, isActive, null);
                nosas._secondSinSlot._effectManager.SetEffectAlpha(effectType, alphaValue / 100f);
                // nosas._secondSinSlot._effectManager.SetEffectScale(effectType, new Vector3(x/100f, y/100f, z/100f));
                break;
            case "Bottom":
                nosas._firstSinSlot._effectManager.SetActiveEffect_OneType(effectType, isActive, null);
                nosas._firstSinSlot._effectManager.SetEffectAlpha(effectType, alphaValue / 100f);
                // nosas._firstSinSlot._effectManager.SetEffectScale(effectType, new Vector3(x/100f, y/100f, z/100f));
                break;
            default:
                nosas._firstSinSlot._effectManager.SetActiveEffect_OneType(effectType, isActive, null);
                nosas._secondSinSlot._effectManager.SetActiveEffect_OneType(effectType, isActive, null);
                nosas._firstSinSlot._effectManager.SetEffectAlpha(effectType, alphaValue / 100f);
                nosas._secondSinSlot._effectManager.SetEffectAlpha(effectType, alphaValue / 100f);
                // nosas._firstSinSlot._effectManager.SetEffectScale(effectType, new Vector3(x/100f, y/100f, z/100f));
                // nosas._secondSinSlot._effectManager.SetEffectScale(effectType, new Vector3(x/100f, y/100f, z/100f));
                break;
            }
        }
    }
}
