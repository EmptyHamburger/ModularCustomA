using ModularSkillScripts;
using Lethe.Patches;
using System;

namespace MTCustomScripts.Consequences;

public class ConsequenceAllowOpenInfo : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        bool allow = modular.GetBoolFromParamString(circles[0]);
        MTCustomScripts.Main._TIMING_OpenInfo_RunOriginMethod = allow;
    }
}