using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerLoseBuffActiveRound : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel != null) return MTCustomScripts.Main.Instance.losebuff_activeRound;
        return -1;
    }
}