using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionGetGlobalVarMT : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        var index = context.GetArgument(0).Read<string>();
        buffer[0] = MTCustomScripts.Main.GlobalLuaValues.Instance.GetGlobalValue(index);

        return ValueTask.FromResult(1);
    }
}