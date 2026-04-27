
using ModularSkillScripts;
using System.Linq;

namespace MTCustomScripts.Acquirers;

public class AcquirerHasSkillOnDashboard : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unitModel = modular.GetTargetModel(circles[0]);
		if (unitModel == null) return -1;

		int[] skillIDList = new int[circles.Length - 1];
		for (int i = 1; i < circles.Length; i++) skillIDList[i - 1] = modular.GetNumFromParamString(circles[i]);
		foreach (SinActionModel sinSlot in unitModel.GetSinActionList())
		{
			for (int i = 0; i < sinSlot.currentSinList.Count; i++)
			{
				SkillModel skillModel = sinSlot.currentSinList[i].GetSkill();
				if (skillIDList.Contains(skillModel.GetID()) || (skillModel.IsDefense() && skillIDList.Contains(sinSlot.GetReplacedSinByDefenseSkill().GetSkill().GetID())))
				{
					return 1;
				}
			}
		}
		return 0;
	}
}
