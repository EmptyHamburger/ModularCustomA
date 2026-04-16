using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerIsSinnerFielded : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		int sinnerID = modular.GetNumFromParamString(circles[0]);
		foreach (BattleUnitModel unitModel in SingletonBehavior<BattleObjectManager>.Instance.GetAliveList(false))
		{
			if (unitModel.GetCharacterID() == sinnerID)
			{
				return 1;
			}
		}
		return 0;
	}
}