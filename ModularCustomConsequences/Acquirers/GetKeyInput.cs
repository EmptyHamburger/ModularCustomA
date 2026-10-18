using BattleUI.Operation;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.UnityEngine;
using ModularSkillScripts.Consequence;
using Utils;
using System;
using ModularSkillScripts;

namespace MTCustomScripts.Acquirers
{
    public class AcquirerGetKeyInput : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            if (!Enum.TryParse<KeyCode>(circles[0], false, out KeyCode keyCode)) return -1;
            if (Input.GetKeyInt(keyCode)) return 1;
            return 0;
        }
    }
}
