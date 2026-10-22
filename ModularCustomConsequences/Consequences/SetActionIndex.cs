using ModularSkillScripts;
using System;
using MTCustomScripts;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceSetActionIndex : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            try
            {
                int newIdx = modular.GetNumFromParamString(circles[0]);
                int priorityGroup = 0;
                if (circles.Length > 1) priorityGroup = modular.GetNumFromParamString(circles[1]);

                MTCustomScripts.Main.RegisterActionPriority(priorityGroup, newIdx, modular.modsa_selfAction);
            }
            catch (Exception ex)
            {
                MTCustomScripts.Main.Logger.LogFatalError($"Couldn't change Action's index: {ex.Message}");
                MTCustomScripts.Main.Logger.LogFatalError($"STAR TRACE: {ex.StackTrace}");
            }
        }
    }
}
