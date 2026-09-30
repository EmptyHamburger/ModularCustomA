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
            BattleUnitModel target = modular.GetTargetModel(circles[2]);
            int SkillID = modular.GetNumFromParamString(circles[3]);
            int Count = 99;
            if(circles.Length > 4) Count = modular.GetNumFromParamString(circles[4]);

            if (attackers == null || target == null) return;

            if (target.GetSinActionList().Count < 1) return;

            if (SkillID == -1)
            {
                SinActionModel selfSAM = modular.modsa_selfAction?._sinAction;
                SinActionModel targetSAM = target.GetSinActionList()[0];

                if (selfSAM != null && targetSAM != null)
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
                            SinActionModel targetSam = target.GetSinActionList()[0];
                            if (targetSam != null)
                            {
                                TryApplyDuelClash(sam, targetSam);
                                Count -= 1;
                            }
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
        if (attackerSAM == null || targetSAM == null) return;

        BattleActionModel attackerAction = attackerSAM.CurrentBattleAction;
        BattleActionModel targetAction = targetSAM.CurrentBattleAction;

        if (attackerAction == null) return;

        SinActionModel oldTargetSAM = attackerAction.GetMainTargetSinAction();
        BattleActionModel oldTargetAction = oldTargetSAM.CurrentBattleAction;


        BattleActionModelManager battleActionManager = Singleton<BattleActionModelManager>.Instance;

        if (battleActionManager != null)
        {
            battleActionManager.RemoveDuel(attackerAction);
            if (oldTargetAction != null) battleActionManager.RemoveDuel(oldTargetAction);
            if (targetAction != null) battleActionManager.RemoveDuel(targetAction);
        }



        attackerAction.ChangeMainTargetSinAction(targetSAM, targetAction, false);
        if (attackerAction.GetAttackWeight() > 1)
        try {attackerAction.ChangeAllSubTarget();} catch (Exception ex) {MTCustomScripts.Main.Logger.LogError($"Cannot refresh sub-targets for Attacker's action: {ex}");}

        if (targetAction != null)
        {
            targetAction.ChangeMainTargetSinAction(attackerSAM, attackerAction, false);
            if (targetAction.GetAttackWeight() > 1)
            try {targetAction.ChangeAllSubTarget();} catch (Exception ex) {MTCustomScripts.Main.Logger.LogError($"Cannot refresh sub-targets for Target's action: {ex}");}

            if (BattleActionModel.CanDuelBoth(attackerAction, targetAction))
            {
                if (attackerAction._model._faction == UNIT_FACTION.PLAYER)
                battleActionManager.AddDuel(attackerAction, targetAction);
                else battleActionManager.AddDuel(targetAction, attackerAction);
            }
        }

        

        if (oldTargetSAM?._actionSlot != null)
        {
            foreach (BattleActionModel bam in oldTargetSAM.GetActionListTargetingThisSlot())
            oldTargetSAM._actionSlot.SetActionTargetingThisSlot(bam);
        }

        if (targetSAM?._actionSlot != null)
        {
            foreach (BattleActionModel bam in targetSAM.GetActionListTargetingThisSlot())
            targetSAM._actionSlot.SetActionTargetingThisSlot(bam);
        }

        if (attackerSAM?._actionSlot != null)
        {
            foreach (BattleActionModel bam in attackerSAM.GetActionListTargetingThisSlot())
            attackerSAM._actionSlot.SetActionTargetingThisSlot(bam);
        }
    }
}