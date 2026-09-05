using ModularSkillScripts;
using System;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasSkillKeyword : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return -1;

        System.Collections.Generic.List<SkillModel> skillList = modular.GetMultipleSkillModel(targetList, circles[1]);
        if (skillList.Count < 1) return -1;

        if (!Enum.TryParse(circles[1], out SKILL_KEYWORD keyword)) return -1;
        int foundSkill = 0;

        foreach (SkillModel skill in skillList)
        {
            if (skill.HasKeyword(keyword)) foundSkill++;
        }

        return foundSkill;
    }
}