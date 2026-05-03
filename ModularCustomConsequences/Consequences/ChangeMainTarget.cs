using ModularSkillScripts;
using System;
using BattleUI;

namespace MTCustomScripts.Consequences;

public class ConsequenceChangeMainTarget: IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel attacker = modular.GetTargetModel(circles[0]);
        BattleUnitModel target = modular.GetTargetModel(circles[1]);
        int SkillID = modular.GetNumFromParamString(circles[2]);
        int Count = 99;
        if(circles.Length > 3) Count = modular.GetNumFromParamString(circles[3]);

        if (attacker == null || target == null) return;

        if (target.GetSinActionList().Count < 1) return;

        foreach(SinActionModel sam in attacker.GetSinActionList())
        {
            if (sam.CurrentBattleAction.Skill.GetID() == SkillID && Count > 0)
            {
                SinActionModel targetSam = target.GetSinActionList()[0];
                BattleActionModel attackerAction = sam.CurrentBattleAction;                
                BattleActionModel targetAction = targetSam.CurrentBattleAction;
                
                attackerAction.ChangeMainTargetSinAction(targetSam, targetAction, true);

                Count-=1;

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