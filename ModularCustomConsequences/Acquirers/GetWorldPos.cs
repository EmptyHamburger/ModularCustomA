using ModularSkillScripts;
using System;
using DG.Tweening;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetWorldPosition : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleUnitModel unit = modular.GetTargetModel(circles[0]);
        string type = circles[1];
        if (unit == null) return 404040404;

        BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(unit);
        if (view == null) return 404040404;

        switch(type)
        {
            case "x":
                return (int) Math.Floor(view.WorldPosition.x * 100);
            case "y":
                return (int) Math.Floor(view.WorldPosition.y * 100);
            case "z":
                return (int) Math.Floor(view.WorldPosition.z * 100);
        }

        return 404040404;
    }
}