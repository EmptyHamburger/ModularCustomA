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
                // MTCustomScripts.Main.Logger.LogMessage($"Set _modifiedSpeedByDuel to {newInt}");
                // modular.modsa_selfAction._sinAction.SetModifiedSpeed(newInt);
                Singleton<BattleActionModelManager>.Instance._actionList.Remove(modular.modsa_selfAction);
                if (newIdx < 0) newIdx = 0;
                if (newIdx >= Singleton<BattleActionModelManager>.Instance._actionList.Count) newIdx = Singleton<BattleActionModelManager>.Instance._actionList.Count;
                Singleton<BattleActionModelManager>.Instance._actionList.Insert(newIdx, modular.modsa_selfAction);
            }
            catch (Exception ex)
            {
                MTCustomScripts.Main.Logger.LogFatal($"Couldn't change Action's index: {ex.Message}");
                MTCustomScripts.Main.Logger.LogFatal($"STAR TRACE: {ex.StackTrace}");
            }
        }
    }
}
