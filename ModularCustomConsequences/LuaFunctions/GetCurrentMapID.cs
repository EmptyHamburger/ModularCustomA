using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace MTCustomScripts.LuaFunctions;

public class GetCurrentMapID : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        buffer[0] = BattleMapManager.Instance._mapObject._currentMap.GetMapID();

        return ValueTask.FromResult(1);
    }
}