using ModularSkillScripts;
using System;
using MTCustomScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceRemoveAllActions : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        // Singleton<StageController>.Instance.EndBattlePhaseForcely(true);
        Singleton<BattleActionModelManager>.Instance._actionList.Clear();
    }
}