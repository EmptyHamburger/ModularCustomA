using ModularSkillScripts;
using BattleUI;
namespace MTCustomScripts.Consequences;

public class ConsequenceSetMainTarget : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        string mode = circles[0];

        if (mode == "Pre")
        {
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> attackers = modular.GetTargetModelList(circles[1]);
            BattleUnitModel target = modular.GetTargetModel(circles[2]);
            int SkillID = modular.GetNumFromParamString(circles[3]);
            int Count = 99;
            if(circles.Length > 4) Count = modular.GetNumFromParamString(circles[4]);

            if (attackers == null || target == null) return;

            if (target.GetSinActionList().Count < 1) return;

            if (SkillID == -1)
            {
                Singleton<BattleActionModelManager>.Instance.RemoveDuel(modular.modsa_selfAction);
                SinActionModel selfSAM = modular.modsa_selfAction._sinAction;
                SinActionModel targetSAM = target.GetSinActionList()[0];
                modular.modsa_selfAction.ChangeMainTargetSinAction(targetSAM, targetSAM._currentBattleAction, true);
                foreach(BattleActionModel bam in selfSAM.GetActionListTargetingThisSlot())
                {
                    selfSAM._actionSlot.SetActionTargetingThisSlot(bam);
                }
            }
            else
            {
                foreach(BattleUnitModel unit in attackers)
                {
                    foreach(SinActionModel sam in unit.GetSinActionList())
                    {
                        if (sam.CurrentBattleAction.Skill.GetID() == SkillID && Count > 0)
                        {
                            SinActionModel targetSam = target.GetSinActionList()[0];
                            BattleActionModel attackerAction = sam.CurrentBattleAction;                
                            BattleActionModel targetAction = targetSam.CurrentBattleAction;
                            
                            Singleton<BattleActionModelManager>.Instance.RemoveDuel(attackerAction);
                            attackerAction.ChangeMainTargetSinAction(targetSam, targetAction, true);
                            foreach(BattleActionModel bam in sam.GetActionListTargetingThisSlot())
                            {
                                sam._actionSlot.SetActionTargetingThisSlot(bam);
                            }

                            Count-=1;
                        }
                    }
                }
            }

            SingletonBehavior<BattleUIRoot>.Instance?.NewOperationController?.UpdateAllSlotForNormal();
            SingletonBehavior<BattleUIRoot>.Instance?.ShowAllCharacterTargetArrows();
        }
        else
        {
            if (modular.modsa_selfAction == null) return;
            BattleUnitModel target = modular.GetTargetModel(circles[1]);
            if (target == null) return;
            Il2CppSystem.Collections.Generic.List<SinActionModel> actionList = Singleton<SinManager>.Instance.GetActionListByUnit(target);
            if (actionList.Count < 1) return;
            modular.modsa_selfAction._targetDataDetail.GetCurrentTargetSet()._mainTarget = new TargetSinActionData(actionList[0]);
        }
    }
}