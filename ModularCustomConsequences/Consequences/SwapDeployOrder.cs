using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceSwapDeploymentOrder : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel unit = modular.GetTargetModel(circles[0]);
        BattleUnitModel target = modular.GetTargetModel(circles[1]);
        if (unit == null || target == null) return;
        int unitOrder = unit._participateOrder;
        unit._participateOrder = target._participateOrder;
        target._participateOrder = unitOrder;
    }
}