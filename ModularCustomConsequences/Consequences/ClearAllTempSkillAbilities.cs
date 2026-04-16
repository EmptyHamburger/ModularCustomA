using ModularSkillScripts;

namespace MTCustomScripts.Consequences;

internal class ConsequenceClearAllTempSkillAbilities : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
		if (targetList.Count < 1) return;

		foreach (BattleUnitModel unit in targetList)
		{
			foreach (SinActionModel sinslot in unit.GetSinActionList())
			{
				foreach (UnitSinModel sinModel in sinslot.currentSinList)
				{
					sinModel.GetSkill().ClearTemporarySkillAbility();
				}
			}
		}
	}
}