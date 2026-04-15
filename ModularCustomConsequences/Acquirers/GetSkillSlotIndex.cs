using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetSkillSlotIndex : IModularAcquirer
{
	public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
	{
		BattleActionModel action = modular.modsa_selfAction;
		if (action == null) return -1;

		return action.SinAction.actionSlot.GetSlotIndex();
	}
}