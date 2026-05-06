using ModularSkillScripts;
using System;
using static SinManager;

namespace MTCustomScripts.Acquirers;

public class AcquirerIsUnitPartOfReson : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleUnitModel unit = modular.GetTargetModel(circles[0]);
		if (unit == null) return -1;
		ResonanceManager resonanceManager = Singleton<SinManager>.Instance._resManager;
		if(!Enum.TryParse(circles[1], out ATTRIBUTE_TYPE attribute))
		{
			int highestRes = 0;
			foreach (ATTRIBUTE_TYPE tempAttribute in Enum.GetValues(typeof(ATTRIBUTE_TYPE)))
			{
				int current = resonanceManager.GetAttributeResonance(modular.modsa_unitModel.Faction, tempAttribute);
				if (current > highestRes)
				{
					attribute = tempAttribute;
					highestRes = current;
				}
			}
		}

		if (resonanceManager.GetMaxAttributeResonanceOfSpecificUnit(attribute, unit) > 0)
		{
			return 1;
		}
		return 0;
	}
}