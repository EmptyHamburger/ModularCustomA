using System.Collections.Generic;

namespace MTCustomScripts.Utils;

internal static class MTUtil
{
    public static List<int> _excludeRendererRefreshInstanceIDList = new();
    public static void UpdateState(bool updatePassive = true, bool updateViewAfterChoice = true, bool updateViewState = true, bool updateRenderer = true)
    {
        BattleObjectManager objManager = SingletonBehavior<BattleObjectManager>.Instance;
        if (updatePassive) objManager.UpdatePassiveState();
        if (updateViewAfterChoice) objManager.OnRoundStart_View_AfterChoice();
        if (updateViewState) objManager.UpdateViewState(false, false);

        if (!updateRenderer) return;
        foreach (BattleUnitView unitView in objManager.GetAliveViewList())
        {
            if (!_excludeRendererRefreshInstanceIDList.Contains(unitView._unitModel._instanceID)) unitView.RefreshAppearanceRenderer(true);
        }
    }
}