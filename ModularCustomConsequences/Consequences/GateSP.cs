using ModularSkillScripts;
using System;
using MTCustomScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceGateSP : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        bool isMin = circles[1] == "Min";
        int gateValue = modular.GetNumFromParamString(circles[2]);
        foreach (BattleUnitModel unit in targetList)
        {
            if (!MTCustomScripts.Main.gateSPDict.TryGetValue(unit.Pointer, out (int Min, int Max) gateData)) gateData = (-100, 100);
            MTCustomScripts.Main.gateSPDict[unit.Pointer] = isMin ? (gateValue, gateData.Max) : (gateData.Min, gateValue);
        }        
    }
}