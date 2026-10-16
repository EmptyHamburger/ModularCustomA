using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetSpeedLimit : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Il2CppSystem.Collections.Generic.List<BattleUnitModel> units = modular.GetTargetModelList(circles[0]);
        if (units.Count < 1) return;

        string limitType = circles[1];
        int newLimit = modular.GetNumFromParamString(circles[2]);

        foreach(BattleUnitModel unit in units)
        if (limitType == "Min") unit._unitDataModel._minSpeed = newLimit; else unit._unitDataModel._maxSpeed = newLimit;
	}
}