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
        string mode = circles[2];
        Enum.TryParse<DAMAGE_SOURCE_TYPE>(circles[3], true, out DAMAGE_SOURCE_TYPE source);
        BattleUnitModel attackerOrNull = circles.Length > 6 ? modular.GetTargetModel(circles[6]) : null;
        BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[4]);
        if (keyword.ToString() != circles[4]) keyword = BUFF_UNIQUE_KEYWORD.None;
        bool deactivePassedBreakSection = modular.GetBoolFromParamString(circles[5]);
        foreach(BattleUnitModel target in targetList)
        {
            target.ChangeHp((mode == "%") ? (int)Math.Floor(target.MaxHp * newHp / 100f) : newHp, source, modular.battleTiming, attackerOrNull, null, null, keyword, deactivePassedBreakSection);
        }
    }
}