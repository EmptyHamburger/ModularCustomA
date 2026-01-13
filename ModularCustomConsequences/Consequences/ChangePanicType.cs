using ModularSkillScripts;
using Lethe.Patches;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangePanicType : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
         * var_1: target (multi)
         * var_2: panic
         * var_3: default/cached/buff
         * opt_4: buff/current
         */

        Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
        if (unitList == null || unitList.Count == 0) return;
        if (!Il2CppSystem.Enum.TryParse<PANIC_TYPE>(circles[1], true, out PANIC_TYPE panic)) return;

        BUFF_UNIQUE_KEYWORD buffKeyword = BUFF_UNIQUE_KEYWORD.None;
        bool SelectBuff = circles.Length >= 4 && (circles[2] != null && circles[2].Equals("Buff", System.StringComparison.OrdinalIgnoreCase)) && (circles[3] != null && (circles[3].Equals("Current", System.StringComparison.OrdinalIgnoreCase) || Il2CppSystem.Enum.TryParse<BUFF_UNIQUE_KEYWORD>(circles[3], true, out buffKeyword)));

        foreach (BattleUnitModel unit in unitList)
        {
            if (!SelectBuff)
            {
                if (circles[2].Equals("Default", System.StringComparison.OrdinalIgnoreCase)) unit._defaultPanicType = panic;
                else if (circles[2].Equals("Cached", System.StringComparison.OrdinalIgnoreCase))
                {
                    unit._cachedPanicType = panic;
                    if (Il2CppSystem.Enum.TryParse<BUFF_UNIQUE_KEYWORD>(Singleton<StaticDataManager>.Instance.GetBuffData(circles[2]).id, true, out var panicUniqueType))
                        unit._cachedPanicTypeBuff = panicUniqueType;
                }
                else return;

                continue;
            }

            if (SelectBuff)
            {
                BuffModel selectedBuff = null;

                if (!circles[3].Equals("Current", System.StringComparison.OrdinalIgnoreCase))
                {
                    BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
                    if (unit._buffDetail.HasBuff(var1Keyword) == true) selectedBuff = unit._buffDetail.FindActivatedBuff(var1Keyword, true);
                }
                if (selectedBuff == null) selectedBuff = modular.modsa_buffModel;
                if (selectedBuff == null) continue;

                Main.TestStuffStorage.overrideBuffPanicDict[selectedBuff] = panic;
            }
        }
    }
}

