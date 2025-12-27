using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerSpecialActionSlotIndex : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        return MTCustomScripts.Main.Instance.special_slotindex;
    }
}