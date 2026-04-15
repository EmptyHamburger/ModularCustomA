using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceReplaceAllAffinity : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_unitModel == null) return;
		bool includeEgo = circles.Length > 1;

		if (Enum.TryParse(circles[0], out ATTRIBUTE_TYPE attribute))
		{
			SkillAbility_1021305UpgradeAttribute replaceAffinityAbility = new();
			replaceAffinityAbility.OnReplace(attribute);
			foreach (SinActionModel sinSlot in modular.modsa_unitModel.GetSinActionList())
			{
				foreach (UnitSinModel sinModel in sinSlot.currentSinList)
				{
					SkillModel skillModel = sinModel.GetSkill();
					if (!includeEgo && skillModel.IsEgoSkill()) continue;

					skillModel.AddTemporarySkillAbility(replaceAffinityAbility);
				}
			}
		}
	}
}