using ModularSkillScripts;
using Lethe.Patches;
using System;
using BattleUI;
using BattleUI.Operation;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasSkillEffect : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return -1;

        int slotIdx = modular.GetNumFromParamString(circles[1]);
        string topOrBottom = circles[2];
        OPERATION_SKILL_EFFECT_TYPE effectType;
        Enum.TryParse(circles[3], true, out effectType);

        foreach (BattleUnitModel unit in targetList)
        {
            NewOperationSinActionSlot nosas = SingletonBehavior<BattleUIRoot>.Instance.NewOperationController.GetSinActionSlot(unit.GetSinActionList()[slotIdx]);

            switch (topOrBottom)
            {
                case "Top":
                    return nosas._secondSinSlot._effectManager.ContainEffect(effectType) ? 1 : 0;
                case "Bottom":
                    return nosas._firstSinSlot._effectManager.ContainEffect(effectType) ? 1 : 0;
                default:
                    return -1;
            }
        }

        return -1;
    }
}
