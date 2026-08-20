using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasMang : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel target = modular.GetTargetModel(circles[0]);
        if (target == null) return -1;
        BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(target);
        if (view == null) return -1;
        int totalMang = 0;
        // foreach(CharacterAppearanceAddOn addOn in view._curAppearance._appearanceAddOn)
        // {
        //     if (addOn is CharacterAppearanceMangController mangAddOn)
        //     {
        //         Main.Logger.LogMessage("Found AppearanceMangController");
        //         foreach(MangSkillController mangCtrl in mangAddOn.mangControllers)
        //         {
        //             Main.Logger.LogMessage("Found MangSkillController");
        //             if (mangCtrl.mangList.Count > 0) totalMang += mangCtrl.mangList.Count;
        //         }
        //     }
        // }
        return totalMang;
    }
}