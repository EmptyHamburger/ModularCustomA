using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using Lua;
using System.Threading.Tasks;
using System.Threading;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListSkillKeywordList : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (target == null) return ValueTask.FromResult(0);

        Il2CppSystem.Collections.Generic.List<SKILL_KEYWORD> skillkeywords = Singleton<StaticDataManager>.Instance.PersonalityStaticDataList.GetData(target.GetUnitID()).SkillKeywordList;;

        var table = new LuaTable();

        for (int i = 0; i < skillkeywords.Count; i++)
        {
            table[i + 1] = skillkeywords[i].ToString();
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}