using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddSkillKeyword : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return;

        System.Collections.Generic.List<SkillModel> skillList = modular.GetMultipleSkillModel(targetList, circles[1]);
        if (skillList.Count < 1) return;

        if (!Enum.TryParse(circles[1], out SKILL_KEYWORD keyword)) return;

        bool addKeyword = circles.Length > 3 && circles[3] == "add";

        foreach (SkillModel skill in skillList)
        {
            if (!skill.HasKeyword(keyword))
            {
                if (addKeyword) skill._skillKeywords.Add(keyword);
                else skill._skillKeywords.Remove(keyword);
            }
        }        
    }
}