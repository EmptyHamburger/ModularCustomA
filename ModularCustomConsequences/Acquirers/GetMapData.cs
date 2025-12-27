using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetMapData : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
        * var_1: current/mapName
        * var_2: size/active/id
        */

        BattleMapPreset selectedMap = null;
        if (circles[0] == "current") selectedMap = BattleMapManager.Instance._mapObject._currentMap;
        else BattleMapManager.Instance._mapObject._mapDict.TryGetValue(circles[0], out selectedMap);

        if (selectedMap == null) return -1;

        switch (circles[1])
        {
            default: return -1;
            case "size": return (int)selectedMap.GetMapSize();
            case "active": return selectedMap.IsActive() ? 1 : 0;
            case "id":
                MTCustomScripts.Main.TestStuffStorage.stringDict[string.Format("{0}{1}", modular.ptr_intlong, "MapId")] = selectedMap.GetMapID();
                return 1;
        }
    }
}