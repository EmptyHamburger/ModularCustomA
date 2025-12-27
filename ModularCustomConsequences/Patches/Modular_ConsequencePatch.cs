using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;
using System.Text.RegularExpressions;
using MTCustomScripts;
internal class Modular_Consequence
{
    [HarmonyPatch(typeof(ModularSA), "Consequence")]
    [HarmonyPrefix, HarmonyPriority(Priority.VeryHigh)]
    public static bool Prefix_ModularSA_Consequence(ref string section, ModularSA __instance)
    {
        try
        {
            section = Regex.Replace(section, @"\[([^\[\]]*)\]", match =>
            {
                string key = string.Format("{0}{1}", __instance.ptr_intlong, match.Groups[1].Value);
                return MTCustomScripts.Main.TestStuffStorage.stringDict.TryGetValue(key, out string value) ? value : match.Groups[1].Value;
            });
        }
        catch (System.Exception ex)
        {
            MainClass.Logg.LogInfo(ex);
        }

        return true;
    }
}