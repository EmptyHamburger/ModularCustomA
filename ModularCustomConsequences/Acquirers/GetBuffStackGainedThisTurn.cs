using ModularSkillScripts;
using Lethe.Patches;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetBuffStackGainedThisTurn : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
        if (keyword.ToString() != circles[1]) keyword = BUFF_UNIQUE_KEYWORD.None;
        return target.GetBuffStackGainedThisTurn(keyword);
    }
}