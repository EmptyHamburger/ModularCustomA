using ModularSkillScripts;
using SharpCompress;
using System;
using System.Collections.Generic;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceChangeCoinType : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: Multi-Target
             * var_2: Skill
             * var_3: Multi-Coin
             * var_4: Coin-Type
             */

            if (circles.Length < 5) return;

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList.Count == 0) return;

            Il2CppSystem.Collections.Generic.List<SkillModel> selectedSkills = modular.GetMultipleSkillModel(unitList, circles[1]);
            System.Collections.Generic.List<CoinModel> coinList = new System.Collections.Generic.List<CoinModel>();

            foreach (SkillModel skill in selectedSkills)
            {
                var coins = modular.GetCoinModelList(skill, circles[2], null);
                foreach (var coin in coins) coinList.Add(coin);
            }

            if (circles[1] == null && !(int.TryParse(circles[1], out _) || circles[1].Equals("Current", StringComparison.OrdinalIgnoreCase))) return;



            COIN_COLOR_TYPE coinColor = COIN_COLOR_TYPE.GOLD;
            if (circles[3] == null || !Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(circles[3], true, out coinColor)) return;
            int coinGrade = 1;
            if (coinColor == COIN_COLOR_TYPE.GREY || coinColor == COIN_COLOR_TYPE.PURPLE) coinGrade = 2;
            else if (coinColor == COIN_COLOR_TYPE.GREEN) coinGrade = 99;


            foreach (CoinModel coin in coinList)
            {
                COIN_COLOR_TYPE currentCoinColor = coin.GetCoinColor();
                if (currentCoinColor == coinColor) continue;


                System.Collections.Generic.List<CoinAbility> coinAbilityList = coin.CoinAbilityList.ToSystem();
                if (currentCoinColor == COIN_COLOR_TYPE.GREY || currentCoinColor == COIN_COLOR_TYPE.PURPLE) coinAbilityList.RemoveAll(x => x is CoinAbility_OverwriteToSuperCoin || x is CoinAbility_SuperCoin);
                else if (currentCoinColor == COIN_COLOR_TYPE.GREEN) coinAbilityList.RemoveAll(x => x is CoinAbility_ExtractCoin);

                if (currentCoinColor == COIN_COLOR_TYPE.GREY || currentCoinColor == COIN_COLOR_TYPE.PURPLE) coinAbilityList.Add(new CoinAbility_OverwriteToSuperCoin());
                else if (currentCoinColor == COIN_COLOR_TYPE.GREEN) coinAbilityList.Add(new CoinAbility_ExtractCoin());

                coin._coinAbilityList = coinAbilityList.ToIl2Cpp();
                coin._classInfo.grade = coinGrade;
                coin._classInfo._coinColorType = coinColor;
            }
        }
    }
}
