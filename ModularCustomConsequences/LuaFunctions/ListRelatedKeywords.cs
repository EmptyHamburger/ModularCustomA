using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using System.Collections;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListRelatedKeywords : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BuffStaticData bsd = StaticDataManager.Instance.GetBuffData(context.GetArgument(0).Read<string>());

        bool flagsub = false;

        switch(context.GetArgument(1).Read<string>())
        {
            case "sub":
                flagsub = true;
                break;
            case "category":
                flagsub = false;
                break;
        }
        IEnumerable list = flagsub ? bsd.SubKeywordList.ToArray() : bsd.CategoryKeywordList.ToArray();
        var table = new LuaTable();
        int i = 0;
        foreach (var keyword in list)
        {
            table[i + 1] = keyword.ToString();
            i++;
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}