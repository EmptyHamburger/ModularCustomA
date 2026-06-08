using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceDynamicLocaleDeactivatePath : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {      
            if (modular.modsa_buffModel == null) MTCustomScripts.Main.Logger.LogFatal("[Dynamic Locale] BuffModel not found! - DeactivatePath");
            MTCustomScripts.Main.DynamicLocale_DeactivtePath(modular.modsa_buffModel, circles);
        }
    }
}
