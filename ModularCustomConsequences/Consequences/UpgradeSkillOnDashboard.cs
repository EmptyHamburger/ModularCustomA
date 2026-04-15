using ModularSkillScripts;
using Spine.Unity;

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
				UnitSinModel newSinModel = new(upgradedID, unitModel, sinSlot, true);
				SkillModel skillModel = sinSlot.currentSinList[i].GetSkill();
				if (skillModel.GetID() == skillID)
				{
					sinSlot.currentSinList[i] = newSinModel;
					goto End;
				}
				else if(skillModel.IsDefense() && skillModel.GetID() == sinSlot.GetReplacedSinByDefenseSkill().GetSkill().GetID())
				{
					sinSlot._replacedSinByDefenseSkill = newSinModel;
					goto End;
				}
			}
		}
	End: return;
	}
}
