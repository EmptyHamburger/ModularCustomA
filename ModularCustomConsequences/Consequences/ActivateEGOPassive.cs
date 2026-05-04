using Il2CppSystem.Collections.Generic;
using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceActivateEGOPassive : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> modelList = modular.GetTargetModelList(circles[0]);
		int egoID = modular.GetNumFromParamString(circles[1]);
		foreach (BattleUnitModel targetModel in modelList)
		{
			targetModel.ActivateEgoPassive(egoID);
		}
	}
}