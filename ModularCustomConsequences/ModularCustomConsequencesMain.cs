using BepInEx;
using BepInEx.Unity.IL2CPP;
using Lethe;
using ModularSkillScripts;
using MTCustomScripts.Acquirers;
using MTCustomScripts.Consequences;
using MTCustomScripts.LuaFunctions;
using Lua;
using System;
using System.Text.Json;
using ModularSkillScripts.LuaFunction;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Lethe.Patches;
using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using HarmonyLib;
using Unity.Mathematics;
using View;
using UnityEngine;
using System.Linq;
using Utils;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime.Runtime;
using ModularSkillScripts.Patches;
using SharpCompress;
using System.ComponentModel;
using System.Text.RegularExpressions;
using BepInEx.Logging;

namespace MTCustomScripts;

[BepInPlugin(GUID, NAME, VERSION)]
// this is required to make your plugin run AFTER Modular has been loaded.
[BepInDependency("GlitchGames.ModularSkillScripts")]

public class Main : BasePlugin
{
    // Edit the below to your own plugin name, version, etc.
    public const string NAME = "MTCustomScripts";
    public const string VERSION = "1.33.14";
    public const string AUTHOR = "MT";
    public const string GUID = $"{AUTHOR}.{NAME}";

    public int special_slotindex = -11;

    public System.Collections.Generic.Dictionary<long, BUFF_UNIQUE_KEYWORD> keywordTriggerDict = new System.Collections.Generic.Dictionary<long, BUFF_UNIQUE_KEYWORD>();
    // public BUFF_UNIQUE_KEYWORD keywordTrigger = BUFF_UNIQUE_KEYWORD.None;

    public BUFF_UNIQUE_KEYWORD gainbuff_keyword = BUFF_UNIQUE_KEYWORD.None;

    public int gainbuff_stack = 0;

    public int gainbuff_turn = 0;

    public int gainbuff_activeRound = 0;

    public ABILITY_SOURCE_TYPE gainbuff_source = ABILITY_SOURCE_TYPE.NONE;

    public System.Collections.Generic.Dictionary<string, string> templateDict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public class GlobalLuaValues
    {
        private static GlobalLuaValues _instance;

        public static GlobalLuaValues Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GlobalLuaValues();
                return _instance;
            }
        }

        private GlobalLuaValues() { }
        public System.Collections.Generic.Dictionary<string, LuaValue> gvars = new System.Collections.Generic.Dictionary<string, LuaValue>();

        public void SetGlobalValue(string key, LuaValue newVal)
        {
            if (key == null) return;
            gvars[key] = newVal;
        }

        public LuaValue GetGlobalValue(string key)
        {
            if (key == null || !gvars.TryGetValue(key, out LuaValue value))
                return LuaValue.Nil;
            return value;
        }

        public void ClearAllValue()
        {
            gvars = new System.Collections.Generic.Dictionary<string, LuaValue>();
        }
    }
    
    public static class Decode

    {
        public static LuaValue decode(string strjson)
        {
            var jsonElem = convert(JsonDocument.Parse(strjson).RootElement);
            return jsonElem;
        }
        private static LuaValue convert(JsonElement raw)
        {
            switch (raw.ValueKind)
            {
                default:
                    return LuaValue.Nil;

                case JsonValueKind.String:
                    return raw.GetString();
                case JsonValueKind.Number:
                    if (raw.TryGetInt64(out var longV)) return longV;
                    return raw.GetDouble();

                case JsonValueKind.Object:
                    var newTable = new LuaTable();
                    foreach (var value in raw.EnumerateObject())
                    {
                        newTable[value.Name] = convert(value.Value);
                    }
                    return newTable;

                case JsonValueKind.Array:
                    var newTable1 = new LuaTable();
                    int startIndex = 1;
                    foreach (var value in raw.EnumerateArray())
                    {
                        newTable1[startIndex++] = convert(value);
                    }
                    return newTable1;

                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return LuaValue.Nil;
            }
        }
    }
    
    public class TestStuffStorage
    {
        private static TestStuffStorage _instance;

        public static TestStuffStorage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestStuffStorage();
                return _instance;
            }
        }
        public System.Collections.Generic.Dictionary<string, string> GetDictionnary()
        {
            return stringDict;
        }

        // public static ModularSA testModular = new ModularSA();

        // public static string[] GetStringComparerResultValues = { "BREATH", "=", "BuffKeyword_category" };

        // public static string[] HasBuffKeywordValue = {"main", "Breath", "print" };

        // public static string[] EditBuffMaxValues = { "both", "both", "set", "5", "info" };

        public static System.Collections.Generic.Dictionary<string, string> stringDict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public class ConsequenceTest : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            // var modelList = modular.GetTargetModelList(circles[0]);
            // if (modelList.Count < 1) return;

            var targetmodel = modular.GetTargetModel(circles[0]);
            if (targetmodel == null) return;

            foreach(SinActionModel bam in targetmodel.GetSinActionList())
            {
                bam.actionSlot.AddTargetIdx(6, 1);
            }

            // SinActionModel fromSinAction_new = targetmodel.AddNewSinActionModel();
            // UnitSinModel fromSinModel_new = new UnitSinModel(1071203, targetmodel, fromSinAction_new);
            // BattleActionModel fromAction_new = new BattleActionModel(fromSinModel_new, targetmodel, fromSinAction_new);
            // fromSinAction_new.target

            // Il2CppSystem.Collections.Generic.List<BattleUnitModel> allylist = modular.GetTargetModelList("AllyExceptSelf12");

            // var TargetSinActionList = new Il2CppSystem.Collections.Generic.List<SinActionModel>();
            // var targetactionlist = targetmodel.GetSinActionList();
            // if (targetactionlist.Count > 0) TargetSinActionList.Add(targetactionlist.ToArray()[0]);

            // foreach(BattleUnitModel targetModel in modelList)
            // {
            //     foreach(BattleActionModel bam in targetModel._actionList)
            //     {
            //         bam._targetDataDetail.ReadyOriginTargeting(bam);
            //         if (TargetSinActionList.Count > 0)
            //         {
            //             bam.ChangeMainTargetSinAction(TargetSinActionList.ToArray()[0], null, true);
            //         }
            //     }
            // }

            // var bamManager = Singleton<BattleActionModelManager>.Instance;
            // var sinManager = Singleton<SinManager>.Instance;
            // var battleObjectManager = sinManager._battleObjectManager;
            // var modelList = battleObjectManager.GetModelList();

            // // var allySinActionList = new Il2CppSystem.Collections.Generic.List<SinActionModel>();

            // // foreach(BattleUnitModel allyModel in allylist)
            // // {
            // //     foreach(SinActionModel sam in allyModel.GetSinActionList())
            // //     {
            // //         allySinActionList.Add(sam);
            // //     }
            // // }
            // Il2CppSystem.Collections.Generic.List<SinActionModel> other8726 = new Il2CppSystem.Collections.Generic.List<SinActionModel>();
            // Il2CppSystem.Collections.Generic.List<SinActionModel> other8727 = new Il2CppSystem.Collections.Generic.List<SinActionModel>();
            // foreach(BattleUnitModel targetModel in modelList)
            // {
            //     if (targetModel.GetUnitID() == 8726)
            //     {
            //         other8726 = targetModel.GetSinActionList();
            //     }
            //     if (targetModel.GetUnitID()== 8727)
            //     {
            //         other8727 = targetModel.GetSinActionList();
            //     }
            // //     // targetModel._actionSlotDetail.AddSkillToSkillPool(1071203, 1);
            // //     // targetModel.OverwriteTargetableList();
            // //     foreach(BattleActionModel bam in targetModel._actionList)
            // //     {
            // //         targetModel.OverwriteTargetableList(bam, targetModel.GetSinActionList(), allylist);
            // //     }
            // }

            // foreach(BattleUnitModel targetModel in modelList)
            // {
            //     if (targetModel.GetUnitID() == 8726)
            //     {
            //         foreach(BattleActionModel bam in targetModel.GetSortedActionList())
            //         {
            //             targetModel.OverwriteTargetableList(bam, other8727, null);
            //         }
            //     }
            //     if (targetModel.GetUnitID() == 8727)
            //     {
            //         foreach(BattleActionModel bam in targetModel.GetSortedActionList())
            //         {
            //             targetModel.OverwriteTargetableList(bam, other8726, null);
            //         }
            //     }
            // }
        }
    }

    public static Main Instance;

    public static ManualLogSource Logger;

    public override void Load()
    {
        Instance = this;
        Logger = Log;

        Harmony harmony = new Harmony(NAME);
        harmony.PatchAll(typeof(Patch_DefenseChange));
        harmony.PatchAll(typeof(RightAfterGetAnyBuff));
        harmony.PatchAll(typeof(Modular_SetupModular));
        // harmony.PatchAll(typeof(RightAfterGiveBuffBySkill));
        harmony.PatchAll(typeof(Modular_Consequence));

        MainClass.timingDict.Add("OnGainBuff", 1337);
        // MainClass.timingDict.Add("OnInflictBuff", 1733);

        MainClass.luaFunctionDict["jsontolua"] = new LuaFunctionJsonDecoder();
        MainClass.luaFunctionDict["listdirectories"] = new MTCustomScripts.LuaFunctions.LuaFunctionListDirectories();
        MainClass.luaFunctionDict["listbuffs"] = new MTCustomScripts.LuaFunctions.LuaFunctionListBuffs();
        MainClass.luaFunctionDict["setgdata"] = new LuaFunctionSetGlobalVarMT();
        MainClass.luaFunctionDict["getgdata"] = new LuaFunctionGetGlobalVarMT();
        MainClass.luaFunctionDict["clearallgdata"] = new LuaFunctionClearGlobalVarMT();
        MainClass.luaFunctionDict["gbkeyword"] = new LuaFunctionGainBuffKeyword();
        MainClass.luaFunctionDict["getcurrentmapid"] = new GetCurrentMapID();
        MainClass.luaFunctionDict["listrelatedkeywords"] = new LuaFunctionListRelatedKeywords();
 
        MainClass.acquirerDict["coinoperator"] = new AcquirerCoinOperator();
        MainClass.acquirerDict["bufftype"] = new AcquirerBuffType();
        MainClass.acquirerDict["getatkres"] = new AcquirerAtkResistance();
        MainClass.acquirerDict["getsinres"] = new AcquirerSinResistance();
        MainClass.acquirerDict["useddefaction"] = new AcquirerIfUsedDefenseActionThisTurn();
        MainClass.acquirerDict["unitfaction"] = new AcquirerUnitFaction();
        MainClass.acquirerDict["saslotindex"] = new AcquirerSpecialActionSlotIndex();
        MainClass.acquirerDict["gbstack"] = new AcquirerGainBuffStack();
        MainClass.acquirerDict["gbturn"] = new AcquirerGainBuffTurn();
        MainClass.acquirerDict["gbactiveround"] = new AcquirerGainBuffActiveRound();
        MainClass.acquirerDict["gbsource"] = new AcquirerGainBuffSource();
        MainClass.acquirerDict["comparestring"] = new AcquirerGetStringComparerResult();
        MainClass.acquirerDict["hasbuffkeyword"] = new AcquirerHasBuffKeyword();
        MainClass.acquirerDict["getmapdata"] = new AcquirerGetMapData();

        MainClass.consequenceDict["ovwatkres"] = new ConsequenceOverwriteAtkResist();
        MainClass.consequenceDict["ovwsinres"] = new ConsequenceOverwriteSinResist();
        MainClass.consequenceDict["refreshspeed"] = new ConsequenceRefreshSpeed();
        MainClass.consequenceDict["destroybuff"] = new ConsequenceDestroyBuff();
        MainClass.consequenceDict["deactivebreak"] = new ConsequenceDeactiveBreakSections();
        MainClass.consequenceDict["bufcategory"] = new ConsequenceBuffCategory();
        MainClass.consequenceDict["defcorrection"] = new ConsequenceDefCorrectionSet();
        MainClass.consequenceDict["addunitscript"] = new ConsequenceAddUnitScript();
        MainClass.consequenceDict["changedefense"] = new ConsequenceChangeDefense();
        MainClass.consequenceDict["editbuffmax"] = new ConsequenceEditBuffMax();

        // MainClass.consequenceDict["test"] = new ConsequenceTest();
        // MainClass.consequenceDict["testthree"] = new ConsequenceTest3();
        // MainClass.consequenceDict["reload"] = new ConsequenceReload();
    }
}