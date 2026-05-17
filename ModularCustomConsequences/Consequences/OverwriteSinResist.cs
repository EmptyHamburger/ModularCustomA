using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceOverwriteSinResist : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        int rest = modular.GetNumFromParamString(circles[2]);
		bool Meth = circles.Length >= 4;

        ATTRIBUTE_TYPE attrType;
        Enum.TryParse(circles[1], true, out attrType);
        float resist = rest / 100f;

        // MainClass.Logg.LogInfo("sinType: " + attrType + " ; resVal: " + resist);

        foreach (BattleUnitModel targetModel in modelList)
        {
            var currentRes = targetModel._resistDetail._attributeResist;
            foreach (var res in currentRes)
            {
                if (res.Type == attrType)
                {
                    if (Meth)
                    {
                        res.value += resist;
                    }
                    else res.value = resist;
                    break;
                }
            }
        }
    }
}