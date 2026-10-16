using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetModularValue : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        int idx = modular.GetNumFromParamString(circles[0]);
        return modular.valueList[idx];
    }
}