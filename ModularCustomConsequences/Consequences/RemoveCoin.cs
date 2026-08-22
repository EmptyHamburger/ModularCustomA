using System;
using System.Linq;
using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceRemoveCoin : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		SkillModel skill = (circles[0] == "Self") ? modular.modsa_skillModel : modular.modsa_oppoAction._skill;
		
		if (circles[1] == "All")
		{
			skill._coinList.Clear();
			return;
		}

		foreach (string circle in circles.Skip(1).Reverse())
		{
			int idx = modular.GetNumFromParamString(circle);
			if (idx < 0)
			{
				skill._coinList.RemoveAt(modular.modsa_coinModel.GetOriginCoinIndex());
				continue;
			}

			idx = Math.Min(idx, skill.CoinList.Count - 1);
			skill._coinList.RemoveAt(idx);
		}
	}
}