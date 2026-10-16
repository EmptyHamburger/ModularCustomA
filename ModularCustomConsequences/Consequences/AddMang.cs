using ModularSkillScripts;
using System;
using Il2CppSystem.Collections.Generic;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddMang : IModularConsequence
{
	public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
	{
		List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        int mangCount = modular.GetNumFromParamString(circles[1]);
        if (mangCount < 1) return;
        
        foreach(BattleUnitModel unit in targetList)
        {
            BattleUnitView view = SingletonBehavior<BattleObjectManager>.Instance.GetView(unit);
            if (view == null) continue;

            Main.Logger.LogMessage($"View Found with name: {unit.GetName()}");

            int controllerNeededCount = (mangCount + 4) / 5;
            CharacterAppearanceMangController AddOn = null;
            
            foreach(CharacterAppearanceAddOn appAddOn in view._curAppearance._appearanceAddOn)
            {
                if (appAddOn is CharacterAppearanceMangController mangAddOn)
                {
                    AddOn = mangAddOn;
                    break;
                }
            }

            if (AddOn == null)
            {
                AddOn = new CharacterAppearanceMangController();
                view._curAppearance._appearanceAddOn.Add(AddOn);
                // AddOn.Initialize(view._curAppearance);
                // AddOn.Init_Spine(view._curAppearance, false);
            }

            Main.Logger.LogMessage($"Found CharacterAppearanceMangController");

            if (AddOn.mangControllers == null)
            AddOn.mangControllers = new List<MangSkillController>();

            Main.Logger.LogMessage($".mangControllers found");

            for(int i = 0; i < controllerNeededCount - AddOn.mangControllers.Count; i++)
            {
                MangSkillController newMangSkillController = new MangSkillController();
                AddOn.mangControllers.Add(newMangSkillController);
                newMangSkillController.Awake();
            }

            Main.Logger.LogMessage($"Added required {controllerNeededCount} MangSkillController");

            foreach(MangSkillController mangController in AddOn.mangControllers)
            {
                if (mangCount < 1) break;
                int mangOnlyForThisOne = Math.Min(5, mangCount);
                MTCustomScripts.Main.intPtrMangAddOn_ActiveMangCountOnSkillStart[mangController.Pointer] = mangOnlyForThisOne;
                mangCount -= mangOnlyForThisOne;
            }

            Main.Logger.LogMessage($"Stored IntPtr and active Mang count for Harmony Patches");

            AddOn.Initialize(view._curAppearance);
            AddOn.Init_Spine(view._curAppearance, false);
        }
	}
}