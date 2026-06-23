using ModularSkillScripts;
using Lethe.Patches;
using System;
using BattleUI;
using BattleUI.Operation;

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

        foreach(BattleUnitModel unit in targetList)
        {
                NewOperationSinActionSlot nosas = SingletonBehavior<BattleUIRoot>.Instance.NewOperationController.GetSinActionSlot(unit.GetSinActionList()[slotIdx]);

                switch (topOrBottom)
                {
                case "Top":
                    nosas._secondSinSlot._effectManager.SetActiveEffect_OneType(effectType, true, null);
                    break;
                case "Bottom":
                    nosas._firstSinSlot._effectManager.SetActiveEffect_OneType(effectType, true, null);
                    break;
                default:
                    nosas._firstSinSlot._effectManager.SetActiveEffect_OneType(effectType, true, null);
                    nosas._secondSinSlot._effectManager.SetActiveEffect_OneType(effectType, true, null);
                    break;
                }
        }
    }
}
