using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionGetBuffLocaleData : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        string buffKeyword = context.GetArgument(0).Read<string>();
        TextData_Buf textData = Singleton<TextDataSet>.Instance._bufList.GetData(buffKeyword);
        if (textData == null)
        {
            MainClass.Logg.LogError($"Buff's Text Data does not exist: {buffKeyword}");
            return ValueTask.FromResult(0);
        }
        string opt = context.GetArgument(1).Read<string>().ToLower();
        switch (opt)
        {
            case "name":
                buffer[0] = textData.name;
                return ValueTask.FromResult(1);
            case "desc":
                buffer[0] = textData.desc;
                return ValueTask.FromResult(1);
            case "summary":
                buffer[0] = textData.summary;
                return ValueTask.FromResult(1);
        }
        buffer[0] = Lua.LuaValue.Nil;
        return ValueTask.FromResult(1);
    }
}