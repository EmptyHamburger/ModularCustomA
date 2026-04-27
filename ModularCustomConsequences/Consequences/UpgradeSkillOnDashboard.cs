using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceUpgradeSkillOnDashboard : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unitModel = modular.GetTargetModel(circles[0]);
		if (unitModel == null) return;

		int skillID = modular.GetNumFromParamString(circles[1]);
		int upgradedID = modular.GetNumFromParamString(circles[2]);
		foreach (SinActionModel sinSlot in unitModel.GetSinActionList())
		{
			for (int i = 0; i < sinSlot.currentSinList.Count; i++)
			{
				SkillModel skillModel = sinSlot.currentSinList[i].GetSkill();
				if (skillModel.GetID() == skillID)
				{
					sinSlot.currentSinList[i] = new(upgradedID, unitModel, sinSlot, true);
					goto End;
				}
				else if(skillModel.IsDefense() && sinSlot.GetReplacedSinByDefenseSkill().GetSkill().GetID() == skillID)
				{
					sinSlot.ChangeReplacedSinByDefenseSkillAtoB(skillID, upgradedID);
					goto End;
				}
			}
		}
	End: return;
	}
}
