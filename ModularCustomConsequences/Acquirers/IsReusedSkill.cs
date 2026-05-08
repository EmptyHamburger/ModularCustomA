using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerIsReusedSkill : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (circles[0] != "Self") action = modular.modsa_oppoAction;

		if (action.IsReusedAction())
		{
			return 1;
		}
		return 0;
	}
}
