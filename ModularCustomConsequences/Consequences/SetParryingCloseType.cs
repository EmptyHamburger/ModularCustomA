using DG.Tweening;
using ModularSkillScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCustomScripts.Consequences
{
    internal class ConsequenceSetParryingCloseType : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            if (modular.modsa_skillModel == null) return;

            if (circles[0] == "FAR") modular.modsa_skillModel._skillData.parryingCloseType = PARRYING_CLOSE_TYPE.FAR;
            else modular.modsa_skillModel._skillData.parryingCloseType = PARRYING_CLOSE_TYPE.NEAR;
        }
    }
}
