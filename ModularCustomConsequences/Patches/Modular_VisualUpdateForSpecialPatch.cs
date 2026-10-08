using HarmonyLib;
using Lethe.Patches;
using ModularSkillScripts;
using ModularSkillScripts.Patches;
using MTCustomScripts.Utils;
using Il2CppSystem;

namespace MTCustomScripts.Patches;

internal class Modular_VisualUpdateForSpecialPatch
{
    [HarmonyPatch(typeof(UniquePatches), nameof(UniquePatches.VisualUpdateForSpecial))]
	[HarmonyPrefix]
	public static bool Prefix_UniquePatches_VisualUpdateForSpecial()
    {
        MTUtil.UpdateState();
        return false;
    }
}
