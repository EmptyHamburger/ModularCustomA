using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangePanicLevel : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
         * var_1: Multi-Target
         * var_2: Panic-Level
         * opt_3: Anything
         */

        Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
        if (unitList == null || unitList.Count <= 0) return;
        if (!int.TryParse(circles[1], out int panicLevel)) return;
        if (panicLevel == 0) return;

        foreach (BattleUnitModel unit in unitList)
        {
            if (panicLevel == 0)
            {
                if ((!unit.CanRecoverLowMoraleState() && circles[2].Equals("Forcefully", System.StringComparison.OrdinalIgnoreCase))
                    || unit.CanRecoverLowMoraleState()) unit.RecoverLowMoraleState();

                if ((!unit.CanRecoverPanicState() && circles[2].Equals("Forcefully", System.StringComparison.OrdinalIgnoreCase))
                    || unit.CanRecoverPanicState()) unit.RecoverPanicState();

                unit._erosionData._isErodeThisTurn = false;
                Singleton<SinManager>.Instance.SetOffErodeSinActions(unit);
                continue;
            }
            else if (panicLevel == 1) unit.OnLowMorale(modular.battleTiming);
            else if (panicLevel == 2) unit.OnPanic(modular.battleTiming);
            else if (panicLevel == 3 && !unit._erosionData.HasOnlyDefaultEGO())
            {
                BattleEgoModel chosenEgo = null;
                if (circles.Length >= 3 && circles[2] != null)
                {
                    if (circles[2].Equals("Latest", System.StringComparison.OrdinalIgnoreCase)) chosenEgo = unit._erosionData.GetLatestUsedNonDefaultOrRandomEgoModel();
                    if (circles[2].Equals("Random", System.StringComparison.OrdinalIgnoreCase)) chosenEgo = unit._erosionData.GetRandomNonDefaultEgoModelWithWeight();
                    else if (Il2CppSystem.Enum.TryParse<EGO_TYPE>(circles[2], true, out EGO_TYPE egoDanger) && egoDanger != EGO_TYPE.ZAYIN) unit._erosionData.GetEgoModelByLevel(egoDanger, out chosenEgo);
                }

                if (chosenEgo == null) return;
                unit._erosionData._isErodeThisTurn = true;
                Singleton<SinManager>.Instance.SetOnErodeSinActions(unit, chosenEgo);
                panicLevel = 2;
            }

            unit.OnPanicOrLowMorale((PANIC_LEVEL)panicLevel, modular.battleTiming);
        }
    }
}