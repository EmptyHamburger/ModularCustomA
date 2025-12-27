using ModularSkillScripts;
using Lua;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeDefense : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        int defId = modular.GetNumFromParamString(circles[1]);

        foreach (BattleUnitModel targetModel in modelList)
        {
            long targetPtr = targetModel.Pointer.ToInt64();
            string key = "AbsoluteMTCustomDefenseChangerSkillIdDataCheck";
            string skillKey = "AbsoluteMTCustomDefenseChangerSkillIdData";
            var dataKey = new LuaUnitDataKey
            {
                unitPtr_intlong = targetPtr,
                dataID = key
            };
            var data = LuaUnitDataKey.LuaUnitValues.TryGetValue(dataKey, out var value) ? value : LuaValue.Nil;
            if (data == LuaValue.Nil)
            {
                MTCustomScripts.Main.Logger.LogWarning("Defense changer script not found, creating a new one");
                UnitScript_10913 newUnitScript = new UnitScript_10913();
                targetModel._unitScripts.Add(newUnitScript);

                var newDataKey = new LuaUnitDataKey
                {
                    unitPtr_intlong = targetPtr,
                    dataID = key
                };
                LuaUnitDataKey.LuaUnitValues[newDataKey] = true;
            }
            var dataSkillKey = new LuaUnitDataKey
            {
                unitPtr_intlong = targetPtr,
                dataID = skillKey
            };
            LuaUnitDataKey.LuaUnitValues[dataSkillKey] = defId;
            MTCustomScripts.Main.Logger.LogInfo($"Defense skill id set to {defId}");
        }
    }
}