using ModularSkillScripts;
using System;
using MTCustomScripts;

namespace MTCustomScripts.Consequences;

public class ConsequenceForceEndClash : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        ParryingStatus parryingStatus = MTCustomScripts.Main.Instance.currentParryingStatus;
        BattleLog_Parrying battleLog_Parrying = MTCustomScripts.Main.Instance.currentBattleLog_Parrying;

        parryingStatus.actorParryingLife = 0;
        parryingStatus.opponentParryingLife = 0;

        string Get(int index) => (circles != null && index < circles.Length) ? circles[index] : null;

        if (Enum.TryParse(Get(0), true, out PARRYING_RESULT self)) battleLog_Parrying._ACharacterResult = self;
        if (Enum.TryParse(Get(1), true, out PARRYING_RESULT oppo)) battleLog_Parrying._BCharacterResult = oppo;
        if (int.TryParse(Get(2), out int selfTotal)) battleLog_Parrying._ACharacterTotalReuslt = selfTotal;
        if (int.TryParse(Get(3), out int oppoTotal)) battleLog_Parrying._BCharacterTotalReuslt = oppoTotal;
    }
}