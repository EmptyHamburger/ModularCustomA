using ModularSkillScripts;

namespace MTCustomScripts.Acquirers;

public class AcquirerGetMang : IModularAcquirer
{
    public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
    {
        BattleActionModel action = (circles[0] == "Self") ? modular.modsa_selfAction : modular.modsa_oppoAction;
        if (action == null) return -1;
        SkillModel skill = action._skill;
        if (skill == null) return -1;

        int totalMang = 0;
        
        foreach(var skillAbility in skill._skillAbilityList)
        {
            if (skillAbility.TryCast<ICreateMang>() is ICreateMang iCreateMang)
            {
                totalMang += iCreateMang.IsCreateMang() ? iCreateMang.GetCreateMangCount() : 0;
            }
        }

        return totalMang;
    }
}