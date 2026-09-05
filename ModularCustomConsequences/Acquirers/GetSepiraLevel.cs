using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetSepiraLevel : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        return (int) MTCustomScripts.Main.Instance.durante_keyword;
    }
}