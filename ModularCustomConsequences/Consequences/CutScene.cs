using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceCutScene : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var model = modular.GetTargetModelList(circles[0]);
        if (model == null) return;

        BattleCutSceneBase newCutScene = new BattleCutSceneBase();
        
    }
}