using System;
using System.Collections.Generic;
using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetShield : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            if (unit == null) return -1;

            string arg = (circles.Length > 1) ? circles[1] : "";

            return arg switch
            {
                "Perm" => unit._state.PermanentShield,
                "Temp" => unit._state.TempShield,
                _ => unit._state.TotalShield
            };
        }
    }
}