using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetParryingClashPower : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            if (MTCustomScripts.Main.currentBattleLog_Parrying == null) return -1;
            return circles[0] == "Self" ? MTCustomScripts.Main.currentBattleLog_Parrying.GetAllCoinTotalResult(modular.modsa_unitModel._instanceID) : MTCustomScripts.Main.currentBattleLog_Parrying.GetAllCoinTotalResult(modular.modsa_oppoAction._model._instanceID);
        }
    }
}
