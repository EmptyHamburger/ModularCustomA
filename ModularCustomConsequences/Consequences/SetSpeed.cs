using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetSpeed : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Il2CppSystem.Collections.Generic.List<BattleUnitModel> units = modular.GetTargetModelList(circles[0]);
        if (units.Count < 1) return;

        int newSpeedVal = modular.GetNumFromParamString(circles[1]);
        bool checkMinMax = false;
        if (circles.Length > 2) checkMinMax = modular.GetBoolFromParamString(circles[2]);

        foreach(BattleUnitModel unit in units)
        unit.SetSpeed(newSpeedVal, checkMinMax);
	}
}