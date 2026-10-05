using ModularSkillScripts;
using BattleUI;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceSetMainTarget : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        string mode = circles[0];

        if (mode == "Pre")
        {
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> attackers = modular.GetTargetModelList(circles[1]);
            if (attackers == null) return;

            SinActionModel targetSAM = null;
            if (circles[2] == "-1") targetSAM = modular.modsa_oppoAction._sinAction;
            else
            {
                BattleUnitModel targetUnit = modular.GetTargetModel(circles[2]);
                if (targetUnit != null) targetSAM = targetUnit.GetSinActionList()[0];
            }
            if (targetSAM == null) return;

            int SkillID = modular.GetNumFromParamString(circles[3]);
            int Count = 99;
            if(circles.Length > 4) Count = modular.GetNumFromParamString(circles[4]);

            if (SkillID == -1)
            {
                SinActionModel selfSAM = modular.modsa_selfAction?._sinAction;
                if (selfSAM != null)
                TryApplyDuelClash(selfSAM, targetSAM);
            }
            else
            {
                foreach(BattleUnitModel unit in attackers)
                {
                    foreach(SinActionModel sam in unit.GetSinActionList())
                    {
                        if (sam.CurrentBattleAction.Skill.GetID() == SkillID && Count > 0)
                        {
                            TryApplyDuelClash(sam, targetSAM);
                            Count -= 1;
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

    public static void TryApplyDuelClash(SinActionModel attackerSAM, SinActionModel targetSAM)
    {
        BattleActionModel attackerAction = attackerSAM.CurrentBattleAction;
        if (attackerSAM == null || targetSAM == null || attackerAction == null) return;

        SinActionModel oldTargetSAM = attackerAction.GetMainTargetSinAction();
        BattleActionModel oldTargetAction = oldTargetSAM?.CurrentBattleAction;
        
        BattleActionModelManager battleActionManager = Singleton<BattleActionModelManager>.Instance;

        if (oldTargetSAM != null && oldTargetSAM != targetSAM)
        attackerAction._targetDataDetail?.ClearCurrentTargetClear(attackerAction, true);

        battleActionManager?.RemoveDuel(attackerAction);
        if (oldTargetAction != null && oldTargetAction != attackerAction)
        battleActionManager?.RemoveDuel(oldTargetAction);

        attackerAction.ChangeMainTargetSinAction(targetSAM, targetSAM.CurrentBattleAction, true);
        targetSAM.OnTargetedAsMain(attackerAction);
    }
}