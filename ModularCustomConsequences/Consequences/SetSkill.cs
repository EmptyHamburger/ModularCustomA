using DG.Tweening;
using ModularSkillScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCustomScripts.Consequences
{
    internal class ConsequenceSetSkill : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            if (unit == null)
            {
                MTCustomScripts.Main.Logger.LogError("[SetSkill] Unit not found!");
                return;
            }
            int SkillID = modular.GetNumFromParamString(circles[1]);
            int newSkillID = modular.GetNumFromParamString(circles[2]);

            foreach (SinActionModel sam in unit.GetSinActionList())
            {
                if (sam._currentBattleAction._skill.GetID() == SkillID)
                {
                    UnitSinModel newUSM = new UnitSinModel(newSkillID, unit, sam);
                    sam.SelectSin(newUSM);
                }
            }
        }
    }
}
