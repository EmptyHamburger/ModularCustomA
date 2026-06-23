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
        // LuaTable table2 = new LuaTable();
        // int index = 1;
        // foreach(BattleActionModel battleActionModel in Singleton<BattleActionModelManager>.Instance._actionList)
        // {
        //     LuaTable newDict = new LuaTable();

        //     newDict["SkillID"] = battleActionModel.GetSkillID();
        //     newDict["InstID"] = battleActionModel.Model.InstanceID;
        //     newDict["SkillType"] = battleActionModel.Skill.skillData.

        //     // table2[index] = newDict;
        //     // index++;
        // }
        // buffer[0] = table2;
        return ValueTask.FromResult(1);
    }
}
