using ModularSkillScripts;
using System;
using System.Linq;

namespace MTCustomScripts.Consequences
{
    public class ConsequenceDynamicLocaleSetOnePathValue : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {      
            if (modular.modsa_buffModel == null) MTCustomScripts.Main.Logger.LogFatal("[Dynamic Locale] BuffModel not found! - SetOnePathValue");
            MTCustomScripts.Main.DynamicLocale_SetTextBlockValue(modular.modsa_buffModel, modular.GetBoolFromParamString(circles[1]) ? modular.GetNumFromParamString(circles[0]).ToString() : circles[0], circles.Skip(2).ToArray());
        }
    }
}
