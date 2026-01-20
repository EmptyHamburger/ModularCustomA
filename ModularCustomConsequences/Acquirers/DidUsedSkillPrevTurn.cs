using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerDidUsedSkillPrevTurn : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        int skillId = modular.GetNumFromParamString(circles[1]);
        return target.DidActionPrevTurn(skillId) ? 1 : 0;
    }
}