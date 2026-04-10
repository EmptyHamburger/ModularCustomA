using BattleUI;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Lethe;
using Lethe.Patches;
using Lua;
using ModularSkillScripts;
using ModularSkillScripts.LuaFunction;
using ModularSkillScripts.Patches;
using MTCustomScripts.Acquirers;
using MTCustomScripts.Consequences;
using MTCustomScripts.LuaFunctions;
using MTCustomScripts.Patches;
using MTCustomScripts.MiscClasses;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using View;

namespace MTCustomScripts;

[BepInPlugin(GUID, NAME, VERSION)]
// this is required to make your plugin run AFTER Modular has been loaded.
[BepInDependency("GlitchGames.ModularSkillScripts")]

public class Main : BasePlugin
{
    // Edit the below to your own plugin name, version, etc.
    public const string NAME = "MTCustomScripts";
    public const string VERSION = "16.72.3.39";
    public const string AUTHOR = "MT";
    public const string GUID = $"{AUTHOR}.{NAME}";

    public Regex waitingRegex = new Regex(@"[wW]ait\(()\)", RegexOptions.Compiled);
    public Regex replaceStringRegex = new Regex(@"\[([^:\[\]]+)(?::([^\[\]]+))?\]", RegexOptions.Compiled);
    public System.Collections.Generic.Dictionary<long, System.Collections.Generic.List<MTModData>> storedMTDataDict = [];
    public System.Collections.Generic.HashSet<int> storedRemoveSkillHash = [];

    public int special_slotindex = -11;

    public System.Collections.Generic.Dictionary<long, BUFF_UNIQUE_KEYWORD> keywordTriggerDict = [];
    // public BUFF_UNIQUE_KEYWORD keywordTrigger = BUFF_UNIQUE_KEYWORD.None;
    public BUFF_UNIQUE_KEYWORD gainbuff_keyword = BUFF_UNIQUE_KEYWORD.None;
    public int gainbuff_stack = 0;
    public int gainbuff_turn = 0;
    public int gainbuff_activeRound = 0;
    public ABILITY_SOURCE_TYPE gainbuff_source = ABILITY_SOURCE_TYPE.NONE;


    public System.Collections.Generic.Dictionary<BattleUnitModel, int[]> changeMpDict = []; 


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
        public System.Collections.Generic.Dictionary<string, LuaValue> gvars = [];

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

    public static string GetCustomMTData(long unit_longptr, string dataID, string dataSource = null)
    {
        var storedMTDataDict = Main.Instance.storedMTDataDict;

        if (storedMTDataDict.TryGetValue(unit_longptr, out System.Collections.Generic.List<MTModData> foundDataList))
        {
            MTModData foundData = null;
            if (dataSource != null) foundData = foundDataList.Find(x => x.dataID == dataID && x.dataSource == dataSource);
            else foundData = foundDataList.Find(x => x.dataID == dataID);

            if (foundData == null) return string.Empty;
            return foundData.dataValue;
        }

        return string.Empty;
    }
    public static void SetCustomMTData(long unit_longptr, string dataID, string dataValue, string dataSource = null)
    {
        var storedMTDataDict = Main.Instance.storedMTDataDict;

        if (storedMTDataDict.TryGetValue(unit_longptr, out System.Collections.Generic.List<MTModData> foundDataList))
        {
            MTModData foundData = null;
            if (dataSource != null) foundData = foundDataList.Find(x => x.dataID == dataID && x.dataSource == dataSource);
            else foundData = foundDataList.Find(x => x.dataID == dataID);

            if (foundData == null) foundDataList.Add(new MTModData(dataID, dataValue, dataSource));
            else foundData.dataValue = dataValue;
        }
        else storedMTDataDict.Add(unit_longptr, [new MTModData(dataID, dataValue, dataSource)]);
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

        public static ModularSA testModular = new ModularSA();
        public static string[] StringArrayGenerator(string circle) { return circle.Split('|'); }

        public static System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE> overrideBuffPanicDict = [];
    }

    public class ConsequenceTest : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            
        }
    }

    public class ConsequenceTest1 : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            CoinSlotListUI coinList = UnityEngine.Object.FindObjectOfType<CoinSlotListUI>();
            foreach(CoinSlotUI coinSlotUI in coinList._slots)
            {
                coinSlotUI.UpdateCoinColor(new Color(0f, 0.5f, 0.5f));
            }
        }
    }
    
    public class AcquirerTest : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            BattleUnitModel target = modular.GetTargetModel(circles[0]);
            int skillId = modular.GetNumFromParamString(circles[1]);
            return target.DidActionPrevTurn(skillId) ? 1 : 0;
        }
    }

    public class LuaFunctionGetRandomBuff : IModularLuaFunction
    {
        public ValueTask<int> ExecuteLuaFunction(ModularSA modular, LuaFunctionExecutionContext context, System.Span<LuaValue> buffer, CancellationToken ct)
        {
            BattleUnitModel target = modular.GetTargetModel(context.GetArgument(0).Read<string>());
            if (target == null) return ValueTask.FromResult(0);
            Enum.TryParse<BUF_TYPE>(context.GetArgument(1).Read<string>(), true, out BUF_TYPE bufType);
            Il2CppSystem.Nullable<bool> canBeDespelled = context.ArgumentCount > 2 ? new Il2CppSystem.Nullable<bool>(context.GetArgument(2).Read<bool>()) : null; 
            buffer[0] = target.GetRandomBuff(bufType, canBeDespelled)._name.ToString();
            return ValueTask.FromResult(1);
        }
    }

    public void AddTiming(Harmony harmony, Type patch, string[] timingList, int[] actEvents)
    {
        try
        {
            if (harmony != null && patch != null) harmony.PatchAll(patch);
            if (timingList != null && actEvents != null)
            {
                for (int i = 0; i < timingList.Length; i++) MainClass.timingDict.Add(timingList[i], actEvents[i]);
            }
        }
        catch (System.Exception ex) { Main.Logger.LogError($"Error on timing with names = {string.Join('/', timingList)}\n{ex}"); }
    }
    
    

    public static Main Instance;

    public static ManualLogSource Logger;

    public override void Load()
    {
        Instance = this;
        Logger = Log;

        Harmony harmony = new Harmony(NAME);
        AddTiming(harmony, typeof(RightAfterGetAnyBuff), null, null);
        AddTiming(harmony, typeof(GetSkillIdsPatch), null, null);

        AddTiming(harmony, typeof(RecoverSwitchPanic), ["OnPanic", "OnOtherPanic", "OnLowMorale", "OnOtherLowMorale"], [90901, 90902, 90903, 90904]);
        AddTiming(null, null, ["OnRecoverBreak", "OnOtherRecoverBreak"], [90905, 90906]);
        AddTiming(null, null, ["OnTakePiledVibration", "OnOtherTakePiledVibration", "OnTakeSwitchingVibration", "OnOtherTakeSwitchingVibration"], [90907, 90908, 90909, 90910]);
        AddTiming(harmony, typeof(LoseAnyBuff), ["OnLoseBuff", "OnBeforeLoseBuff"], [90911, 90912]);
        AddTiming(harmony, typeof(ChangeSP), ["OnChangeSP", "OnOtherChangeSP", "OnTakeSPDamage", "OnOtherTakeSPDamage"], [90913, 90914, 90915, 90916]);
        AddTiming(harmony, typeof(OnUnOpposed), ["OnUnOpposed"], [90917]);

        try
        {
            harmony.PatchAll(typeof(Patch_DefenseChange));
            harmony.PatchAll(typeof(Modular_SetupModular));
            harmony.PatchAll(typeof(Modular_EnactConsequence));
            harmony.PatchAll(typeof(BuffModel_OverwritePanic));
            harmony.PatchAll(typeof(EquipDefenseOperation));
            // harmony.PatchAll(typeof(CoinSlotUI_UpdateCoinColor));
            // harmony.PatchAll(typeof(StyxPatch));
            // harmony.PatchAll(typeof(SystemAbilityDetail_Patch));
            // harmony.PatchAll(typeof(RightAfterGiveBuffBySkill));
            
            // harmony.PatchAll(typeof(RightAfterGetAnyBuff));
            // MainClass.timingDict.Add("OnGainBuff", 1337);
            // MainClass.timingDict.Add("OnInflictBuff", 1733);
        }
        catch (System.Exception ex) { Main.Logger.LogError("Error when loading patches: " + ex); }

        try
        {
            MainClass.luaFunctionDict["jsontolua"] = new MTCustomScripts.LuaFunctions.LuaFunctionJsonDecoder();
            MainClass.luaFunctionDict["listdirectories"] = new MTCustomScripts.LuaFunctions.LuaFunctionListDirectories();
            MainClass.luaFunctionDict["listbuffs"] = new MTCustomScripts.LuaFunctions.LuaFunctionListBuffs();
            MainClass.luaFunctionDict["setgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionSetGlobalVarMT();
            MainClass.luaFunctionDict["getgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionGetGlobalVarMT();
            MainClass.luaFunctionDict["clearallgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionClearGlobalVarMT();
            MainClass.luaFunctionDict["gbkeyword"] = new MTCustomScripts.LuaFunctions.LuaFunctionGainBuffKeyword();
            MainClass.luaFunctionDict["getcurrentmapid"] = new MTCustomScripts.LuaFunctions.GetCurrentMapID();
            MainClass.luaFunctionDict["listrelatedkeywords"] = new MTCustomScripts.LuaFunctions.LuaFunctionListRelatedKeywords();
            MainClass.luaFunctionDict["getappearanceid"] = new MTCustomScripts.LuaFunctions.LuaFunctionGetAppearanceID();
            MainClass.luaFunctionDict["listbreakvalues"] = new MTCustomScripts.LuaFunctions.LuaFunctionListBreakSectionValue();
            MainClass.luaFunctionDict["listegoskillids"] = new MTCustomScripts.LuaFunctions.LuaFunctionListEgoSkillIDs();
            // MainClass.luaFunctionDict["getrandombuff"] = new LuaFunctionGetRandomBuff(); //Object reference not set to an instance of an object
            // MainClass.luaFunctionDict["listskilltargets"] = new MTCustomScripts.LuaFunctions.LuaFunctionListSkillTargets();
        }
        catch (System.Exception ex) { Main.Logger.LogError("Error when loading LUA functions: " + ex); }

        try
        {
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
            MainClass.acquirerDict["comparer"] = new MTCustomScripts.Acquirers.AcquirerGetComparerResult();
            MainClass.acquirerDict["hasbuffkeyword"] = new MTCustomScripts.Acquirers.AcquirerHasBuffKeyword();
            MainClass.acquirerDict["getmapdata"] = new MTCustomScripts.Acquirers.AcquirerGetMapData();
            MainClass.acquirerDict["getfinal"] = new MTCustomScripts.Acquirers.AcquirerGetFinalPower();
            MainClass.acquirerDict["getpaniclevel"] = new MTCustomScripts.Acquirers.AcquirerGetPanicLevel();
            MainClass.acquirerDict["getcoinprobadder"] = new MTCustomScripts.Acquirers.AcquirerGetCoinProbAdder();
            MainClass.acquirerDict["getskilldata"] = new MTCustomScripts.Acquirers.AcquirerGetSkillData();
            MainClass.acquirerDict["hasskill"] = new MTCustomScripts.Acquirers.AcquirerHasSkill();
            MainClass.acquirerDict["diduseskillprevturn"] = new MTCustomScripts.Acquirers.AcquirerDidUsedSkillPrevTurn();
            MainClass.acquirerDict["getbuffstackgainedthisturn"] = new MTCustomScripts.Acquirers.AcquirerGetBuffStackGainedThisTurn();
            MainClass.acquirerDict["getbreaklevel"] = new MTCustomScripts.Acquirers.AcquirerGetCurrentBrokenLevel();
            MainClass.acquirerDict["isactionable"] = new MTCustomScripts.Acquirers.AcquirerIsActionable();
            MainClass.acquirerDict["getopposkillid"] = new MTCustomScripts.Acquirers.AcquirerGetOppoSkillId();
            MainClass.acquirerDict["getabilitymoduleproperty"] = new MTCustomScripts.Acquirers.AcquirerGetAbilityModuleProperty();
            MainClass.acquirerDict["getcurrentpower"] = new MTCustomScripts.Acquirers.AcquirerGetCurrentPower();
            MainClass.acquirerDict["getdefaultmaxhp"] = new MTCustomScripts.Acquirers.AcquirerGetDefaultMaxHp();
            MainClass.acquirerDict["gethpincrement"] = new MTCustomScripts.Acquirers.AcquirerGetHpIncrementByLevel();
            MainClass.acquirerDict["getchangespvalue"] = new MTCustomScripts.Acquirers.AcquirerGetChangedSPValue();
            MainClass.acquirerDict["getmtdata"] = new MTCustomScripts.Acquirers.AcquirerGetMTData();
            MainClass.acquirerDict["getuptielevel"] = new MTCustomScripts.Acquirers.AcquirerGetUptieLevel();
        } catch (System.Exception ex) { Main.Logger.LogError("Error when loading Acquirers: " + ex); }

        try
        {
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
            MainClass.consequenceDict["addskill"] = new MTCustomScripts.Consequences.ConsequenceAddSkill();
            MainClass.consequenceDict["removeskill"] = new MTCustomScripts.Consequences.ConsequenceRemoveSkill();
            MainClass.consequenceDict["clearallunitscript"] = new MTCustomScripts.Consequences.ConsequenceClearUnitScript();
            MainClass.consequenceDict["changehp"] = new MTCustomScripts.Consequences.ConsequenceChangeHp();
            MainClass.consequenceDict["changesp"] = new MTCustomScripts.Consequences.ConsequenceChangeSp();
            MainClass.consequenceDict["instantdeath"] = new MTCustomScripts.Consequences.ConsequenceInstantDeath();
            // MainClass.consequenceDict["changetakebuffdmg"] = new MTCustomScripts.Consequences.ConsequenceChangeTakeBuffDamage(); //doesnt work
            MainClass.consequenceDict["lbreak"] = new MTCustomScripts.Consequences.ConsequenceLBreak();
            MainClass.consequenceDict["addcoin"] = new MTCustomScripts.Consequences.ConsequenceAddCoin();
            MainClass.consequenceDict["removecoin"] = new MTCustomScripts.Consequences.ConsequenceRemoveCoin();
            MainClass.consequenceDict["changecolor"] = new MTCustomScripts.Consequences.ConsequenceChangeCoinType();
            MainClass.consequenceDict["changeabilitymoduleproperty"] = new MTCustomScripts.Consequences.ConsequenceChangeAbilityModuleProperty();
            MainClass.consequenceDict["clearskillabilities"] = new MTCustomScripts.Consequences.ConsequenceClearSkillAbilities();
            MainClass.consequenceDict["clearcoinabilities"] = new MTCustomScripts.Consequences.ConsequenceClearCoinAbilities();
            MainClass.consequenceDict["addskillability"] = new MTCustomScripts.Consequences.ConsequenceAddSkillAbility();
            MainClass.consequenceDict["removealltargetexceptmaintarget"] = new MTCustomScripts.Consequences.ConsequenceRemoveAllTargetExceptMainTarget();
            MainClass.consequenceDict["modifysubtarget"] = new MTCustomScripts.Consequences.ConsequenceModifySubTargetList();
            MainClass.consequenceDict["setmaintarget"] = new MTCustomScripts.Consequences.ConsequenceSetMainTarget();
            MainClass.consequenceDict["addcoinability"] = new MTCustomScripts.Consequences.ConsequenceAddCoinAbility();
            MainClass.consequenceDict["setspusage"] = new MTCustomScripts.Consequences.ConsequenceSetSpUsage();
            // MainClass.consequenceDict["addego"] = new MTCustomScripts.Consequences.ConsequenceAddEgo();
            MainClass.consequenceDict["disableidle"] = new MTCustomScripts.Consequences.ConsequenceDisableIdle();
            MainClass.consequenceDict["setskillamount"] = new MTCustomScripts.Consequences.ConsequenceSetSkillAmount();
            MainClass.consequenceDict["setlevel"] = new MTCustomScripts.Consequences.ConsequenceSetLevel();
            MainClass.consequenceDict["setmaxhp"] = new MTCustomScripts.Consequences.ConsequenceSetMaxHp();
            MainClass.consequenceDict["addcoinabilitybasicbuff"] = new MTCustomScripts.Consequences.ConsequenceAddCoinAbilityBasicBuff();
            MainClass.consequenceDict["setmtdata"] = new MTCustomScripts.Consequences.ConsequenceSetMTData();
        } catch (System.Exception ex) { Main.Logger.LogError("Error when loading Consequences: " + ex); }

        try
        {
            MainClass.consequenceDict["test"] = new ConsequenceTest();
            MainClass.consequenceDict["test1"] = new ConsequenceTest1();
            MainClass.acquirerDict["test2"] = new AcquirerTest();
            // MainClass.consequenceDict["test"] = new ConsequenceTest();
            // MainClass.consequenceDict["testthree"] = new ConsequenceTest3();
            // MainClass.consequenceDict["reload"] = new ConsequenceReload();
        } catch (System.Exception ex) { Main.Logger.LogError("Error when loading Test Consequences/Acquirers: " + ex); }
    }
}
