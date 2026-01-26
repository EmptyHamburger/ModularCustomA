using ModularSkillScripts;
using Lethe.Patches;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeHp : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return;
        int newHp = modular.GetNumFromParamString(circles[1]);
        Enum.TryParse<DAMAGE_SOURCE_TYPE>(circles[2], true, out DAMAGE_SOURCE_TYPE source);
        BattleUnitModel attackerOrNull = circles.Length > 5 ? modular.GetTargetModel(circles[5]) : null;
        BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[3]);
        if (keyword.ToString() != circles[3]) keyword = BUFF_UNIQUE_KEYWORD.None;
        bool deactivePassedBreakSection = modular.GetBoolFromParamString(circles[4]);
        foreach(BattleUnitModel target in targetList)
        {
            target.ChangeHp(newHp, source, modular.battleTiming, attackerOrNull, null, null, keyword, deactivePassedBreakSection);
        }
    }
}