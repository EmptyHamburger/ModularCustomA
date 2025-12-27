using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGainBuffStack : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel != null) return MTCustomScripts.Main.Instance.gainbuff_stack;
        return -1;
    }
}