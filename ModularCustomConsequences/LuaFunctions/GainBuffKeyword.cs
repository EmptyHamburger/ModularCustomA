using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionGainBuffKeyword : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        buffer[0] = MTCustomScripts.Main.Instance.gainbuff_keyword.ToString();

        return ValueTask.FromResult(1);
    }
}