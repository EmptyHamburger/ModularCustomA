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

        var targetModelList = modular.GetTargetModelList(circles[0]);
        bool hasTarget = (targetModelList.Count > 0) ? true : false;

        int modifyIntValue = 0;
        if (!int.TryParse(circles[5], out modifyIntValue)) return;

        BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);

        for (int index = 0; index < (hasTarget ? targetModelList.Count : 1); index++)
        {
            BattleUnitModel targetModel = hasTarget ? targetModelList[index] : null;
            BuffModel selectedBuff = null;

            if (circles[1] != "current" && targetModel != null)
            {
                if (targetModel._buffDetail.HasBuff(var1Keyword) == true) selectedBuff = targetModel._buffDetail.FindActivatedBuff(var1Keyword, true);
            }

            if (selectedBuff == null) selectedBuff = modular.modsa_buffModel;
            
            bool hasInfo = circles.Length > 4 && circles[6] != null && (circles[6] == "info" || circles[6] == "both");
            bool hasMaxLower = circles.Length > 4 && circles[6] != null && (circles[6] == "lowmax" || circles[6] == "both");


            if (circles[2] == "stack" || circles[2] == "both")
            {
                if (circles[3] == "adder" || circles[3] == "both")
                {
                    if (circles[4] == "add") selectedBuff._maxStackAdder += modifyIntValue;
                    if (circles[4] == "add" && hasInfo) selectedBuff._buffInfo._maxStack += modifyIntValue;

                    if (circles[4] == "set") selectedBuff._maxStackAdder = modifyIntValue;
                    if (circles[4] == "set" && hasInfo) selectedBuff._buffInfo._maxStack = modifyIntValue;
                }

                if (circles[3] == "vanilla" || circles[3] == "both")
                {
                    if (circles[4] == "add") selectedBuff._vanilaMaxStack += modifyIntValue;
                    if (circles[4] == "add" && hasInfo) selectedBuff._buffInfo._vanilaMaxStack += modifyIntValue;

                    if (circles[4] == "set") selectedBuff._vanilaMaxStack = modifyIntValue;
                    if (circles[4] == "set" && hasInfo) selectedBuff._buffInfo._vanilaMaxStack = modifyIntValue;
                }
            }

            ///----------------------------------------------------------------------------///
            ///----------------------------------------------------------------------------///
            ///----------------------------------------------------------------------------///

            if (circles[2] == "count" || circles[2] == "both")
            {
                if (circles[3] == "adder" || circles[3] == "both")
                {
                    if (circles[4] == "add") selectedBuff._maxTurnAdder += modifyIntValue;
                    if (circles[4] == "add" && hasInfo) selectedBuff._buffInfo._maxTurn += modifyIntValue;

                    if (circles[4] == "set") selectedBuff._maxTurnAdder = modifyIntValue;
                    if (circles[4] == "set" && hasInfo) selectedBuff._buffInfo._maxTurn = modifyIntValue;
                }

                if (circles[3] == "vanilla" || circles[3] == "both")
                {
                    if (circles[4] == "add") selectedBuff._vanilaMaxTurn += modifyIntValue;
                    if (circles[4] == "add" && hasInfo) selectedBuff._buffInfo._vanilaMaxTurn += modifyIntValue;

                    if (circles[4] == "set") selectedBuff._vanilaMaxTurn = modifyIntValue;
                    if (circles[4] == "set" && hasInfo) selectedBuff._buffInfo._vanilaMaxTurn = modifyIntValue;
                }
            }

            ///----------------------------------------------------------------------------///
            ///----------------------------------------------------------------------------///
            ///----------------------------------------------------------------------------///
            
            if (hasMaxLower)
            {
                BattleObjectManager instance = SingletonBehavior<BattleObjectManager>.Instance;
                BattleUnitModel model = instance.GetModel(selectedBuff.GetOwnerInstanceID());
                int loseValue = 0;

                if (selectedBuff.GetCurrentStack() > selectedBuff.GetMaxStack())
                {
                    loseValue = selectedBuff.GetMaxStack() - selectedBuff.GetCurrentStack();
                    selectedBuff.LoseStack(model, 0, modular.battleTiming, out loseValue);
                }

                if (selectedBuff.GetCurrentTurn() > selectedBuff.GetMaxTurn())
                {
                    loseValue = selectedBuff.GetMaxTurn() - selectedBuff.GetCurrentTurn();
                    selectedBuff.LoseTurn(model, modular.battleTiming, out loseValue);
                }
            }
        }
    }
}