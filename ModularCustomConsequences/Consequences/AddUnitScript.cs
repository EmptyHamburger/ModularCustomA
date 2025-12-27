using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddUnitScript : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        var modelList = modular.GetTargetModelList(circles[0]);
        if (modelList.Count < 1) return;

        string unitscriptName = circles[1];
        unitscriptName = $"UnitScript_{unitscriptName}";
        try
        {
            UnitScriptBase newUnitScript = (UnitScriptBase)Activator.CreateInstance(typeof(UnitScriptBase).Assembly.GetType(unitscriptName));
            foreach (BattleUnitModel targetModel in modelList)
            {
                targetModel._unitScripts.Add(newUnitScript);
            }
            MTCustomScripts.Main.Logger.LogInfo($"Added '{unitscriptName}'");
        }
        catch (Exception msg)
        {
            MTCustomScripts.Main.Logger.LogError($"Can't add '{unitscriptName}' to an unit: {msg}");
        }
    }
}