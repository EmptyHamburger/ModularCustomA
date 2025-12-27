using ModularSkillScripts;
using Lethe.Patches;

namespace MTCustomScripts.Acquirers;

public class ConsequenceEditBuffMax : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
        * var_1: current/buffKeyword
        * var_2: stack/count/both
        * var_3: adder/vanilla/both
        * var_4: add/set
        * var_5: value
        * opt_6: info/lowmax/both
        * 
        */

        int modifyIntValue = 0;
        if (!int.TryParse(circles[4], out modifyIntValue)) return;


        BuffModel selectedBuff = null;

        if (circles[0] != "current")
        {
            BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[0]);
            if (modular.modsa_unitModel._buffDetail.HasBuff(var1Keyword) == true) selectedBuff = modular.modsa_unitModel._buffDetail.FindActivatedBuff(var1Keyword, true);
        }
        if (selectedBuff == null) selectedBuff = modular.modsa_buffModel;


        bool hasInfo = circles.Length > 4 && circles[5] != null && (circles[5] == "info" || circles[5] == "both");
        bool hasMaxLower = circles.Length > 4 && circles[5] != null && (circles[5] == "lowmax" || circles[5] == "both");


        if (circles[1] == "stack" || circles[1] == "both")
        {
            if (circles[2] == "adder" || circles[2] == "both")
            {
                if (circles[3] == "add") selectedBuff._maxStackAdder += modifyIntValue;
                if (circles[3] == "add" && hasInfo) selectedBuff._buffInfo._maxStack += modifyIntValue;

                if (circles[3] == "set") selectedBuff._maxStackAdder = modifyIntValue;
                if (circles[3] == "set" && hasInfo) selectedBuff._buffInfo._maxStack = modifyIntValue;
            }

            if (circles[2] == "vanilla" || circles[2] == "both")
            {
                if (circles[3] == "add") selectedBuff._vanilaMaxStack += modifyIntValue;
                if (circles[3] == "add" && hasInfo) selectedBuff._buffInfo._vanilaMaxStack += modifyIntValue;

                if (circles[3] == "set") selectedBuff._vanilaMaxStack = modifyIntValue;
                if (circles[3] == "set" && hasInfo) selectedBuff._buffInfo._vanilaMaxStack = modifyIntValue;
            }
        }

        ///----------------------------------------------------------------------------///
        ///----------------------------------------------------------------------------///
        ///----------------------------------------------------------------------------///

        if (circles[1] == "count" || circles[1] == "both")
        {
            if (circles[2] == "adder" || circles[2] == "both")
            {
                if (circles[3] == "add") selectedBuff._maxTurnAdder += modifyIntValue;
                if (circles[3] == "add" && hasInfo) selectedBuff._buffInfo._maxTurn += modifyIntValue;

                if (circles[3] == "set") selectedBuff._maxTurnAdder = modifyIntValue;
                if (circles[3] == "set" && hasInfo) selectedBuff._buffInfo._maxTurn = modifyIntValue;
            }

            if (circles[2] == "vanilla" || circles[2] == "both")
            {
                if (circles[3] == "add") selectedBuff._vanilaMaxTurn += modifyIntValue;
                if (circles[3] == "add" && hasInfo) selectedBuff._buffInfo._vanilaMaxTurn += modifyIntValue;

                if (circles[3] == "set") selectedBuff._vanilaMaxTurn = modifyIntValue;
                if (circles[3] == "set" && hasInfo) selectedBuff._buffInfo._vanilaMaxTurn = modifyIntValue;
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
                selectedBuff.LoseStack(model, 0, modular.battleTiming, loseValue);
            }

            if (selectedBuff.GetCurrentTurn() > selectedBuff.GetMaxTurn())
            {
                loseValue = selectedBuff.GetMaxTurn() - selectedBuff.GetCurrentTurn();
                selectedBuff.LoseTurn(model, modular.battleTiming, loseValue);
            }
        }
    }
}