using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using System.Collections.Generic;
using MTCustomScripts;
using Il2CppSystem;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListBattleActions : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        buffer[0] = MTCustomScripts.Main.Instance.actionListDatas;
        return ValueTask.FromResult(1);
    }
}