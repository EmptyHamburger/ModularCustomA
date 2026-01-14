using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetFinalPower : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            COIN_ROLL_TYPE rollType = modular.modsa_oppoAction != null ? COIN_ROLL_TYPE.PARRYING : COIN_ROLL_TYPE.ACTION;
            return modular.modsa_skillModel.GetSkillPowerAdder(modular.modsa_oppoAction, rollType, modular.modsa_skillModel.CoinList);
        }
    }
}
