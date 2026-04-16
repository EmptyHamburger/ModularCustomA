using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceReplaceAllAffinity : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
		if (targetList.Count < 1) return;
		bool includeEgo = circles.Length > 2;

		if (Enum.TryParse(circles[1], out ATTRIBUTE_TYPE attribute))
		{
			foreach(BattleUnitModel unit in targetList)
			{
				foreach (SinActionModel sinSlot in modular.modsa_unitModel.GetSinActionList())
				{
					foreach (UnitSinModel sinModel in sinSlot.currentSinList)
					{
						SkillModel skillModel = sinModel.GetSkill();
						if (!includeEgo && skillModel.IsEgoSkill()) continue;

						SkillAbility_1021305UpgradeAttribute replaceAffinityAbility = new();
						replaceAffinityAbility.OnReplace(attribute);

						skillModel.AddTemporarySkillAbility(replaceAffinityAbility);
					}
				}
			}
		}
	}
}