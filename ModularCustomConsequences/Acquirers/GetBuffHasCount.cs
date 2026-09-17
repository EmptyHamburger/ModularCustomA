using System;
using System.Collections.Generic;
using Lethe.Patches;
using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetBuffHasCount : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            if (unit == null) return -1;

            string buffName = circles[1];
            BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(buffName);

            BuffInfo buffInfo = unit.GetBuffInfo(var1Keyword, 0);
            if (buffInfo == null || buffInfo.GetType() != typeof(BuffInfo))  return -1;

            var isCountableBuff = buffInfo.IsCountableBuff();

            if (isCountableBuff)
            {
                return 1;
            }
            else
            {
                return 0; 
            }

        }
    }
}