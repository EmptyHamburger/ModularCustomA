using BepInEx;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using System;
using Il2CppSystem.Collections.Generic;

namespace MTCustomScripts.Consequences;

public class ConsequenceAddSkillAbility : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
         * var_1: multi-target
         * var_2: multi-skill
         * var_3: className
         * var_4: scriptName
         * var_5: turnLimit
         */

        if (circles.Length < 3) return;

        string scriptName = (circles.Length < 4 || circles[3].IsNullOrWhiteSpace() || circles[3].ToLower() == "s") ? circles[2] : circles[3];
        int turnLimit = (circles.Length < 5 || circles[4].IsNullOrWhiteSpace() || circles[4] == "0") ? 0 : modular.GetNumFromParamString(circles[4]);

        List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
        if (targetList == null || targetList.Count <= 0) return;

        System.Collections.Generic.List<SkillModel> skillList = modular.GetMultipleSkillModel(targetList, circles[1]);
        if (skillList == null || skillList.Count <= 0) return;

        string skillScriptName = circles[2];

        if (circles[2].Equals("modreplace", StringComparison.OrdinalIgnoreCase))
        {
            circles[3] = circles[3].Replace(';', ':').Replace('\\', '/').Replace("<<", "(").Replace(">>", ")");
            circles[3] = "TIMING:" + circles[3];

            foreach (SkillModel skill in skillList)
            {
                long ptr = skill.Pointer.ToInt64();

                ModularSA modsa = new ModularSA();
                modsa.originalString = "Modular/" + circles[3]; ;
                modsa.modsa_skillModel = skill;
                modsa.ptr_intlong = ptr;
                modsa.SetupModular(circles[3]);

                if (!SkillScriptInitPatch.modsaDict.ContainsKey(ptr)) SkillScriptInitPatch.modsaDict.Add(ptr, new Il2CppSystem.Collections.Generic.List<ModularSA>());
                SkillScriptInitPatch.modsaDict[ptr].Add(modsa);
            }
        }
        else
        {
            skillScriptName = $"SkillAbility_{skillScriptName}";
            BuffReferenceData buffData = (circles.Length < 6 || circles[5].IsNullOrWhiteSpace()) ? null : StyxUtils.TreatBuffData(circles[5]);

            try
            {
                foreach (SkillModel skill in skillList)
                {
                    SkillAbility newSkillAbility = (SkillAbility)Activator.CreateInstance(typeof(SkillAbility).Assembly.GetType(skillScriptName));
                    skill._skillAbilityList.Add(newSkillAbility);
                    newSkillAbility.Init(skill, scriptName, 0, skill.SkillAbilityList.Count, turnLimit, buffData);
                }
            }
            catch (Exception msg) { Main.Logger.LogError($"Couldn't add skill script '{skillScriptName}': {msg}"); }
        }
    }
}