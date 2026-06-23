using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using Lua;
using System.Collections.Generic;
using MTCustomScripts;
using Il2CppSystem;

namespace MTCustomScripts.LuaFunctions;

public class LuaFunctionListActionTargetThisSlot : IModularLuaFunction
{
    public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
    {
        BattleActionModel unitAction = modular.modsa_selfAction;
        if (context.GetArgument(0).Read<string>() != "Self") unitAction = modular.modsa_oppoAction;
        if (unitAction == null)
        {
            MTCustomScripts.Main.Logger.LogError($"[ListActionTargetThisSlot] {context.GetArgument(0).Read<string>()} action not found!");
            return ValueTask.FromResult(0);
        }
        LuaTable table = new LuaTable();
        int index = 1;
        foreach(BattleActionModel battleActionModel in unitAction.SinAction.GetActionListTargetingThisSlot())
        {
            LuaTable newDict = new LuaTable();

            newDict["SkillID"] = battleActionModel.GetSkillID();
            newDict["InstID"] = battleActionModel.Model.InstanceID;

            table[index] = newDict;
            index++;
        }
        buffer[0] = table;
        return ValueTask.FromResult(1);
    }
}
