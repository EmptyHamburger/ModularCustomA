using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerLoseBuffTurn : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_passiveModel != null) return MTCustomScripts.Main.Instance.losebuff_turn;
        return -1;
    }
}