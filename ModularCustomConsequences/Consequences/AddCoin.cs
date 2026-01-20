using ModularSkillScripts;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceAddCoin : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            //TO REVAMP//

            /*
             * var_1: Multi-Target
             * var_2: Skill
             * var_3: CoinIndex
             * var_4: Power
             * var_5: Operator
             * var_6: Type
             * opt_7: Abilities
             * opt_8: CopyCoinTarget
             * opt_9: CopyCoinSkill
             * opt_10: CopyCoinIndex
             * opt_11: CopyStatic
             */

            
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList == null || unitList.Count <= 0) return;
            if (!Il2CppSystem.Enum.TryParse<OPERATOR_TYPE>(circles[4], true, out OPERATOR_TYPE coinOperator)) coinOperator = OPERATOR_TYPE.NONE;
            if (!Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(circles[5], true, out COIN_COLOR_TYPE coinColor)) coinColor = COIN_COLOR_TYPE.NONE;
            bool copyCoin = (circles.Length >= 10 && circles[7] != string.Empty && circles[8] != string.Empty && circles[9] != string.Empty);

            CoinModel newCoin = null;

            SkillCoinData coinData = new SkillCoinData();
            if (copyCoin)
            {
                int opt8Value = modular.GetNumFromParamString(circles[8]);
                int opt9Value = modular.GetNumFromParamString(circles[9]);

                Il2CppSystem.Predicate<BattleActionModel> copyPredicate = new System.Predicate<BattleActionModel>((action) => action.GetSkillID() == opt8Value).ToIL2CPP();


                BattleUnitModel copyTarget = modular.GetTargetModel(circles[7]);
                SkillModel copySkill = StyxUtils.SafeGetAbility(copyPredicate, copyTarget, opt8Value);
                if (copySkill == null) return;

                if (copySkill.GetCoinByIndex(opt9Value) == null) return;
                if (circles.Length >= 11 && circles[10].Equals("CopyStatic", System.StringComparison.OrdinalIgnoreCase)) coinData = copySkill.GetCoinByIndex(opt9Value).ClassInfo;
                else newCoin = new CoinModel(copySkill.GetCoinByIndex(opt9Value), false);
            }

            Il2CppSystem.Predicate<AbilityData> grayDataPredicate = new System.Predicate<AbilityData>((data) =>data.scriptName == "SuperCoin").ToIL2CPP();

            if (coinOperator != OPERATOR_TYPE.NONE) coinData._operatorType = coinOperator;
            if (coinOperator != OPERATOR_TYPE.NONE) coinData.operatorType = circles[4];
            if (coinColor != COIN_COLOR_TYPE.NONE) coinData._coinColorType = coinColor;
            if (coinColor != COIN_COLOR_TYPE.NONE) coinData.color = circles[5];
            if (coinColor != COIN_COLOR_TYPE.NONE) coinData.grade = (int)coinColor;
            if (circles[3] != string.Empty && !circles[3].Equals("Null", System.StringComparison.OrdinalIgnoreCase)) coinData.scale = modular.GetNumFromParamString(circles[3]);


            if (circles[5].Equals("Grey", System.StringComparison.OrdinalIgnoreCase) && coinData.abilityScriptList.Find(grayDataPredicate) == null)
                coinData.abilityScriptList.Add(new AbilityData() { scriptName = "SuperCoin" });

            else if (circles[5].Equals("Gold", System.StringComparison.OrdinalIgnoreCase)) coinData.abilityScriptList.RemoveAll(grayDataPredicate);




            int var2Value = modular.GetNumFromParamString(circles[1]);
            int var3Value = modular.GetNumFromParamString(circles[2]);

            if (copyCoin == false) newCoin = new CoinModel(coinData, 99);
            if (newCoin == null) return;

            if (circles.Length >= 7 && !circles[6].Equals("Null", System.StringComparison.OrdinalIgnoreCase)) StyxUtils.TreatCoinAbilities(newCoin.ClassInfo, circles[6]);
            newCoin.CreateCoinAbilities();


            Il2CppSystem.Predicate<CoinAbility> grayAbilityPredict = new System.Predicate<CoinAbility>((ability) => ability is CoinAbility_OverwriteToSuperCoin || ability is CoinAbility_SuperCoin).ToIL2CPP();

            if (circles[5].Equals("Grey", System.StringComparison.OrdinalIgnoreCase) && newCoin.CoinAbilityList.Find(grayAbilityPredict) == null)
            {
                newCoin.CoinAbilityList.Add(new CoinAbility_SuperCoin());
                newCoin.CoinAbilityList.Add(new CoinAbility_OverwriteToSuperCoin());
            }
            else if (circles[5].Equals("Gold", System.StringComparison.OrdinalIgnoreCase)) newCoin.CoinAbilityList.RemoveAll(grayAbilityPredict); 

            Il2CppSystem.Predicate<BattleActionModel> skillPredicate = new System.Predicate<BattleActionModel>((action) => action.GetSkillID() == var2Value).ToIL2CPP();
            Il2CppSystem.Predicate<CoinModel> coinPredicate = new System.Predicate<CoinModel>((coin) => coin.GetRealCoinIndex() >= var3Value).ToIL2CPP();
            foreach (BattleUnitModel unit in unitList)
            {
                SkillModel selectedSkill = StyxUtils.SafeGetAbility(skillPredicate, unit, var2Value);
                if (selectedSkill == null) return;

                Il2CppSystem.Collections.Generic.List<CoinModel> coinList = selectedSkill.CoinList.FindAll(coinPredicate);
                foreach (CoinModel coin in coinList) coin.SetRealCoinIndex(coin.GetRealCoinIndex());

                newCoin._originCoinIndex = selectedSkill.CoinList.Count;
                newCoin._logIndex = selectedSkill.CoinList.Count;
                selectedSkill.CoinList[var3Value] = newCoin;
            }
        }
    }
}
