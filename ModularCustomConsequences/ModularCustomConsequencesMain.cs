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
using DiscordRPC;

namespace MTCustomScripts;

[BepInPlugin(GUID, NAME, VERSION)]
// this is required to make your plugin run AFTER Modular has been loaded.
[BepInDependency("GlitchGames.ModularSkillScripts")]

public class Main : BasePlugin
{
    // Edit the below to your own plugin name, version, etc.
    public const string NAME = "MTCustomScripts";
    public const string VERSION = "6.48.16";
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

        public static ModularSA testModular = new ModularSA();

        public static string[] StringArrayGenerator(string circle) { return circle.Split('|'); }

        public static System.Collections.Generic.Dictionary<string, string> stringDict = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE> overrideBuffPanicDict = new System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE>();
    }

    public class testStuffStorage
    {
        public static System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE> overrideBuffPanicDict = new System.Collections.Generic.Dictionary<BuffModel, PANIC_TYPE>();

        public static void HijackDiscordStatus(string msg)
        {
            LetheMain.discordRpcClient.SetPresence(new RichPresence
            {
                Type = ActivityType.Watching,
                Details = msg,
                Timestamps = Timestamps.Now,
                State = "Hijacked by Styx",
                Assets = new Assets
                {
                    LargeImageKey = "lethe_icon",
                    LargeImageUrl = "https://lethelc.site/"
                }
            });
        }
    }

    public class AcquirerGetPanicLevel : IModularAcquirer
    {
        public int ExecuteAcquirer(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: target (single)
             */
            BattleUnitModel target = modular.GetTargetModel(circles[0]);
            if (target == null) return -1;

            if (!target.IsLowMorale() && !target.IsPanic()) return 0;
            else if (target.IsLowMorale() && !target.IsPanic()) return 1;
            else if (target.IsPanic()) return 2;
            else
            {
                //Add Log here with all above bools (IsLowMorale() and IsPanic())
                Logger.LogMessage($"IsLowMorale: {target.IsLowMorale()} | IsPanic: {target.IsPanic()}");
                return -1;
            }
        }
    }

    public class ConsequenceChangePanicType : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            /*
             * var_1: target (multi)
             * var_2: panic
             * var_3: default/cached/buff
             * opt_4: buff/current
             */

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
            if (unitList == null || unitList.Count == 0) return;
            if (!Il2CppSystem.Enum.TryParse<PANIC_TYPE>(circles[1], true, out PANIC_TYPE panic)) return;

            BUFF_UNIQUE_KEYWORD buffKeyword = BUFF_UNIQUE_KEYWORD.None;
            bool SelectBuff = (circles[2] != null && circles[2] == "buff") && (circles[3] != null && (circles[3] == "current") || Il2CppSystem.Enum.TryParse<BUFF_UNIQUE_KEYWORD>(circles[3], true, out buffKeyword));

            foreach (BattleUnitModel unit in unitList)
            {
                if (!SelectBuff)
                {
                    if (circles[2] == "default") unit._defaultPanicType = panic;
                    else if (circles[2] == "cached")
                    {
                        unit._cachedPanicType = panic;
                        if (Il2CppSystem.Enum.TryParse<BUFF_UNIQUE_KEYWORD>(Singleton<StaticDataManager>.Instance.GetBuffData(circles[2]).id, true, out var panicUniqueType))
                            unit._cachedPanicTypeBuff = panicUniqueType;
                    }
                    else return;

                    continue;
                }

                if (SelectBuff)
                {
                    BuffModel selectedBuff = null;

                    if (circles[3] != "current")
                    {
                        BUFF_UNIQUE_KEYWORD var1Keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
                        if (unit._buffDetail.HasBuff(var1Keyword) == true) selectedBuff = unit._buffDetail.FindActivatedBuff(var1Keyword, true);
                    }
                    if (selectedBuff == null) selectedBuff = modular.modsa_buffModel;
                    if (selectedBuff == null) continue;

                    testStuffStorage.overrideBuffPanicDict[selectedBuff] = panic;
                }
            }
        }
    }

    public class ConsequenceChangePanicLevel : IModularConsequence
{
    public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
    {
        /*
         * var_1: Multi-Target
         * var_2: Panic-Level
         */

        Il2CppSystem.Collections.Generic.List<BattleUnitModel> unitList = modular.GetTargetModelList(circles[0]);
        if (unitList == null || unitList.Count <= 0) return;
        if (!int.TryParse(circles[1], out int panicLevel)) return;
        if (panicLevel == 0) return;

        foreach (BattleUnitModel unit in unitList)
        {
            if (panicLevel == 1) unit.OnLowMorale(modular.battleTiming);
            else if (panicLevel == 2) unit.OnPanic(modular.battleTiming);

            unit.OnPanicOrLowMorale((PANIC_LEVEL)panicLevel, modular.battleTiming);
        }
    }
}

    public class ConsequenceTest : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            // PatternScript_1341 patternScript_1327 = new PatternScript_1341();
            // foreach(var appr in patternScript_1327._phaseAppearances.ToArray())
            // {
            //     Logger.LogMessage($"Appearance listing: {appr}");
            // }
            // CoinAbility newCA = new CoinAbility_OverwriteToSuperCoin();
            // COIN_COLOR_TYPE cct = COIN_COLOR_TYPE.GOLD;
            // int grade = 1;
            // int cindex = modular.GetNumFromParamString(circles[0]);
            // switch (circles[1])
            // {
            //     default:
            //         cct = COIN_COLOR_TYPE.GOLD;
            //         grade = 1;
            //         break;
            //     case "green":
            //         cct = COIN_COLOR_TYPE.GREEN;
            //         grade = 99;
            //         break;
            //     case "purple":
            //         cct = COIN_COLOR_TYPE.PURPLE;
            //         grade = 2;
            //         break;
            //     case "grey":
            //         cct = COIN_COLOR_TYPE.GREY;
            //         grade = 2;
            //         break;
            // }
            // newCA.OverwriteCoinColor(out cct);
            // newCA.OverwriteCoinGrade(out grade);

            // Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
            // foreach(BattleUnitModel target in targetList)
            // {
            //     foreach(CoinModel coinModel in modular.modsa_skillModel.CoinList)
            //     {
            //         coinModel._coinAbilityList.Add(newCA);
            //     }
            // }
            // if (string.Equals(circles[0], "all", StringComparison.OrdinalIgnoreCase))
            // {
            //     foreach (CoinModel coin in modular.modsa_skillModel.CoinList)
            //     {
            //         Singleton<SkillAbility_OverwriteToSuperCoinViaBuffCheck>.Instance.AddScriptToCoin(coin);
            //     }
            // }
            // else
            // {
            //     foreach (string circle in circles)
            //     {
            //         int idx = modular.GetNumFromParamString(circle);
            //         if (idx < 0)
            //         {
            //             Singleton<SkillAbility_OverwriteToSuperCoinViaBuffCheck>.Instance.AddScriptToCoin(
            //                 modular.modsa_skillModel.GetCoin(modular.modsa_coinModel.GetOriginCoinIndex()));
            //             continue;
            //         }

            //         idx = Math.Min(idx, modular.modsa_skillModel.CoinList.Count - 1);
            //         Singleton<SkillAbility_OverwriteToSuperCoinViaBuffCheck>.Instance
            //             .AddScriptToCoin(modular.modsa_skillModel.GetCoin(idx));
            //     }
            // }

            Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
            if (targetList.Count < 1) return;
            BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
            if (keyword.ToString() != circles[1]) keyword = BUFF_UNIQUE_KEYWORD.None;
            int loseStack = modular.GetNumFromParamString(circles[2]);
            int loseTurn = modular.GetNumFromParamString(circles[3]);
            // Il2CppSystem.Nullable<int> limit = null;
            foreach(BattleUnitModel target in targetList)
            {
                target.ForceToActivateBuffEffect(keyword, modular.modsa_unitModel, loseStack, loseTurn, null, modular.battleTiming);
            }
        }
    }

    public class ConsequenceTest1 : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
            int newMp = modular.GetNumFromParamString(circles[1]);
            foreach(BattleUnitModel target in targetList)
            {
                target.ChangeMp(newMp);
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
    public class ConsequenceChangeTakeBuffDamage : IModularConsequence
    {
        public void ExecuteConsequence(ModularSA modular, string section, string circledSection, string[] circles)
        {
            Il2CppSystem.Collections.Generic.List<BattleUnitModel> targetList = modular.GetTargetModelList(circles[0]);
            if (targetList.Count < 1) return;
            BUFF_UNIQUE_KEYWORD keyword = CustomBuffs.ParseBuffUniqueKeyword(circles[1]);
            if (keyword.ToString() != circles[1]) keyword = BUFF_UNIQUE_KEYWORD.None;
            int resultDmg = modular.GetNumFromParamString(circles[2]);
            DAMAGE_SOURCE_TYPE source = DAMAGE_SOURCE_TYPE.NONE;
            if (circles[3] != "All") Enum.TryParse<DAMAGE_SOURCE_TYPE>(circles[3], true, out source);
            foreach(BattleUnitModel target in targetList)
            {
                if (circles[3] == "All")
                {
                    foreach (DAMAGE_SOURCE_TYPE srcType in Enum.GetValues<DAMAGE_SOURCE_TYPE>())
                    {
                        if (srcType != DAMAGE_SOURCE_TYPE.NONE) target.ChangeTakeDamage(null, null, resultDmg, srcType, keyword, modular.battleTiming);
                    }
                    continue;
                }
                target.ChangeTakeDamage(null, null, resultDmg, source, keyword, modular.battleTiming);
            }
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
        harmony.PatchAll(typeof(EquipDefenseOperation));
        harmony.PatchAll(typeof(BuffModelPatch));

        // MainClass.timingDict.Add("OnGainBuff", 1337);
        // MainClass.timingDict.Add("OnInflictBuff", 1733);
        MainClass.timingDict.Add("OnPanic", 90901);
        MainClass.timingDict.Add("OnOtherPanic", 909012);
        MainClass.timingDict.Add("OnLowMorale", 90903);
        MainClass.timingDict.Add("OnOtherLowMorale", 90904);
        MainClass.timingDict.Add("OnRecoverBreak", 90905);
        MainClass.timingDict.Add("OnOtherRecoverBreak", 90906);
        MainClass.timingDict.Add("OnLoseBuff", 90907);
        MainClass.timingDict.Add("OnBeforeLoseBuff", 90908);


        MainClass.luaFunctionDict["jsontolua"] = new MTCustomScripts.LuaFunctions.LuaFunctionJsonDecoder();
        MainClass.luaFunctionDict["listdirectories"] = new MTCustomScripts.LuaFunctions.LuaFunctionListDirectories();
        MainClass.luaFunctionDict["listbuffs"] = new MTCustomScripts.LuaFunctions.LuaFunctionListBuffs();
        MainClass.luaFunctionDict["setgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionSetGlobalVarMT();
        MainClass.luaFunctionDict["getgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionGetGlobalVarMT();
        MainClass.luaFunctionDict["clearallgdata"] = new MTCustomScripts.LuaFunctions.LuaFunctionClearGlobalVarMT();
        MainClass.luaFunctionDict["gbkeyword"] = new MTCustomScripts.LuaFunctions.LuaFunctionGainBuffKeyword();
        MainClass.luaFunctionDict["getcurrentmapid"] = new MTCustomScripts.LuaFunctions.GetCurrentMapID();
        MainClass.luaFunctionDict["listrelatedkeywords"] = new MTCustomScripts.LuaFunctions.LuaFunctionListRelatedKeywords();
        MainClass.luaFunctionDict["getappearanceid"] = new MTCustomScripts.LuaFunctions.LuaFunctionGetAppearanceID(); //new
        MainClass.luaFunctionDict["listbreakvalues"] = new MTCustomScripts.LuaFunctions.LuaFunctionListBreakSectionValue(); //new
        // MainClass.luaFunctionDict["getrandombuff"] = new LuaFunctionGetRandomBuff(); //Object reference not set to an instance of an object

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
        MainClass.acquirerDict["getcoinprobadder"] = new MTCustomScripts.Acquirers.AcquirerGetCoinProbAdder();
        MainClass.acquirerDict["getskilldata"] = new MTCustomScripts.Acquirers.AcquirerGetSkillData();
        MainClass.acquirerDict["hasskill"] = new MTCustomScripts.Acquirers.AcquirerHasSkill();
        MainClass.acquirerDict["didusedskillprevturn"] = new MTCustomScripts.Acquirers.AcquirerDidUsedSkillPrevTurn();
        MainClass.acquirerDict["getbuffstackgainedthisturn"] = new MTCustomScripts.Acquirers.AcquirerGetBuffStackGainedThisTurn();
        MainClass.acquirerDict["getbreaklevel"] = new MTCustomScripts.Acquirers.AcquirerGetCurrentBrokenLevel();
        MainClass.acquirerDict["isactionable"] = new MTCustomScripts.Acquirers.AcquirerIsActionable();
        MainClass.acquirerDict["getopposkillid"] = new MTCustomScripts.Acquirers.AcquirerGetOppoSkillId();

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
        // MainClass.consequenceDict["addcoin"] = new MTCustomScripts.Consequences.ConsequenceAddCoin();
        MainClass.consequenceDict["clearallunitscript"] = new MTCustomScripts.Consequences.ConsequenceClearUnitScript();
        MainClass.consequenceDict["changehp"] = new MTCustomScripts.Consequences.ConsequenceChangeHp();
        MainClass.consequenceDict["changesp"] = new MTCustomScripts.Consequences.ConsequenceChangeSp();
        MainClass.consequenceDict["instantdeath"] = new MTCustomScripts.Consequences.ConsequenceInstantDeath();
        // MainClass.consequenceDict["changetakebuffdmg"] = new MTCustomScripts.Consequences.ConsequenceChangeTakeBuffDamage(); //doesnt work
        MainClass.consequenceDict["lbreak"] = new MTCustomScripts.Consequences.ConsequenceLBreak();

        MainClass.consequenceDict["test"] = new ConsequenceTest();
        MainClass.consequenceDict["test1"] = new ConsequenceTest1();
        MainClass.acquirerDict["test2"] = new AcquirerTest();
        MainClass.consequenceDict["addskill"] = new MTCustomScripts.Consequences.ConsequenceAddSkill();
        MainClass.consequenceDict["removeskill"] = new MTCustomScripts.Consequences.ConsequenceRemoveSkill();
        //MainClass.consequenceDict["addcoin"] = new MTCustomScripts.Consequences.ConsequenceAddCoin();
        // MainClass.consequenceDict["test"] = new ConsequenceTest();
        // MainClass.consequenceDict["testthree"] = new ConsequenceTest3();
        // MainClass.consequenceDict["reload"] = new ConsequenceReload();
    }
}
