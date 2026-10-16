using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetSpeed : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Il2CppSystem.Collections.Generic.List<BattleUnitModel> units = modular.GetTargetModelList(circles[0]);
        if (units.Count < 1) return;

        int newSpeedVal = modular.GetNumFromParamString(circles[1]);
        int priority = modular.GetNumFromParamString(circles[2]);
        bool checkMinMax = false;
        if (circles.Length > 3) checkMinMax = modular.GetBoolFromParamString(circles[3]);

        foreach(BattleUnitModel unit in units)
        unit.SetSpeed(newSpeedVal * 1000 + priority, checkMinMax);
	}
}