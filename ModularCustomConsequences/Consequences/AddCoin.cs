using Il2CppSystem;
using ModularSkillScripts;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceAddCoin : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: Multi-Target
             * var_2: Mult-Skill
             * var_3: CoinIndex
             * var_4: CopyCoinTarget/Power
             * var_5: CopyCoinSkill/Operator
             * var_6: CopyCoinIndex/Color
             * opt-7: CopyStaticOnly
             */

            try
            {

                Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
                System.Collections.Generic.List<SkillModel> skillList = modular.GetMultipleSkillModel(unitList, circles[1]);

                CoinModel newCoin = null;
                int coinIndex = modular.GetNumFromParamString(circles[2]);


                if (circles[3].StartsWith("copy"))
                {
                    BattleUnitModel targetUnit = modular.GetTargetModel(circles[3].Substring(4));
                    SkillModel targetSkill = modular.GetSingleSkillModel(targetUnit, circles[4]);
                    CoinModel targetCoin = modular.GetSingleCoin(targetSkill, circles[5], null);

                    if (circles[6].Equals("CopyStatic", System.StringComparison.OrdinalIgnoreCase)) newCoin = new CoinModel(targetCoin.ClassInfo, coinIndex);
                    else newCoin = new CoinModel(targetCoin, false);
                }
                else
                {
                    SkillCoinData newCoinData = new SkillCoinData();
                    newCoinData.scale = modular.GetNumFromParamString(circles[3]);
                    newCoinData._operatorType = (Il2CppSystem.Enum.TryParse<OPERATOR_TYPE>(circles[4], true, out OPERATOR_TYPE opType)) ? opType : OPERATOR_TYPE.ADD;
                    newCoinData._coinColorType = (Il2CppSystem.Enum.TryParse<COIN_COLOR_TYPE>(circles[5], true, out COIN_COLOR_TYPE colorType)) ? colorType : COIN_COLOR_TYPE.GOLD;
                    if (newCoinData._coinColorType == COIN_COLOR_TYPE.GOLD) newCoinData.grade = 1;
                    else if (newCoinData._coinColorType == COIN_COLOR_TYPE.GREY || newCoinData._coinColorType == COIN_COLOR_TYPE.PURPLE)
                    {
                        newCoinData.abilityScriptList.Add(new AbilityData() { scriptName = "SuperCoin" });
                        newCoinData.grade = 2;
                    }
                    else if (newCoinData._coinColorType == COIN_COLOR_TYPE.GREEN)
                    {
                        newCoinData.abilityScriptList.Add(new AbilityData() { scriptName = "ExtractCoin" });
                        newCoinData.grade = 99;
                    }

                    newCoin = new CoinModel(newCoinData, coinIndex);
                }

                foreach (SkillModel selectedSkill in skillList)
                {
                    CoinModel newCoinCopy = newCoin.Copy();
                    newCoinCopy.SetRealCoinIndex(coinIndex);
                    newCoinCopy.SetCoinIndex(selectedSkill.CoinList.Count);

                    selectedSkill.AddCoin(newCoinCopy);
                }
            }
            catch (System.Exception ex) { Main.Logger.LogError("ConsequenceAddCoin error: " + ex); }
        }
    }
}