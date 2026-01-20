using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerIsActionable : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        return target.IsActionable() ? 1 : 0;
    }
}