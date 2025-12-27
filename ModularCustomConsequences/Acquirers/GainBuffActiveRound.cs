using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGainBuffActiveRound : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel != null) return MTCustomScripts.Main.Instance.gainbuff_activeRound;
        return -1;
    }
}