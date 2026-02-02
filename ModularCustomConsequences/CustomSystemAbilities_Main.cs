using ModularSkillScripts;
using Newtonsoft.Json;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.IO;

namespace MTCustomScripts
{
    public static class CustomSystemAbilities_Main
    {
        public static bool TryAddCustomSystemAbility(CustomSystemAbility newSystemAbility, out string logging)
        {
            int newId = newSystemAbility.GetCustomIdentifier();
            if (newId == 5000 || Il2CppSystem.Enum.IsDefined(Il2CppSystem.Type.GetType(nameof(SYSTEM_ABILITY_KEYWORD), true), newId))
            {
                logging = $"System Ability with ID={newId} and Name={newSystemAbility.GetCustomNameId()} is template or already taken in Vanilla";
                return false;
            }

            if (customSystemAbilityDict.ContainsValue(newSystemAbility))
            {
                logging = $"System Ability with ID={newId} and Name={newSystemAbility.GetCustomNameId()} is already in the dictionnary";
                return false;
            }

            customSystemAbilityDict[newId] = newSystemAbility;
            logging = $"Successfull addition of System Ability with ID={newId} and Name={newSystemAbility.GetCustomNameId()}";
            return true;
        }

        public static void PrintDictionary()
        {
            Main.Logger.LogInfo("Started the print of customSystemAbilityDict");
            foreach (var kvp in customSystemAbilityDict)
            {
                Main.Logger.LogInfo($"Key: {kvp.Key}, Value: {kvp.Value.GetCustomNameId()}");
            }
            Main.Logger.LogInfo("Ended the print of customSystemAbilityDict");
        }


        public static bool CheckOverwriteAbility(SYSTEM_ABILITY_KEYWORD systemKeyword, out CustomSystemAbility __result)
        {
            int newKeywordValue = (int)systemKeyword;

            if (!Il2CppSystem.Enum.IsDefined(SystemAbilityKeywordEnumType, newKeywordValue) && CustomSystemAbilities_Main.customSystemAbilityDict.TryGetValue(newKeywordValue, out CustomSystemAbility customSystemAbility))
            {
                Main.Logger.LogInfo($"Succesfully recovered copy of customAbility with ID={customSystemAbility.GetCustomIdentifier()} and name={customSystemAbility.GetCustomNameId()}");
                __result = customSystemAbility.Copy();
                return false;
            }

            __result = null;
            return true;
        }


        public static bool HasSystemAbility(this SystemAbilityDetail detail, SYSTEM_ABILITY_KEYWORD newKeyword, out SystemAbility systemAbility)
        {
            if (CustomSystemAbilities_Main.CheckOverwriteAbility(newKeyword, out CustomSystemAbility customAbility))
            {
                systemAbility = detail._activatedAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == customAbility.GetCustomIdentifier());
                if (systemAbility == null) systemAbility = detail._nextTurnAbilityList.ToSystem().Find(x => (x as CustomSystemAbility).GetCustomIdentifier() == customAbility.GetCustomIdentifier());
            }
            else
            {
                systemAbility = detail._activatedAbilityList.ToSystem().Find(x => x.UniqueKeyword == newKeyword);
                if (systemAbility == null) systemAbility = detail._nextTurnAbilityList.ToSystem().Find(x => x.UniqueKeyword == newKeyword);
            }
            if (systemAbility != null) return true;
            else return false;
        }

        public static Il2CppSystem.Type SystemAbilityKeywordEnumType = Il2CppSystem.Type.GetType(nameof(SYSTEM_ABILITY_KEYWORD), true);
        public static System.Collections.Generic.Dictionary<int, CustomSystemAbility> customSystemAbilityDict = new System.Collections.Generic.Dictionary<int, CustomSystemAbility>();
    }


    public class CustomSystemAbility : BattleSystemAbility
    {
        public CustomSystemAbility()
        {

        }

        public virtual int GetCustomIdentifier()
        {
            return 5000;
        }

        public virtual string GetCustomNameId()
        {
            return "CustomAbilityTemplate";
        }
    }




    [System.Serializable]
    public class ModularSystemAbilityStaticDataList
    {
        public static void Initialize(ModularSystemAbilityStaticDataList instance)
        {
            ModularSystemAbilityStaticDataList._instance = instance;
            instance.modularAbilityStaticDataList.Clear();

            string modsBasePath = Path.Combine(BepInEx.Paths.PluginPath, "Lethe", "mods");
            System.Collections.Generic.List<string> jsonFiles = new System.Collections.Generic.List<string>();

            try
            {
                if (!Directory.Exists(modsBasePath)) return;
                string[] modDirectories = Directory.GetDirectories(modsBasePath);



                foreach (string modDir in modDirectories)
                {
                    string abilityFolder = Path.Combine(modDir, "custom_system_ability");
                    if (Directory.Exists(abilityFolder))
                    {
                        string[] jsonFilesInFolder = Directory.GetFiles(abilityFolder, "*.json", SearchOption.TopDirectoryOnly);
                        jsonFiles.AddRange(jsonFilesInFolder);
                    }
                }



                foreach (string filePath in jsonFiles)
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(filePath);
                        ModularSystemAbilityStaticDataList parsedList = JsonConvert.DeserializeObject<ModularSystemAbilityStaticDataList>(jsonContent);

                        if (parsedList != null && parsedList.modularAbilityStaticDataList != null)
                        {
                            foreach (ModularSystemAbilityStaticData staticData in parsedList.modularAbilityStaticDataList)
                            {
                                string modName = Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(filePath)));
                                staticData.ModFile = modName;
                                instance.modularAbilityStaticDataList.Add(staticData);
                                CustomSystemAbilities_Main.TryAddCustomSystemAbility(new ModularSystemAbility(staticData), out string addLog);
                                Main.Logger.LogInfo($"Trying to create custom modular, result: {addLog}");
                            }

                            Main.Logger.LogInfo($"Loaded {parsedList.modularAbilityStaticDataList.Count} modular system abilities from {Path.GetFileName(filePath)}");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Main.Logger.LogError($"Failed to load file {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }
                Main.Logger.LogInfo($"Total custom modular system abilities loaded: {instance.modularAbilityStaticDataList.Count}");
            }
            catch (System.Exception ex) { Main.Logger.LogError($"Fatal error loading modular system abilities: {ex.Message}"); }
        }


        public ModularSystemAbilityStaticData GetData(int id)
        {
            ModularSystemAbilityStaticData data = Instance.modularAbilityStaticDataList.Find(x => x.Id == id);
            return (data == null) ? null : data;
        }
        public ModularSystemAbilityStaticData GetData(string name)
        {
            ModularSystemAbilityStaticData data = Instance.modularAbilityStaticDataList.Find(x => x.Name == name);
            return (data == null) ? null : data;
        }
        public System.Collections.Generic.List<ModularSystemAbilityStaticData> GetByMod(string mod)
        {
            System.Collections.Generic.List<ModularSystemAbilityStaticData> data = Instance.modularAbilityStaticDataList.FindAll(x => x.ModFile == mod);
            return (data == null || data.Count <= 0) ? new System.Collections.Generic.List<ModularSystemAbilityStaticData>() : data;
        }


        [JsonProperty]
        public System.Collections.Generic.List<ModularSystemAbilityStaticData> modularAbilityStaticDataList;



        public static ModularSystemAbilityStaticDataList Instance
        {
            get
            {
                ModularSystemAbilityStaticDataList instance = ModularSystemAbilityStaticDataList._instance;
                if (instance == null) throw new System.Exception("Not initialized");
                return instance;
            }
        }
        private static ModularSystemAbilityStaticDataList _instance;
    }




    public class ModularSystemAbility : CustomSystemAbility
    {
        public ModularSystemAbility(ModularSystemAbilityStaticData classInfo)
        {
            customIdentifier = classInfo.Id;
            customName = classInfo.Name;
            originalClassInfo = classInfo;
            currentClassInfo = classInfo;

            foreach (string modular in classInfo.modularList)
            {
                string correctedModular = modular.Remove(0, 8);
                ModularSA finalModular = new ModularSA();
                finalModular.abilityMode = 2;
                finalModular.SetupModular(correctedModular);

                string timing = (correctedModular.StartsWith("TIMING:")) ? modular.Split('/', 2, System.StringSplitOptions.RemoveEmptyEntries)[0].Substring(7) : modular.Split('/', 2, System.StringSplitOptions.RemoveEmptyEntries)[0];
                if (modularDict.ContainsKey(timing)) modularDict[timing].Add(finalModular);
                else modularDict[timing] = new System.Collections.Generic.List<ModularSA> { finalModular };
            }
        }

        public override int GetCustomIdentifier()
        {
            return customIdentifier;
        }

        public override string GetCustomNameId()
        {
            return customName;
        }

        private void ClearAllEditedTemporary()
        {
            if (editedParamList.Count <= 0) return;
            Main.Logger.LogInfo($"Starting the cleaning of modular System Ability with ID={this.GetCustomIdentifier()} and Name={this.GetCustomNameId()}");
            foreach (string key in editedParamList.Keys)
            {
                try
                {
                    Main.Logger.LogInfo($"Cleaning property with key={key}");
                    var editedParam = editedParamList[key];
                    if (editedParam is ModularSystemAbilityStaticData_BundledParam bundledParam)
                    {
                        bundledParam.temporaryData = 0;
                        bundledParam.temporaryBannedBuffKeywordList.Clear();
                        bundledParam.temporaryBannedSourceTypeList.Clear();
                    }
                    else if (editedParam is System.Collections.Generic.Dictionary<System.Enum, int> systEnumDict) systEnumDict.Clear();
                    else if (editedParam is System.Collections.Generic.Dictionary<Il2CppSystem.Enum, int> cppEnumDict) cppEnumDict.Clear();
                    else Main.Logger.LogWarning($"Unknown type for key={key} and type={editedParam?.GetType()}");
                }
                catch (System.Exception ex) { Main.Logger.LogError($"Unknown error expected for key={key} and type={editedParamList[key]?.GetType()} with error {ex}"); }
            }
            Main.Logger.LogInfo($"Finished the cleaning of modular System Ability with ID={this.GetCustomIdentifier()} and Name={this.GetCustomNameId()}");
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnRoundStart_After_Event(BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "RoundStart";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.Enact(this.Owner, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnBattleStart(BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "StartBattle";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.Enact(this.Owner, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnStartTurn_BeforeLog(BattleActionModel action, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "WhenUse";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnStartBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnStartBehaviour";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnStartDuel(BattleActionModel ownerAction, BattleActionModel opponentAction)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "StartDuel";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(opponentAction.Model);
                modbsa.Enact(ownerAction.Model, ownerAction.Skill, ownerAction, opponentAction, actevent, BATTLE_EVENT_TIMING.ON_START_DUEL);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnStartCoin(BattleActionModel action, CoinModel coin, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnCoinToss";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.modsa_coinModel = coin;
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnWinDuel(BattleActionModel selfAction, BattleActionModel oppoAction, int parryingCount, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "WinDuel";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(oppoAction.Model);
                modbsa.Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ON_WIN_DUEL);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnLoseDuel(BattleActionModel selfAction, BattleActionModel oppoAction)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "DefeatDuel";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(oppoAction.Model);
                modbsa.Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ON_LOSE_DUEL);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnWinParrying(BattleActionModel selfAction, BattleActionModel oppoAction)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "WinParrying";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(oppoAction.Model);
                modbsa.Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ON_WIN_PARRYING);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnLoseParrying(BattleActionModel selfAction, BattleActionModel oppoAction)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "DefeatParrying";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(oppoAction.Model);
                modbsa.Enact(selfAction.Model, selfAction.Skill, selfAction, oppoAction, actevent, BATTLE_EVENT_TIMING.ON_LOSE_PARRYING);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void BeforeAttack(BattleActionModel action)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "BSA";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                } else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, BATTLE_EVENT_TIMING.BEFORE_GIVE_ATTACK);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnSucceedAttack(BattleActionModel action, CoinModel coin, BattleUnitModel target, int finalDmg, int realDmg, bool isCritical, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OSA";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.lastFinalDmg = finalDmg;
                modbsa.lastHpDmg = realDmg;
                modbsa.wasCrit = isCritical;
                modbsa.modsa_coinModel = coin;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(target);
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnSucceedEvade(BattleActionModel evadeAction, BattleActionModel attackAction, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnSucceedEvade";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(attackAction.Model);
                modbsa.Enact(evadeAction.Model, evadeAction.Skill, evadeAction, attackAction, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnEndAttack(BattleActionModel action, int accumulatedDmg, BATTLE_EVENT_TIMING timing, BattleLog_Duel duelLog = null)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "EndSkill";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnEndBehaviour(BattleActionModel action, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnEndBehaviour";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnDie(BattleUnitModel unit, BattleUnitModel killer, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnDie";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(killer);
                modbsa.Enact(unit, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnDieOtherUnit(BattleUnitModel killer, BattleUnitModel deadUnit, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnOtherDie";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(deadUnit);
                modbsa.Enact(this.Owner, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnDiscardSin(UnitSinModel sin, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnDiscard";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.Enact(sin.Model, sin.GetSkill(), null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnKillTarget(BattleActionModel action, BattleUnitModel target, DAMAGE_SOURCE_TYPE dmgSrcType, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "EnemyKill";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.modsa_target_list.Clear();
                modbsa.modsa_target_list.Add(target);
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnRetreat(BattleUnitModel triggerUnit, BUFF_UNIQUE_KEYWORD retreatKeyword, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnRetreat";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.Enact(triggerUnit, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void RightAfterGetAnyBuff(BattleUnitModel unit, BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, int activeRound, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, BattleUnitModel giverOrNull, BattleActionModel actionOrNull, int overStack, int overTurn)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnGainBuff";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modbsa.Pointer.ToInt64())) continue;
                BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modbsa.Pointer.ToInt64()];
                if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;
                currentModular = modbsa;

                MainClass.Logg.LogInfo($"Founds modSystemAbility - GainBuff timing: {this.GetCustomIdentifier()} and {this.GetCustomNameId()}");

                MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                MTCustomScripts.Main.Instance.gainbuff_turn = turn;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = activeRound;
                MTCustomScripts.Main.Instance.gainbuff_source = srcType;
                modbsa.Enact(unit, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void RightBeforeLosingBuff(BUFF_UNIQUE_KEYWORD keyword, int stack, int turn, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnBeforeLoseBuff";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                if (!MTCustomScripts.Main.Instance.keywordTriggerDict.ContainsKey(modbsa.Pointer.ToInt64())) continue;
                BUFF_UNIQUE_KEYWORD trigger = MTCustomScripts.Main.Instance.keywordTriggerDict[modbsa.Pointer.ToInt64()];
                if ((trigger != BUFF_UNIQUE_KEYWORD.None) && (trigger != keyword)) continue;
                currentModular = modbsa;
                MainClass.Logg.LogInfo($"Founds modSystemAbility - BeforeLoseBuff timing: {this.GetCustomIdentifier()} and {this.GetCustomNameId()}");

                MTCustomScripts.Main.Instance.gainbuff_keyword = keyword;
                MTCustomScripts.Main.Instance.gainbuff_stack = stack;
                MTCustomScripts.Main.Instance.gainbuff_turn = turn;
                MTCustomScripts.Main.Instance.gainbuff_activeRound = 0;
                modbsa.Enact(this.Owner, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnBreak(BattleUnitModel unit, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "OnBreak";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.Enact(unit, null, null, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnEndEnemyAttack(BattleActionModel action, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "EnemyEndSkill";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                } else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void OnTakeAttackDamage(BattleActionModel action, BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "WH";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, timing);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override void BeforeTakeAttackDamage(BattleActionModel action)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "BWH";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                if (action.GetMainTarget() != null)
                {
                    modbsa.modsa_target_list.Clear();
                    modbsa.modsa_target_list.Add(action.GetMainTarget());
                }
                else if (modbsa.modsa_target_list.Count > 0) modbsa.modsa_target_list.Clear();
                modbsa.Enact(action.Model, action.Skill, action, null, actevent, BATTLE_EVENT_TIMING.BEFORE_TAKE_ATTACK);
            }
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//


        public override void OnRoundEnd(BATTLE_EVENT_TIMING timing)
        {
            this.ClearAllEditedTemporary();
            string stringTiming = "EndBattle";
            int actevent = MainClass.timingDict[stringTiming];
            foreach (ModularSA modbsa in modularDict[stringTiming])
            {
                currentModular = modbsa;
                modbsa.Enact(this.Owner, null, null, null, actevent, timing);
            }
        }



        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//


        public override int GetExpectedSkillPowerAdder(BattleActionModel action, Il2CppSystem.Collections.Generic.List<BattleActionModel> prevActions, COIN_ROLL_TYPE rollType, SinActionModel expectedTargetSinActionOrNull)
        {
            return currentClassInfo.getSkillPowerAdder.permanentData + currentClassInfo.getSkillPowerAdder.temporaryData;
        }
        public override int GetSkillPowerAdder(BattleActionModel action, COIN_ROLL_TYPE rollType)
        {
            return currentClassInfo.getSkillPowerAdder.permanentData + currentClassInfo.getSkillPowerAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetExpectedSkillPowerResultAdder(BattleActionModel action, Il2CppSystem.Collections.Generic.List<BattleActionModel> prevActions, BattleUnitModel expectedTarget)
        {
            return currentClassInfo.getSkillPowerResultAdder.permanentData + currentClassInfo.getSkillPowerResultAdder.temporaryData;
        }
        public override int GetSkillPowerResultAdder(BattleActionModel action, BATTLE_EVENT_TIMING timing)
        {
            return currentClassInfo.getSkillPowerResultAdder.permanentData + currentClassInfo.getSkillPowerResultAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetExpectedCoinScaleAdder(CoinModel coin)
        {
            return currentClassInfo.getCoinScaleAdder.permanentData + currentClassInfo.getCoinScaleAdder.temporaryData;
        }
        public override int GetCoinScaleAdder(CoinModel coin)
        {
            return currentClassInfo.getCoinScaleAdder.permanentData + currentClassInfo.getCoinScaleAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override float GetExpectedCoinScaleMultiplier(CoinModel coin)
        {
            return currentClassInfo.getCoinScaleMultiplier.permanentData + currentClassInfo.getCoinScaleMultiplier.temporaryData;
        }
        public override float GetCoinScaleMultiplier(CoinModel coin)
        {
            return currentClassInfo.getCoinScaleMultiplier.permanentData + currentClassInfo.getCoinScaleMultiplier.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override COIN_RESULT GetForcedCoinResult()
        {
            if (currentClassInfo.getForcedCoinResult.permanentData > 2) return (COIN_RESULT)currentClassInfo.getForcedCoinResult.permanentData;
            else if (currentClassInfo.getForcedCoinResult.temporaryData > 2) return (COIN_RESULT)currentClassInfo.getForcedCoinResult.temporaryData;
            return base.GetForcedCoinResult();
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override bool IgnoreSinBuffHpDamage(BUFF_UNIQUE_KEYWORD keyword, int dmg, bool isForced, BATTLE_EVENT_TIMING timing)
        {
            if (currentClassInfo.getForcedCoinResult.permanentBannedBuffKeywordList.Contains(keyword) || currentClassInfo.getForcedCoinResult.temporaryBannedBuffKeywordList.Contains(keyword)) return true;
            return base.IgnoreSinBuffHpDamage(keyword, dmg, isForced, timing);
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetExpectedParryingResultAdder(BattleActionModel action, int actorResult, BattleActionModel oppoActionOrNull, int oppoResult)
        {
            return currentClassInfo.getParryingResultAdder.permanentData + currentClassInfo.getParryingResultAdder.temporaryData;
        }
        public override int GetParryingResultAdder(BattleActionModel action, int actorResult, BattleActionModel oppoAction, int oppoResult)
        {
            return currentClassInfo.getParryingResultAdder.permanentData + currentClassInfo.getParryingResultAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override float GetAtkResistAdder(ATK_BEHAVIOUR type)
        {
            int permanentAtkAdder = (currentClassInfo.permanentAtkResistAdderDict.ContainsKey(type)) ? currentClassInfo.permanentAtkResistAdderDict[type] : 0;
            int temporaryAtkAdder = (currentClassInfo.temporaryAtkResistAdderDict.ContainsKey(type)) ? currentClassInfo.temporaryAtkResistAdderDict[type] : 0;

            return permanentAtkAdder + temporaryAtkAdder;
        }
        public override float GetAttributeResistAdder(ATTRIBUTE_TYPE type)
        {
            int permanentSinAdder = (currentClassInfo.permanentSinResistAdderDict.ContainsKey(type)) ? currentClassInfo.permanentSinResistAdderDict[type] : 0;
            int temporarySinAdder = (currentClassInfo.temporarySinResistAdderDict.ContainsKey(type)) ? currentClassInfo.temporarySinResistAdderDict[type] : 0;

            return permanentSinAdder + temporarySinAdder;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetExpectedAttackDmgAdder(BattleActionModel action, BattleUnitModel targetOrNull)
        {
            return currentClassInfo.getAttackDmgAdder.permanentData + currentClassInfo.getAttackDmgAdder.temporaryData;
        }
        public override int GetAttackDmgAdder(BattleActionModel action, BattleUnitModel target)
        {
            return currentClassInfo.getAttackDmgAdder.permanentData + currentClassInfo.getAttackDmgAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override float GetExpectedAttackDmgMultiplier(BattleActionModel action, CoinModel coin, BattleUnitModel targetOrNull, SinActionModel targetSinActionOrNull)
        {
            return currentClassInfo.getAttackDmgMultiplier.permanentData + currentClassInfo.getAttackDmgMultiplier.temporaryData;
        }
        public override float GetAttackDmgMultiplier(BattleActionModel action, CoinModel coin, BattleUnitModel target, bool isCritical)
        {
            return currentClassInfo.getAttackDmgMultiplier.permanentData + currentClassInfo.getAttackDmgMultiplier.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetExpectedTakeAttackDmgAdder(BattleActionModel action, BattleUnitModel attacker)
        {
            return currentClassInfo.getTakeAttackDmgAdder.permanentData + currentClassInfo.getTakeAttackDmgAdder.temporaryData;
        }
        public override int GetTakeAttackDmgAdder(BattleActionModel action, BattleUnitModel attacker)
        {
            return currentClassInfo.getTakeAttackDmgAdder.permanentData + currentClassInfo.getTakeAttackDmgAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override float GetExpectedTakeAttackDmgMultiplier(BattleActionModel action, BattleUnitModel attacker)
        {
            return currentClassInfo.getTakeAttackDmgMultiplier.permanentData + currentClassInfo.getTakeAttackDmgMultiplier.temporaryData;
        }
        public override float GetTakeAttackDmgMultiplier(BattleActionModel action, BattleUnitModel attacker)
        {
            return currentClassInfo.getTakeAttackDmgMultiplier.permanentData + currentClassInfo.getTakeAttackDmgMultiplier.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetMentalSystemResultIncreaseAdder()
        {
            return currentClassInfo.getMentalSystemResultIncreaseAdder.permanentData + currentClassInfo.getMentalSystemResultIncreaseAdder.temporaryData;
        }
        public override int GetMentalSystemResultDecreaseAdder()
        {
            return currentClassInfo.getMentalSystemResultDecreaseAdder.permanentData + currentClassInfo.getMentalSystemResultDecreaseAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetTakeMpDmgAdder(BattleUnitModel attackerOrNull, DAMAGE_SOURCE_TYPE sourceType, BattleActionModel attackerActionOrNull)
        {
            if (currentClassInfo.getTakeMpDmgAdder.permanentBannedSourceTypeList.Contains(sourceType) || currentClassInfo.getTakeMpDmgAdder.temporaryBannedSourceTypeList.Contains(sourceType)) return 0;

            return currentClassInfo.getTakeMpDmgAdder.permanentData + currentClassInfo.getTakeMpDmgAdder.temporaryData;
        }

        //-------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------//

        public override int GetEgoResourceAdder(ATTRIBUTE_TYPE type)
        {
            int permanentAtkAdder = (currentClassInfo.permanentEgoResourceAdderDict.ContainsKey(type)) ? currentClassInfo.permanentEgoResourceAdderDict[type] : 0;
            int temporaryAtkAdder = (currentClassInfo.temporaryEgoResourceAdderDict.ContainsKey(type)) ? currentClassInfo.temporaryEgoResourceAdderDict[type] : 0;

            return permanentAtkAdder + temporaryAtkAdder;
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------//


        public static ModularSA currentModular;
        private int customIdentifier;
        private string customName;
        public ModularSystemAbilityStaticData originalClassInfo;
        public ModularSystemAbilityStaticData currentClassInfo;

        public System.Collections.Generic.Dictionary<string, object> editedParamList = new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
        public System.Collections.Generic.Dictionary<string, string> dataDictionary = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ModularSA>> modularDict = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<ModularSA>>(System.StringComparer.OrdinalIgnoreCase);
    }


    [System.Serializable]
    public class ModularSystemAbilityStaticData
    {

        [JsonProperty]
        public int Id;
        [JsonProperty]
        public string Name;

        public string ModFile = "Undefined";

        //--------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------//


        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getSkillPowerAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getSkillPowerResultAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getParryingResultAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getCoinScaleAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getCoinScaleMultiplier;

        //--------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------//

        [JsonProperty]
        public System.Collections.Generic.Dictionary<ATK_BEHAVIOUR, int> permanentAtkResistAdderDict = new System.Collections.Generic.Dictionary<ATK_BEHAVIOUR, int>();
        public System.Collections.Generic.Dictionary<ATK_BEHAVIOUR, int> temporaryAtkResistAdderDict = new System.Collections.Generic.Dictionary<ATK_BEHAVIOUR, int>();

        [JsonProperty]
        public System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int> permanentSinResistAdderDict = new System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int>();
        public System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int> temporarySinResistAdderDict = new System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int>();

        //--------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------//

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getAttackDmgAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getAttackDmgMultiplier;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getTakeAttackDmgAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getTakeAttackDmgMultiplier;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getTakeMpDmgAdder;

        //--------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------//

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getMentalSystemResultIncreaseAdder;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getMentalSystemResultDecreaseAdder;

        //--------------------------------------------------------------------------------//
        //--------------------------------------------------------------------------------//

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam getForcedCoinResult;

        [JsonProperty]
        public ModularSystemAbilityStaticData_BundledParam ignoreSinBuffHpDamage;


        [JsonProperty]
        public System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int> permanentEgoResourceAdderDict = new System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int>();
        public System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int> temporaryEgoResourceAdderDict = new System.Collections.Generic.Dictionary<ATTRIBUTE_TYPE, int>();

        [JsonProperty]
        public System.Collections.Generic.List<string> modularList = new System.Collections.Generic.List<string>();
    }

    [System.Serializable]
    public class ModularSystemAbilityStaticData_BundledParam
    {
        [JsonProperty]
        public int permanentData = 0;

        [JsonProperty]
        public int temporaryData = 0;




        [JsonProperty]
        public System.Collections.Generic.List<DAMAGE_SOURCE_TYPE> permanentBannedSourceTypeList = new System.Collections.Generic.List<DAMAGE_SOURCE_TYPE>();
        public System.Collections.Generic.List<DAMAGE_SOURCE_TYPE> temporaryBannedSourceTypeList = new System.Collections.Generic.List<DAMAGE_SOURCE_TYPE>();



        [JsonProperty]
        public System.Collections.Generic.List<BUFF_UNIQUE_KEYWORD> permanentBannedBuffKeywordList = new System.Collections.Generic.List<BUFF_UNIQUE_KEYWORD>();
        public System.Collections.Generic.List<BUFF_UNIQUE_KEYWORD> temporaryBannedBuffKeywordList = new System.Collections.Generic.List<BUFF_UNIQUE_KEYWORD>();
    }
}
