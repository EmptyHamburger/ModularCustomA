using Lethe.EnumInjections;
using ModularSkillScripts;
using System;
using System.Linq;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddCoinAbilityBasicBuff : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        SkillModel skill = modular.modsa_skillModel;
        if (circles[0] != "Self") skill = modular.modsa_oppoAction.Skill;
        if (skill == null) return;
        string coinScriptName = circles[1]; //ForceToActivateBuffOSA

        int idx = modular.GetNumFromParamString(circles[2]);
        int turnLimit = modular.GetNumFromParamString(circles[3]);
        BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[4]);
        int stack = modular.GetNumFromParamString(circles[5]);
        int turn = modular.GetNumFromParamString(circles[6]);
        int activeRound = modular.GetNumFromParamString(circles[7]);

        CoinAbility_BasicBuff newCoinAbility = (CoinAbility_BasicBuff)Activator.CreateInstance(typeof(CoinAbility_BasicBuff));

        newCoinAbility._activeRound = activeRound;
        newCoinAbility._stack = stack;
        newCoinAbility._turn = turn;

        newCoinAbility.Init(skill.GetCoinByIndex(idx), idx, coinScriptName, 0f, turnLimit, BuffAbilityManager.GetBuffAbility(keyword.ToString())._info);
        

        // try
        // {
        //     if (circles.Length > 2)
        //     {
        //         foreach (string circle in circles.Skip(2))
        //         {
        //             CoinAbility newCoinAbility = (CoinAbility)Activator.CreateInstance(typeof(CoinAbility).Assembly.GetType(coinScriptName));
        //             int idx = modular.GetNumFromParamString(circle);
        //             if (idx < 0)
        //             {
        //                 modular.modsa_coinModel._coinAbilityList.Add(newCoinAbility);
        //                 continue;
        //             }
        //             idx = Math.Min(idx, modular.modsa_skillModel.CoinList.Count - 1);
        //             modular.modsa_skillModel.GetCoinByIndex(idx)._coinAbilityList.Add(newCoinAbility);
        //         }
        //         return;
        //     }
        //     foreach(CoinModel CM in modular.modsa_skillModel._coinList)
        //     {
        //         CoinAbility newCoinAbility = (CoinAbility)Activator.CreateInstance(typeof(CoinAbility).Assembly.GetType(coinScriptName));
        //         CM._coinAbilityList.Add(newCoinAbility);
        //     }
        // }
        // catch (Exception msg)
        // {
        //     MTCustomScripts.Main.Logger.LogError($"Couldn't add coin script '{coinScriptName}': {msg}");
        // }
    }
}