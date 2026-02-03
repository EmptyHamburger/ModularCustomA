using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetFinalPower : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            return modular.modsa_skillModel.GetSkillPowerResultAdder(modular.modsa_selfAction, modular.battleTiming, modular?.modsa_coinModel);
        }
    }
}
