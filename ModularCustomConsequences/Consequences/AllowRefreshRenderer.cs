using ModularSkillScripts;
using System;
using MTCustomScripts.Utils;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceAllowRefreshRenderer : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {      
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
            if (targetList.Count < 1) return;
            
            bool allowRefresh = modular.GetBoolFromParamString(circles[1]);

            foreach (BattleUnitModel unit in targetList)
            if (allowRefresh) MTUtil._excludeRendererRefreshInstanceIDList.Remove(unit._instanceID); else MTUtil._excludeRendererRefreshInstanceIDList.Add(unit._instanceID);
        }
    }
}
