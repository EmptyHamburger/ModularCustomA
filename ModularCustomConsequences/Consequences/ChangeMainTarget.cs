using ModularSkillScripts;
using System;
using BattleUI;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeMainTarget: IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel attacker = modular.GetTargetModel(circles[0]);
        int SkillID = modular.GetNumFromParamString(circles[1]);
        BattleUnitModel target = modular.GetTargetModel(circles[2]);

        if (attacker == null || target == null) return;

        if (target.GetSinActionList().Count < 1) return;

        foreach(SinActionModel sam in attacker.GetSinActionList())
        {
            if (sam.CurrentBattleAction.Skill.GetID() == SkillID)
            {
                SinActionModel targetSam = target.GetSinActionList()[0];
                BattleActionModel attackerAction = sam.CurrentBattleAction;                
                BattleActionModel targetAction = targetSam.CurrentBattleAction;
                
                attackerAction.ChangeMainTargetSinAction(targetSam, targetAction, true);

                // var bamManager = Singleton<BattleActionModelManager>.Instance;
                // if (bamManager != null && targetAction != null)
                // {
                //     bamManager.RemoveDuel(attackerAction);
                //     bamManager.RemoveDuel(targetAction);

                //     if (BattleActionModel.CanDuelBoth(attackerAction, targetAction))
                //     bamManager.AddDuel(attackerAction, targetAction);
                // }
            }
        }
        SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.UpdateAllSlotForNormal();
        SingletonBehavior<BattleUIRoot>.Instance?.ShowAllCharacterTargetArrows();
	}
}