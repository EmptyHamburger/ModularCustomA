using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using System.Collections.Generic;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListPassiveIDs : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (target == null) return ValueTask.FromResult(0);
        var tempDict = new Dictionary<string, List<int>>
        {
            ["egopassive"] = new List<int>(),
            ["passive"] = new List<int>()
        };

        foreach (EgoPassiveModel egoPassive in target._passiveDetail._egoPassiveList)
        {
            tempDict["egopassive"].Add(egoPassive.GetID());
        }
        foreach (PassiveModel passive in target._passiveDetail._passivelist)
        {
            tempDict["passive"].Add(passive.GetID());
        }

        LuaTable table = new LuaTable();

        foreach (var elem in tempDict)
        {
            LuaTable idList = new LuaTable();

            for (int i = 0; i < elem.Value.Count; i++)
            {
                idList[i + 1] = elem.Value[i];
            }

            table[elem.Key] = idList;
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}