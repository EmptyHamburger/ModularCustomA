using ModularSkillScripts;
using Lethe.Patches;

namespace MTCustomScripts.Consequences;

public class ConsequenceEditBuffMax : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
        * var_1: multi-target
        * var_2: current/buffKeyword
        * var_3: stack/count/both
        * var_4: adder/vanilla/both
        * var_5: add/set
        * var_6: value
        * opt_7: info/lowmax/both
        * 
        */

        Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetModelList = modular.GetTargetModelList(circles[0]);
        if (targetModelList.Count <= 0) return;

        int modifyIntValue = modular.GetNumFromParamString(circles[5]);
        BUFF_UNIQUE_KEYWORD buffKeyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);

        BattleObjectManager instance = SingletonBehavior<BattleObjectManager>.Instance;
        BattleUnitModel modularBuffUnit = instance.GetModel(modular.modsa_buffModel.GetOwnerInstanceID());

        for (int i = 0; i < targetModelList.Count; i++)
        {
            BattleUnitModel targetModel = targetModelList[i];
            BuffModel selectedBuff = (circles[1] == "current") ? modular.modsa_buffModel : targetModel._buffDetail.FindActivatedBuff(buffKeyword, true);
            if (selectedBuff == modular.modsa_buffModel) targetModel = modularBuffUnit;

            if (selectedBuff == null) continue;

            int optionalValue = 0;
            if (circles[6] == "info" || circles[6] == "both") optionalValue += 1;
            if (circles[6] == "lowmax" || circles[6] == "both") optionalValue += 2;

            int addType = 0;
            if (circles[3] == "adder" || circles[3] == "both") addType += 1;
            if (circles[3] == "vanilla" || circles[3] == "both") addType += 2;

            string valueToEdit = circles[2];
            bool setterTypeIsAdd = circles[4] == "add";

            if (valueToEdit == "stack" || valueToEdit == "both") selectedBuff.UpdateStackValue(addType, modifyIntValue, setterTypeIsAdd, optionalValue, targetModel);
            if (valueToEdit == "count" || valueToEdit == "both") selectedBuff.UpdateCountValue(addType, modifyIntValue, setterTypeIsAdd, optionalValue, targetModel);
        }
    }
}