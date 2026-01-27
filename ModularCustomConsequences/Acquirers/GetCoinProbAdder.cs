using System;
using System.Collections.Generic;
using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetCoinProbAdder : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            return (int)unit.GetCoinProbAdder();
        }
    }
}