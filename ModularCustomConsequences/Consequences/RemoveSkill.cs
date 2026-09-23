using ModularSkillScripts;
using Server;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceRemoveSkill : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: multi-target
             * var-2: multi-skill
             */

            try
            {

                Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
                if (unitList.Count <= 0) return;

                Main mainClass = Main.Instance;

                System.Collections.Generic.List<int> skillIdList = new System.Collections.Generic.List<int>();
                foreach (SkillModel currentSkill in modular.GetMultipleSkillModel(unitList, circles[1])) skillIdList.Add(currentSkill.GetID());
                if (skillIdList.Count <= 0) return;

                foreach (BattleUnitModel unit in unitList)
                {
                    for (int i = 0; i < skillIdList.Count; i++)
                    {
                        int SkillId = skillIdList[i];

                        if (unit.UnitDataModel.ClassInfo.GetSkillIds().Contains(SkillId)) mainClass.storedRemoveSkillHash.Add(SkillId);

                        if (unit.UnitDataModel.HasSkill(SkillId)) unit.UnitDataModel._skillList.Remove(unit.UnitDataModel.GetSkillModel(SkillId));
                        UnitAttribute skillAttribute = unit.UnitDataModel._unitAttributeList.ToSystem().Find(x => x.SkillId == SkillId);
                        if (skillAttribute != null) unit.UnitDataModel._unitAttributeList.Remove(skillAttribute);

                        BattleUnitView unitView = SingletonBehavior<BattleObjectManager>.Instance.GetView(unit);
                        if (unitView != null && unitView._battleSkillViewers.ContainsKey(SkillId.ToStringSmallGC())) unitView._battleSkillViewers.Remove(SkillId.ToStringSmallGC());
                    }
                }
            }
            catch (System.Exception ex) { Main.Logger.LogError("ConsequenceRemoveSkill error: " + ex); }
        }
    }
}
