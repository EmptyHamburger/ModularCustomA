using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using DiscordRPC;
using HarmonyLib;
using Lethe;
using Lethe.Patches;
using Lua;
using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using ModularSkillScripts.Patches;
using MTCustomScripts.Acquirers;
using MTCustomScripts.Consequences;
using MTCustomScripts.LuaFunctions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace MTCustomScripts;

[BepInPlugin(GUID, NAME, VERSION)]
// this is required to make your plugin run AFTER Modular has been loaded.
[BepInDependency("GlitchGames.ModularSkillScripts")]

public class Main : BasePlugin
{
    // Edit the below to your own plugin name, version, etc.
    public const string NAME = "MTCustomScripts";
    public const string VERSION = "1.33.16";
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
        public static void AddSkill(BattleUnitModel unit, int skillId, int skillAmt)
        {
            SkillStaticDataList skillList = Singleton<StaticDataManager>.Instance._skillList;
            SkillStaticData data = skillList.GetData(skillId);
            UnitAttribute skillAttribute = new UnitAttribute();
            skillAttribute.number = skillAmt;
            skillAttribute.skillId = skillId;
            SkillModel skillModel = new SkillModel(data, 55, 4);


            unit.UnitDataModel._unitAttributeList.Add(skillAttribute);
            unit.UnitDataModel._skillList.Add(skillModel);
        }

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

        public static ModularSA testModular = new ModularSA();

        public static string[] StringArrayGenerator(string circle) { return circle.Split('|'); }

        public static System.Collections.Generic.Dictionary<string, string> stringDict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE> overrideBuffPanicDict = new System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE>();
    }

    public class ConsequenceTest : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            
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
        harmony.PatchAll(typeof(BuffModel_OverwritePanic));
        harmony.PatchAll(typeof(PanicOrLowMorale));

        // MainClass.timingDict.Add("OnGainBuff", 1337);
        // MainClass.timingDict.Add("OnInflictBuff", 1733);
        MainClass.timingDict.Add("OnPanic", 90901);
        MainClass.timingDict.Add("OnOtherPanic", 909012);
        MainClass.timingDict.Add("OnLowMorale", 90903);
        MainClass.timingDict.Add("OnOtherLowMorale", 90904);
        MainClass.timingDict.Add("OnRecoverBreak", 90905);
        MainClass.timingDict.Add("OnOtherRecoverBreak", 90906);


        MainClass.luaFunctionDict["jsontolua"] = new MTCustomScripts.LuaFunctions.LuaFunctionJsonDecoder();
        MainClass.luaFunctionDict["listdirectories"] = new MTCustomScripts.LuaFunctions.LuaFunctionListDirectories();
        MainClass.luaFunctionDict["listbuffs"] = new MTCustomScripts.LuaFunctions.LuaFunctionListBuffs();
        MainClass.luaFunctionDict["setgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionSetGlobalVarMT();
        MainClass.luaFunctionDict["getgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionGetGlobalVarMT();
        MainClass.luaFunctionDict["clearallgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionClearGlobalVarMT();
        MainClass.luaFunctionDict["gbkeyword"] = new MTCustomScripts.LuaFunctions.LuaFunctionGainBuffKeyword();
        MainClass.luaFunctionDict["getcurrentmapid"] = new MTCustomScripts.LuaFunctions.GetCurrentMapID();
        MainClass.luaFunctionDict["listrelatedkeywords"] = new MTCustomScripts.LuaFunctions.LuaFunctionListRelatedKeywords();
 
        MainClass.acquirerDict["coinoperator"] = new MTCustomScripts.Acquirers.AcquirerCoinOperator();
        MainClass.acquirerDict["bufftype"] = new MTCustomScripts.Acquirers.AcquirerBuffType();
        MainClass.acquirerDict["getatkres"] = new MTCustomScripts.Acquirers.AcquirerAtkResistance();
        MainClass.acquirerDict["getsinres"] = new MTCustomScripts.Acquirers.AcquirerSinResistance();
        MainClass.acquirerDict["useddefaction"] = new MTCustomScripts.Acquirers.AcquirerIfUsedDefenseActionThisTurn();
        MainClass.acquirerDict["unitfaction"] = new MTCustomScripts.Acquirers.AcquirerUnitFaction();
        MainClass.acquirerDict["saslotindex"] = new MTCustomScripts.Acquirers.AcquirerSpecialActionSlotIndex();
        MainClass.acquirerDict["gbstack"] = new MTCustomScripts.Acquirers.AcquirerGainBuffStack();
        MainClass.acquirerDict["gbturn"] = new MTCustomScripts.Acquirers.AcquirerGainBuffTurn();
        MainClass.acquirerDict["gbactiveround"] = new MTCustomScripts.Acquirers.AcquirerGainBuffActiveRound();
        MainClass.acquirerDict["gbsource"] = new MTCustomScripts.Acquirers.AcquirerGainBuffSource();
        MainClass.acquirerDict["comparestring"] = new MTCustomScripts.Acquirers.AcquirerGetStringComparerResult();
        MainClass.acquirerDict["hasbuffkeyword"] = new MTCustomScripts.Acquirers.AcquirerHasBuffKeyword();
        MainClass.acquirerDict["getmapdata"] = new MTCustomScripts.Acquirers.AcquirerGetMapData();
        MainClass.acquirerDict["getfinal"] = new MTCustomScripts.Acquirers.AcquirerGetFinalPower();
        MainClass.acquirerDict["getpaniclevel"] = new MTCustomScripts.Acquirers.AcquirerGetPanicLevel();

        MainClass.consequenceDict["ovwatkres"] = new MTCustomScripts.Consequences.ConsequenceOverwriteAtkResist();
        MainClass.consequenceDict["ovwsinres"] = new MTCustomScripts.Consequences.ConsequenceOverwriteSinResist();
        MainClass.consequenceDict["refreshspeed"] = new MTCustomScripts.Consequences.ConsequenceRefreshSpeed();
        MainClass.consequenceDict["destroybuff"] = new MTCustomScripts.Consequences.ConsequenceDestroyBuff();
        MainClass.consequenceDict["deactivebreak"] = new MTCustomScripts.Consequences.ConsequenceDeactiveBreakSections();
        MainClass.consequenceDict["bufcategory"] = new MTCustomScripts.Consequences.ConsequenceBuffCategory();
        MainClass.consequenceDict["defcorrection"] = new MTCustomScripts.Consequences.ConsequenceDefCorrectionSet();
        MainClass.consequenceDict["addunitscript"] = new MTCustomScripts.Consequences.ConsequenceAddUnitScript();
        MainClass.consequenceDict["changedefense"] = new MTCustomScripts.Consequences.ConsequenceChangeDefense();
        MainClass.consequenceDict["editbuffmax"] = new MTCustomScripts.Consequences.ConsequenceEditBuffMax();
        MainClass.consequenceDict["changepaniclevel"] = new MTCustomScripts.Consequences.ConsequenceChangePanicLevel();
        MainClass.consequenceDict["changepanictype"] = new MTCustomScripts.Consequences.ConsequenceChangePanicType();
        MainClass.consequenceDict["piraterichpresence"] = new MTCustomScripts.Consequences.ConsequenceModifyRichPresence();
        //MainClass.consequenceDict["addcoin"] = new MTCustomScripts.Consequences.ConsequenceAddCoin();
        // MainClass.consequenceDict["test"] = new ConsequenceTest();
        // MainClass.consequenceDict["testthree"] = new ConsequenceTest3();
        // MainClass.consequenceDict["reload"] = new ConsequenceReload();
    }
}