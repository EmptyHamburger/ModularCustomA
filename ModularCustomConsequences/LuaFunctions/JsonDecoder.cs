using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using Lua;
using System.Threading;
using System.Threading.Tasks;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionJsonDecoder : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {

        var rawStr = context.GetArgument(0).Read<string>();
        buffer[0] = MTCustomScripts.Main.Decode.decode(rawStr);
        return ValueTask.FromResult(1);
    }
}