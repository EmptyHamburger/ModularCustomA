using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasMang : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(target);
        foreach(CharacterAppearanceAddOn addOn in view._curAppearance._appearanceAddOn)
        {
            if (addOn is CharacterAppearanceMangController mangAddOn)
            {
                foreach(MangSkillController mangCtrl in mangAddOn.mangControllers)
                if (mangCtrl.mangList.Count > 0) return 1;
            }
        }
        return 0;
    }
}