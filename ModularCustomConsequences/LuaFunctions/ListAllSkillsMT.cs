using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using Lua;
using System.Threading.Tasks;
using System.Threading;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListAllSkillsMT : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
        if (target == null) return ValueTask.FromResult(0);

        var table = new LuaTable();
        int i = 1;
        foreach (UnitAttribute unitAttribute in target._unitDataModel._unitAttributeList)
        {
            LuaTable newDict = new LuaTable();

            newDict["SkillID"] = unitAttribute.skillId;
            newDict["Amount"] = unitAttribute.number;

            table[i] = newDict;
            i++;
        }

        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}