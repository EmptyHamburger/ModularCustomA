using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceSetModularValue : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            int idx = modular.GetNumFromParamString(circles[0]);
            int newVal = modular.GetNumFromParamString(circles[1]);

            modular.valueList[idx] = newVal;
        }
    }
}
