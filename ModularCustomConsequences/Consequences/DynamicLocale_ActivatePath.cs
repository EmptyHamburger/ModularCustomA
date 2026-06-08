using ModularSkillScripts;
using System;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceDynamicLocaleActivatePath : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {      
            if (modular.modsa_buffModel == null) MTCustomScripts.Main.Logger.LogFatal("[Dynamic Locale] BuffModel not found! - ActivatePath");
            MTCustomScripts.Main.DynamicLocale_ActivatePath(modular.modsa_buffModel, circles);
        }
    }
}
