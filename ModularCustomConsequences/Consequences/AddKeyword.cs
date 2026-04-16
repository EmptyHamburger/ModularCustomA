using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddKeyword : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
		if (targetList.Count < 1) return;

		if (Enum.TryParse(circles[1], out UNIT_KEYWORD keyword))
		{
			foreach(BattleUnitModel unit in targetList)
			{
				if (circles.Length > 2 && !unit.AssociationList.Contains(keyword))
				{
					unit.AddAssociation(keyword);
				}
				else if (!unit.HasUnitKeyword(keyword))
				{
					unit.AddUnitKeyword(keyword);
				}
			}
		}
	}
}