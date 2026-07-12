using ModularSkillScripts;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTCustomScripts.Consequences;

public class ConsequenceModifySubTargetList : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        if (modular.modsa_selfAction == null) return;
        string mode = circles[0];
        bool exceptTargetedUnits = modular.GetBoolFromParamString(circles[1]);
        string inputParamTarget = circles[2];
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = new();
        Il2CppSystem.Collections.Generic.List<BattleUnitModel> excludeList = modular.GetTargetModelList(circles[3]);

        if (exceptTargetedUnits)
        {
            string paramTarget = inputParamTarget;
            string numText = Regex.Replace(inputParamTarget, "\\D", "");
            int requestedTargetNum = 99;

            if (numText.Length > 0)
            {
                requestedTargetNum = int.Parse(numText);
                paramTarget = inputParamTarget.Replace(numText, "");
            }

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> fullTargetList = modular.GetTargetModelList(paramTarget + "99");

            HashSet<BattleUnitModel> alreadyTargeted = [];
            Il2CppSystem.Collections.Generic.List<TargetSinActionData> currentSet = modular.modsa_selfAction._targetDataDetail.GetCurrentTargetSet()._subTargetList;
            
            if (modular.modsa_selfAction._targetDataDetail.GetMainTarget() != null) alreadyTargeted.Add(modular.modsa_selfAction._targetDataDetail.GetMainTarget());
            foreach (TargetSinActionData TSAD in currentSet)
            alreadyTargeted.Add(TSAD._targetSinAction._unitModel);

            for (int i = 0; i < fullTargetList.Count; i++)
            {
                BattleUnitModel unit = fullTargetList[i];
                if (alreadyTargeted.Contains(unit)) continue;
                targetList.Add(unit);
                if (targetList.Count >= requestedTargetNum) break;
            }
        }
        else targetList = modular.GetTargetModelList(inputParamTarget);

        for (int i = targetList.Count - 1; i > -1; i--)
        {
            if (excludeList.Contains(targetList[i])) targetList.RemoveAt(i);
        }

        if (mode == "Add")
        {
            foreach(BattleUnitModel target in targetList)
            {
                foreach(SinActionModel sinActionModel in Singleton<SinManager>.Instance.GetActionListByUnit(target))
                {
                    modular.modsa_selfAction._targetDataDetail.GetCurrentTargetSet()._subTargetList.Add(new TargetSinActionData(sinActionModel));
                }
            }
        }
        else
        {
            Il2CppSystem.Collections.Generic.List<TargetSinActionData> removeSubTargetList = new();
            
            foreach(BattleUnitModel target in targetList)
            {
                foreach(TargetSinActionData TSAD in modular.modsa_selfAction._targetDataDetail.GetCurrentTargetSet()._subTargetList)
                {
                    if (TSAD._targetSinAction._unitModel == target) removeSubTargetList.Add(TSAD);
                }
            }

            foreach(TargetSinActionData TSAD in removeSubTargetList)
            {
                modular.modsa_selfAction._targetDataDetail.GetCurrentTargetSet()._subTargetList.Remove(TSAD);
            }
        }
    }
}