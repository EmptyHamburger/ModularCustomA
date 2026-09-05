using ModularSkillScripts;
using Il2CppSystem.Collections.Generic;
using System.Reflection;
using System;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetMangCount : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleActionModel action = modular.modsa_selfAction;
            if (circles[0] != "Self") action = modular.modsa_oppoAction;

            int totalMangCount = 0;

            foreach(SkillAbility skillAbility in action._skill._skillAbilityList)
            {
                MethodInfo method = skillAbility.GetType().GetMethod(
                    "GetCreateMangCount",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null);

                if (method == null) continue;

                int mangCount = (int) method.Invoke(skillAbility, null);
                totalMangCount += mangCount;
                Main.Logger.LogFatal($"{skillAbility.GetType().Name} - Mang Count = {mangCount}");
            }

            return totalMangCount;
        }
    }
}
