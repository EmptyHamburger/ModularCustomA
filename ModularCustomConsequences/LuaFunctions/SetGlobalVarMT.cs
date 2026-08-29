using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionSetGlobalVarMT : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        var index = context.GetArgument(0).Read<string>();
        var val = context.GetArgument(1);
        MTCustomScripts.Main.GlobalLuaValues.Instance.SetGlobalValue(index, val);

        return ValueTask.FromResult(0);
    }
}