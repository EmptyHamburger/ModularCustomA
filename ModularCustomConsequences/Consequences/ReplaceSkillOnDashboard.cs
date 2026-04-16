using ModularSkillScripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTCustomScripts.Consequences;

public class ConsequenceReplaceSkillOnDashboard : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unitModel = modular.GetTargetModel(circles[0]);
		if (unitModel == null) return;

		int sinActionIndex = modular.GetNumFromParamString(circles[1]);
		int unitSinModelIndex = modular.GetNumFromParamString(circles[2]);
		int[] skillIDList = new int[circles.Length - 3];
		for (int i = 3; i < circles.Length; i++) skillIDList[i - 3] = modular.GetNumFromParamString(circles[i]);

		if (sinActionIndex < 0 || unitSinModelIndex < 0)
		{
			List<(int, int, int)> skillSlotList = [];
			foreach (SinActionModel sinSlot in unitModel.GetSinActionList())
			{
				for (int i = 0; i < sinSlot.currentSinList.Count; i++)
				{
					SkillModel skillModel = sinSlot.currentSinList[i].GetSkill();
					if (skillIDList.Contains(skillModel.GetID()))
					{
						goto SkipEverything;
					}

					if (skillModel.IsDefense() && skillIDList.Contains(sinSlot.GetReplacedSinByDefenseSkill().GetSkill().GetID()))
					{
						goto SkipEverything;
					}

					if (skillModel.IsEgoSkill()) continue;

					skillSlotList.Add(new(sinSlot.GetSlotIndex(), i, skillModel.GetSkillTier()));
				}
			}
			skillSlotList.Sort(new SkillComparer());
			sinActionIndex = skillSlotList[0].Item1;
			unitSinModelIndex = skillSlotList[0].Item2;
		}

		SinActionModel targetAction = unitModel.GetSinActionList()[sinActionIndex];
		targetAction.currentSinList[unitSinModelIndex] = new(skillIDList[0], unitModel, targetAction, true);
	SkipEverything: return;
	}

	private class SkillComparer : IComparer<(int, int, int)>
	{
		public int Compare((int, int, int) x, (int, int, int) y)
		{
			if (x.Item3 > y.Item3)
			{
				return 1;
			}
			else if (x.Item3 == y.Item3)
			{
				if (x.Item1 < y.Item1)
				{
					return -1;
				}
				return 0;
			}
			return -1;
		}
	}
}
