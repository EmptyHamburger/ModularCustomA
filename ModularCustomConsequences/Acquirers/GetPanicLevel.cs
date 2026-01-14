using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetPanicLevel : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: target (single)
             */
            BattleUnitModel target = modular.GetTargetModel(circles[0]);
            if (target == null) return -1;

            if (!target.IsLowMorale() && !target.IsPanic()) return 0;
            else if (target.IsLowMorale() && !target.IsPanic()) return 1;
            else if (target.IsPanic()) return 2;
            else
            {
                //Add Log here with all above bools (IsLowMorale() and IsPanic())
                return -1;
            }
        }
    }
}
