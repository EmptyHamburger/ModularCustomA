using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceAddSkill : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: multi-target
             * var-2: skillId
             * var-3: level
             * var-4: uptie
             * var-5: amt
             */

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList.Count <= 0) return;

            int skillId = modular.GetNumFromParamString(circles[1]);
            if (skillId <= 0) return;

            foreach (BattleUnitModel unit in unitList)
            {
                if (unit.UnitDataModel.SkillList.ToSystem().Find(x => x.GetID() == skillId) != null) return;

                SkillStaticDataList skillList = Singleton<StaticDataManager>.Instance._skillList;
                SkillStaticData data = skillList.GetData(skillId);
                UnitAttribute skillAttribute = new UnitAttribute();
                skillAttribute.number = (circles[4] != null) ? modular.GetNumFromParamString(circles[4]) : 0;
                skillAttribute.skillId = skillId;
                int level = (circles[2] != null) ? modular.GetNumFromParamString(circles[2]) : unit.UnitDataModel.Level;
                int sync = (circles[3] != null) ? modular.GetNumFromParamString(circles[3]) : unit.UnitDataModel.SyncLevel;
                SkillModel skillModel = new SkillModel(data, level, sync);


                unit.UnitDataModel._unitAttributeList.Add(skillAttribute);
                unit.UnitDataModel._skillList.Add(skillModel);
                SingletonBehavior<BattleObjectManager>.Instance.GetView(unit).AddSkill(skillModel);
            }
        }
    }
}
