using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

internal class ConsequenceClearAllTempSkillAbilities : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		if (modular.modsa_unitModel == null) return;

		foreach (SinActionModel sinslot in modular.modsa_unitModel.GetSinActionList())
		{
			foreach (UnitSinModel sinModel in sinslot.currentSinList)
			{
				sinModel.GetSkill().ClearTemporarySkillAbility();
			}
		}
	}
}