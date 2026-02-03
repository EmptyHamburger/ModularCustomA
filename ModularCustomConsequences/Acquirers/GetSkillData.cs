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
             * opt-4: coin-list/var
             */

            BattleUnitModel unit = modular.GetTargetModel(circles[0]);
            SkillModel skill = modular.GetSingleSkillModel(unit, circles[1]);
            Il2CppSystem.Collections.Generic.List<CoinModel> selectedCoins = (circles.Length >= 4 && circles[3] != null) ? modular.GetCoinModelList(skill, circles[3], null) : null;
            bool isNotCoin = (selectedCoins.Count == 0) ? true : false;
            COIN_ROLL_TYPE rollType = (modular.battleTiming == BATTLE_EVENT_TIMING.ON_START_DUEL || modular.battleTiming == BATTLE_EVENT_TIMING.BEFORE_ROLL_COIN_PARRYING) ? COIN_ROLL_TYPE.PARRYING : COIN_ROLL_TYPE.ACTION;

            BattleActionModel selfAction = unit._actionList.ToSystem().Find(x => x._skill == skill);
            BattleActionModel oppoAction = null;

            BattleActionModelManager battleActionModelManager = Singleton<BattleActionModelManager>.Instance;
            if (selfAction != null) oppoAction = battleActionModelManager.GetDuelOpponentAction(selfAction);

            int result = -1;

            switch (circles[2])
            {
                case "Scale":
                    result = skill.GetCoinScale();
                    break;
                case "CoinScaleAdder":
                    result = (selfAction != null && oppoAction != null) ? skill.GetCoinScaleAdder(selfAction, selectedCoins[0], oppoAction) : -1;
                    break;
                case "Final":
                    result = (selfAction != null) ? skill.GetSkillPowerResultAdder(selfAction, modular.battleTiming, selectedCoins[0]) : -1;
                    break;
                case "Clash":
                    result = (selfAction != null) ? skill.GetParryingResultAdder(selfAction, selfAction.skillPowerResultValue, oppoAction, oppoAction.skillPowerResultValue) : -1;
                    break;
                case "Weight":
                    result = (selfAction != null) ? skill.GetAttackWeight(selfAction) : -1;
                    break;
                case "ogWeight":
                    result = skill.GetOriginAttackWeight();
                    break;
                case "Evade":
                    result = (selfAction != null || oppoAction != null) ? skill.GetEvadeSkillPowerAdder(selfAction, oppoAction) : -1;
                    break;
                case "Default":
                    result = skill.GetSkillDefaultPower();
                    break;
                case "DefaultAdder":
                    result = skill.GetSkillPowerAdder(selfAction, rollType, skill.CoinList);
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
                    if (selfAction == null || oppoAction == null) return -1;
                    result = (skill.CanDuel(selfAction, oppoAction)) ? 1 : 0;
                    break;
                case "Rank":
                    result = skill.GetSkillTier();
                    break;
                case "Fixed":
                    if (selfAction == null) return -1;
                    result = (skill.CanBeChangedTarget(selfAction)) ? 1 : 0;
                    break;
                case "Attribute":
                    result = (int)skill.GetAttributeType();
                    break;
                case "EgoType":
                    result = (int)skill.GetSkillEgoType();
                    break;
                case "IsAction":
                    result = (unit._actionList.ToSystem().Find(x => x.Skill == skill) != null) ? 1 : 0;
                    break;
                case "UseCount":
                    result = unit._actionList.ToSystem().FindAll(x => x.GetSkillID() == skill.GetID()).Count;
                    break;
                case "TargetClash":
                    result = (selfAction != null || oppoAction != null) ? skill.GetOpponentParryingResultAdder(selfAction, selfAction.skillPowerResultValue, oppoAction, oppoAction.skillPowerResultValue) : -1 ;
                    break;
                case "TargetType":
                    result = (int)skill.GetSkillTargetType();
                    break;
                case "TargetCount":
                    result = selfAction.GetAttakedTargetList().Count;
                    break;
                case "RealTargetCount":
                    result = selfAction._realAttakedTargetList.Count;
                    break;
                case "IsTargettingName":
                    if (selfAction == null && isNotCoin) return -1;
                    result = (selfAction.GetAttakedTargetList().ToSystem().Find(x => x.GetName() == circles[3]) != null) ? 1 : 0;
                    break;
                case "IsTargettingUniqueName":
                    if (selfAction == null && !isNotCoin) return -1;
                    result = (selfAction.GetAttakedTargetList().ToSystem().Find(x => x.GetUniqueName() == circles[3]) != null) ? 1 : 0;
                    break;
                case "IsTargettingID":
                    if (selfAction == null && !isNotCoin) return -1;
                    result = (selfAction.GetAttakedTargetList().ToSystem().Find(x => x.GetCharacterID() == modular.GetNumFromParamString(circles[3])) != null) ? 1 : 0;
                    break;
                case "IsTargettingMainName":
                    if (selfAction == null && selfAction.GetMainTarget() == null) return -1;
                    result = (selfAction.GetMainTarget().GetName() == circles[3]) ? 1 : 0;
                    break;
                case "IsTargettingMainUniqueName":
                    if (selfAction == null && selfAction.GetMainTarget() == null) return -1;
                    result = (selfAction.GetMainTarget().GetUniqueName() == circles[3]) ? 1 : 0;
                    break;
                case "IsTargettingMainID":
                    if (selfAction == null && selfAction.GetMainTarget() == null) return -1;
                    result = (selfAction.GetMainTarget().GetCharacterID() == modular.GetNumFromParamString(circles[3])) ? 1 : 0;
                    break;
                default:
                    result = skill.GetID();
                break;
            }

            return result;
        }
    }
}
