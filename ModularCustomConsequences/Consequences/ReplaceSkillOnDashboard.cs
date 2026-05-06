using ModularSkillScripts;
using System.Collections.Generic;

namespace MTCustomScripts.Consequences;

public class ConsequenceReplaceSkillOnDashboard : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unitModel = modular.GetTargetModel(circles[0]);
		if (unitModel == null) return;

		int sinActionIndex = modular.GetNumFromParamString(circles[1]);
		int unitSinModelIndex = modular.GetNumFromParamString(circles[2]);
		int skillID = modular.GetNumFromParamString(circles[3]);

		if (sinActionIndex < 0 || unitSinModelIndex < 0)
		{
			List<(int, int)> skillSlotList = [];
			foreach (SinActionModel sinSlot in unitModel.GetSinActionList())
			{
				for (int i = 0; i < sinSlot.currentSinList.Count; i++)
				{
					SkillModel skillModel = sinSlot.currentSinList[i].GetSkill();
					if (skillModel.IsDefense() || skillModel.IsEgoSkill()) continue;
					skillSlotList.Add(new(sinSlot.GetSlotIndex(), i));
				}
			}
			skillSlotList.Sort(new SkillComparer());
			sinActionIndex = skillSlotList[0].Item1;
			unitSinModelIndex = skillSlotList[0].Item2;
		}

		SinActionModel targetAction = unitModel.GetSinActionList()[sinActionIndex];
		targetAction.currentSinList[unitSinModelIndex] = new(skillID, unitModel, targetAction, true);
	}

	private class SkillComparer : IComparer<(int, int)>
	{
		public int Compare((int, int) x, (int, int) y)
		{
			if (x.Item1 < y.Item1)
			{
				return -1;
			}
			else if (x.Item1 == y.Item1)
			{
				if (x.Item2 < y.Item2)
				{
					return -1;
				}
			}
			return 0;
		}
	}
}
