using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequencePlayMotion : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel fromUnit = modular.GetTargetModel(circles[0]);
        if (fromUnit == null || fromUnit.IsDead()) return;
        string motion_detail = circles[1];
        var appearance = SingletonBehavior<BattleObjectManager>.Instance.GetViewByUnitID(fromUnit._originID).Appearance;
        var view = SingletonBehavior<BattleObjectManager>.Instance.GetViewByUnitID(fromUnit._originID).Appearance.GetView();
        List<BattleUnitView> ViewUnits = new List<BattleUnitView>();
        ViewUnits.Add(view);
        ViewFocusInfo viewInfo = new ViewFocusInfo(view, ViewUnits);
        SingletonBehavior<BattleCamManager>.Instance.AddFocusingTarget(viewInfo);
        MOTION_DETAIL motion = (MOTION_DETAIL)Enum.Parse(typeof(MOTION_DETAIL), motion_detail);
        int index = -1;
        if (circles.Length > 2 && !string.IsNullOrEmpty(circles[2]))
        {
            int.TryParse(circles[2], out index);
        }

        appearance.ChangeMotion(motion, true, index, false, null, false);
    }
}