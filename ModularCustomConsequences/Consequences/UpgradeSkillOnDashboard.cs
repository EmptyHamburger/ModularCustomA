using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceUpgradeSkillOnDashboard : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unitModel = modular.modsa_unitModel;
		if (unitModel == null) return;

		int skillID = modular.GetNumFromParamString(circles[0]);
		int upgradedID = modular.GetNumFromParamString(circles[1]);
		foreach (SinActionModel sinSlot in unitModel.GetSinActionList())
		{
			for (int i = 0; i < sinSlot.currentSinList.Count; i++)
			{
				SkillModel skillModel = sinSlot.currentSinList[i].GetSkill();
				if (skillModel.GetID() == skillID || (skillModel.IsDefense() && skillModel.GetID() == sinSlot.GetReplacedSinByDefenseSkill().GetSkill().GetID()))
				{
					sinSlot.currentSinList[i] = new(upgradedID, unitModel, sinSlot, true);
					goto End;
				}
			}
		}
	End: return;
	}
}
