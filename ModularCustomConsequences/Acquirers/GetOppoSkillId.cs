using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetOppoSkillId : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_oppoAction != null) return modular.modsa_oppoAction.Skill.GetID();
		return 0;
	}
}