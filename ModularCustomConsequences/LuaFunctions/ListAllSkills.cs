using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using Lua;
using System.Threading.Tasks;
using System.Threading;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListAllSkills : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (target == null) return ValueTask.FromResult(0);
        Il2CppSystem.Collections.Generic.List<SkillModel> skills = target.GetSkillList();

        var table = new LuaTable();

        for (int i = 0; i < skills.Count; i++)
        {
            table[i + 1] = skills[i].GetID();
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}