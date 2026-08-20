using ModularSkillScripts;
using Il2CppSystem;
using System.Collections.Generic;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetExpectedSkillPower : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleActionModel action = modular.modsa_selfAction;
            BattleActionModel oppoAction = modular.modsa_oppoAction;
            if (circles[0] != "Self")
            {
                action = modular.modsa_oppoAction;
                oppoAction = modular.modsa_selfAction;
            }

            if (action == null)
            {
                Main.Logger.LogError("GetExpectedSkillPower: action is null! Returning -1");
                return -1;
            }

            // COIN_ROLL_TYPE rollType = Enum.TryParse<COIN_ROLL_TYPE>(circles[1], out var parsedType) ? parsedType : COIN_ROLL_TYPE.NONE;
            // if (rollType == COIN_ROLL_TYPE.NONE)
            // {
            //     Main.Logger.LogError("COIN_ROLL_TYPE is NONE! Returning 0");
            //     return 0;
            // }

            // bool isMax = (circles[2] == "Max");
            bool isMax = (circles[1] == "Max");

            return isMax ? action.GetExpectedMaxCoinValue(oppoAction?._sinAction, out _) : action.GetExpectedMinCoinValue(oppoAction?._sinAction, out _);
            // // int expectedSkillPowerAdder = action.GetExpectedSkillPower(rollType, (action == modular.modsa_selfAction) ? modular.modsa_oppoAction._sinAction : modular.modsa_selfAction._sinAction, (action == modular.modsa_selfAction) ? modular.modsa_oppoAction : modular.modsa_selfAction);
            // int skillPowerDefault = action._skill.GetSkillDefaultPower();
            // int skillPowerAdder = action._skill.GetSkillPowerAdder(action, rollType, action._skill._coinList);
            // int finalPowerAfterModifiers = skillPowerDefault + skillPowerAdder;

            // foreach(CoinModel coinModel in action._skill._coinList)
            // {
            //     int coinScale = coinModel.GetScale();
            //     int coinScaleAdder = coinModel.GetCoinScaleAdder(action, oppoAction._model);
            //     int expectedCoinScaleAdder = coinModel.GetExpectedCoinScaleAdder(action, rollType, oppoAction._model);
            //     OPERATOR_TYPE opType = coinModel.GetOperatorType();
            //     bool isHead = isMax ? (opType != OPERATOR_TYPE.SUB && ((opType == OPERATOR_TYPE.MUL) ? (coinScale > 1) : (coinScale > 0))) : (opType == OPERATOR_TYPE.SUB || ((opType == OPERATOR_TYPE.MUL) ? (coinScale < 1) : (coinScale < 0)));
            //     finalPowerAfterModifiers = SkillModelManager.GetExpectedSkillPowerByOneCoin(finalPowerAfterModifiers, coinScale + coinScaleAdder + expectedCoinScaleAdder, opType, isHead);
            // }

            // if (oppoAction != null && oppoAction.IsAttack())
            // {
            //     finalPowerAfterModifiers += action.GetParryingAdder(oppoAction);
            //     // finalPowerAfterModifiers += action.GetExpectedParryingResultAdder();
            // }

            // if (finalPowerAfterModifiers < 0)
            // {
            //     Main.Logger.LogMessage($"Expected Max {circles[1]} Skill Power after modifiers = {finalPowerAfterModifiers}! Returning 0");
            //     return 0;
            // }

            // return finalPowerAfterModifiers;
        }
    }
}
