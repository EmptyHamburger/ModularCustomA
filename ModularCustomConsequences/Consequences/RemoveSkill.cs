using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceRemoveSkill : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: multi-target
             * var-2: skillId
             */

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList.Count <= 0) return;

            int skillId = modular.GetNumFromParamString(circles[1]);
            if (skillId <= 0) return;

            foreach (BattleUnitModel unit in unitList)
            {
                if (unit.UnitDataModel.HasSkill(skillId)) unit.UnitDataModel._skillList.Remove(unit.UnitDataModel.GetSkillModel(skillId));
                UnitAttribute skillAttribute = unit.UnitDataModel._unitAttributeList.ToSystem().Find(x => x.SkillId == skillId);
                if (skillAttribute != null) unit.UnitDataModel._unitAttributeList.Remove(skillAttribute);

                BattleUnitView unitView = SingletonBehavior<BattleObjectManager>.Instance.GetView(unit);
                if (unitView != null && unitView._battleSkillViewers.ContainsKey(skillId.ToStringSmallGC())) unitView._battleSkillViewers.Remove(skillId.ToStringSmallGC());
            }
        }
    }
}
