using ModularSkillScripts;
using System;
using System.Collections.Generic;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetSkillData : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: single-unit
             * var_2: single-skill
             * var-3: data-type
             * opt-4: coin-list
             */

            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            SkillModel skill = modular.GetSingleSkillModel(unit, circles[1]);
            Il2CppSystem.Collections.Generic.List<CoinModel> selectedCoins = (circles.Length >= 4 && circles[3] != null) ? modular.GetCoinModelList(skill, circles[3], null) : null;
            COIN_ROLL_TYPE rollType = (modular.battleTiming == BATTLE_EVENT_TIMING.ON_START_DUEL || modular.battleTiming == BATTLE_EVENT_TIMING.BEFORE_ROLL_COIN_PARRYING) ? COIN_ROLL_TYPE.PARRYING : COIN_ROLL_TYPE.ACTION;

            BattleActionModel selfAction = unit._actionList.ToSystem().Find(x => x._skill == skill);
            BattleActionModel oppoAction = null;

            BattleActionModelManager battleActionModelManager = Singleton<BattleActionModelManager>.Instance;
            if (selfAction != null) oppoAction = battleActionModelManager.GetDuelOpponentAction(selfAction);

            int result = -1;

            switch (circles[2])
            {
                case "ID":
                    result = skill.GetID();
                    break;
                case "CoinScale":
                    result = skill.GetCoinScale();
                    break;
                case "CoinScaleAdder":
                    result = skill.GetCoinScaleAdder(oppoAction, selectedCoins[0]);
                    break;
                case "Final":
                    result = skill.GetSkillPowerAdder(oppoAction, rollType, selectedCoins);
                    break;
                case "Clash":
                    result = skill.GetSkillPowerResultAdder(oppoAction, modular.battleTiming, selectedCoins[0]);
                    break;
                case "Weight":
                    result = skill.GetAttackWeight(oppoAction);
                    break;
                case "OgWeight":
                    result = skill.GetOriginAttackWeight();
                    break;
                case "EvadePower":
                    result = skill.GetEvadeSkillPowerAdder(selfAction, oppoAction);
                    break;
                case "Default":
                    result = skill.GetSkillDefaultPower();
                    break;
                case "Motion":
                    result = (int)skill.GetSkillMotion();
                    break;
                case "Level":
                    result = unit.GetSkillLevel(skill, null, out _, out _);
                    break;
                case "SkillAtkLevel":
                    result = skill.GetSkillLevelCorrection();
                    break;
                case "DefType":
                    result = (int)skill.GetDefenseType();
                    break;
                case "AtkType":
                    result = (int)skill.GetAttackType();
                    break;
                case "CanDuel":
                    result = (skill.CanDuel(selfAction, oppoAction)) ? 1 : 0;
                    break;
                case "Rank":
                    result = skill.GetSkillTier();
                    break;
                case "Fixed":
                    result = (skill.CanBeChangedTarget(selfAction)) ? 1 : 0;
                    break;
                case "Attribute":
                    result = (int)skill.GetAttributeType();
                    break;
                case "EgoType":
                    result = (int)skill.GetSkillEgoType();
                    break;
            }

            return result;
        }
    }
}
