using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetTimingID : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
        return modular.activationTiming;
	}
}
