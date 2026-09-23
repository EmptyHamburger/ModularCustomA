using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetCoinIndex : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_coinModel == null) return -1;
        return circles[0] switch
        {
            "Log" => modular.modsa_coinModel.GetCoinLogIndex(),
            "Real" => modular.modsa_coinModel._currentRealCoinIndex,
            "Origin" => modular.modsa_coinModel._originCoinIndex,
            _ => -1
        };
    }
}