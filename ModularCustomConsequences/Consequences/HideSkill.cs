using ModularSkillScripts;
using System;
using MTCustomScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceHideSkill : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel battleUnitModel = modular.GetTargetModel(circles[0]);
        if (battleUnitModel == null) return;

        SingletonBehavior<BattleObjectManager>.Instance.GetView(battleUnitModel)._battleSkillViewers.Remove(circles[1]);
    }
}
