using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceClearUnitScript : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        foreach (BattleUnitModel targetModel in modelList)
        {
            targetModel._unitScripts.Clear();
        }
    }
}