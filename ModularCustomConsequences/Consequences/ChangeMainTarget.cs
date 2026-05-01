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
                sam.CurrentBattleAction.ChangeMainTargetSinAction(target.GetSinActionList()[0], target.GetSinActionList()[0].CurrentBattleAction, true);
                sam.CurrentBattleAction.RecheckTargetList();
            }
        }
        SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.UpdateAllSlotForNormal();
        SingletonBehavior<BattleUIRoot>.Instance?.ShowAllCharacterTargetArrows();
	}
}