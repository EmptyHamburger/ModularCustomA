using ModularSkillScripts;
using System;
using System.Collections.Generic;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerHasSkill : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            if (unit == null) return -1;
            return unit.UnitDataModel.GetSkillModel(modular.GetNumFromParamString(circles[1])) != null ? 1 : 0;
        }
    }
}