using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetSpeedAdder : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel target = modular.GetTargetModel(circles[0]);
		if (target == null) return -1;

		if (circles.Length >= 2)
		{
			if (circles[1] == "min") return target.GetMinSpeedAdder();
			else if (circles[1] == "max") return target.GetMaxSpeedAdder();
		}

		return target.GetSpeedAdder();
	}
}
