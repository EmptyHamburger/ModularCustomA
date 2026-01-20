using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using Lua;
using System.Threading.Tasks;
using System.Threading;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionGetAppearanceID : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (target == null) return ValueTask.FromResult(0);

        buffer[0] = target.GetAppearanceID();
        return ValueTask.FromResult(1);
    }
}