using ModularSkillScripts;
using System;
using Lethe.Patches;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeTakeBuffDamage : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList.Count < 1) return;
        BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
        if (keyword.ToString() != circles[1]) keyword = BUFF_UNIQUE_KEYWORD.None;
        int resultDmg = modular.GetNumFromParamString(circles[2]);
        DAMAGE_SOURCE_TYPE source = DAMAGE_SOURCE_TYPE.NONE;
        if (circles[3] != "All") Enum.TryParse<DAMAGE_SOURCE_TYPE>(circles[3], true, out source);
        foreach(BattleUnitModel target in targetList)
        {
            if (circles[3] == "All")
            {
                foreach (DAMAGE_SOURCE_TYPE srcType in Enum.GetValues<DAMAGE_SOURCE_TYPE>())
                {
                    if (srcType != DAMAGE_SOURCE_TYPE.NONE) target.ChangeTakeDamage(null, null, resultDmg, srcType, keyword, modular.battleTiming);
                }
                continue;
            }
            target.ChangeTakeDamage(null, null, resultDmg, source, keyword, modular.battleTiming);
        }
    }
}