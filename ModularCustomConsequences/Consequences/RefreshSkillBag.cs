using System.Collections.Generic;
using Il2CppInterop.Runtime;
using ModularSkillScripts;
using Server;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceRefreshSkillBag : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: multi-target
             */
            try
            {

                Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
                if (unitList.Count <= 0) return;
                foreach (BattleUnitModel unit in unitList)
                {
                    unit._actionSlotDetail._skillDictionary.Clear(); 
                    unit._actionSlotDetail.SetSkillDictionary();   
                }
            }
            catch (System.Exception ex) { Main.Logger.LogError("ConsequenceRefreshSkillBag error: " + ex); }
        }
    }
}
