using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceDynamicLocaleClearOneActivePaths : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {      
            if (modular.modsa_buffModel == null) MTCustomScripts.Main.Logger.LogFatal("[Dynamic Locale] BuffModel not found! - ClearOneActivePaths");
            MTCustomScripts.Main.DynamicLocale_ClearOneActivePaths(modular.modsa_buffModel);
        }
    }
}
