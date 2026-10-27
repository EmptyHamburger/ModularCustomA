using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeCustomMotion : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> units = modular.GetTargetModelList(circles[0]);
        if (units.Count < 1) return;

        foreach (BattleUnitModel unit in units)
        {
            unit.SetOverrideAppearanceID(circles[1]);
            SingletonBehavior<BattleObjectManager>.Instance.GetView(unit).ChangeAppearance(circles[1], true);
        }
    }
}