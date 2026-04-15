using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddKeyword : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel targetModel = modular.GetTargetModel(circles[0]);
		if (targetModel == null) return;

		if (Enum.TryParse(circles[1], out UNIT_KEYWORD keyword))
		{
			if (circles.Length > 2 && !targetModel.AssociationList.Contains(keyword))
			{
				targetModel.AddAssociation(keyword);
			}
			else if (!targetModel.HasUnitKeyword(keyword))
			{
				targetModel.AddUnitKeyword(keyword);
			}
		}
	}
}