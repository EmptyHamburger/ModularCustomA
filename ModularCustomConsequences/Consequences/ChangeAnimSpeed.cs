using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeAnimSpeed : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel fromUnit = modular.GetTargetModel(circles[0]);
        if (fromUnit == null || fromUnit.IsDead()) return;
        var appearance = SingletonBehavior<BattleObjectManager>.Instance.GetViewByUnitID(fromUnit._originID).Appearance;

        float speed = 1f;
        if (float.TryParse(circles[1], out speed)) { speed /= 100f; };
        appearance.SetAnimSpeed(speed);
    }
}
