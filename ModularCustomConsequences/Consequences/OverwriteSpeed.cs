using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceOverwriteSpeed : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        int newSpeed = modular.GetNumFromParamString(circles[1]);

        foreach (BattleUnitModel targetModel in modelList)
        {
            targetModel._originSpeed = newSpeed;
        }
    }
}