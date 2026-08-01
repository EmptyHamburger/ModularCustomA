---@meta

--[[
    MTCustomScripts Version: v22.100.4
]]

--#region Aliases
--- @alias BuffType
--- | "Neutral"
--- | "Positive"
--- | "Negative"

--- @alias BuffCategory
---| "SIN"
---| "RESOURCE"
---| "SHIELD_MANAGER"
---| "BREATH"
---| "CHARGE"
---| "COMBUSTION"
---| "LACERATION"
---| "VIBRATION"
---| "BURST"
---| "SINKING"
---| "BULLET"
---| "CAN_GET_ONLY_BY_SYSTEM"
---| "AACFPBBCA"
---| "FREISHUTZ_OUTIS_EGO_BULLET"
---| "DUEL_DECLARATION"
---| "CONCENTRATED_ATTACK"
---| "DIANXUE"
---| "BURSTREACTIVE"
---| "IGNORE_CHECED_CORRECTION_EXCLUSION"
---| "TURN_IS_ALSO_LOADED_BULLET"
---| "VIBRATION_CONVERTED"
---| "VIBRATION_MERGED"
---| "SUPPORTIVE_PROTECT"

--- @alias AtkResType
--- | "SLASH"
--- | "PENETRATE"
--- | "HIT"

--- @alias SinResType
--- | "CRIMSON"
--- | "SCARLET"
--- | "AMBER"
--- | "SHAMROCK"
--- | "AZURE"
--- | "INDIGO"
--- | "VIOLET"

--- @alias BuffSource
--- | 0 NONE
--- | 1 SKILL
--- | 2 EVENT
--- | 3 BUFF
--- | 4 PASSIVE
--- | 5 SYSTEM_ABILITY
--- | 6 EGO_GIFT
--- | 7 PATTERN
--- | 8 STAGE
--- | 9 UNIT

--- @alias CoinOperatorReturn
--- | 1 ADD
--- | 2 SUB
--- | 3 MUL

--- @alias BuffTypeReturn
--- | 0 Neutral
--- | 1 Positive
--- | 2 Negative

--- @alias ActiveRoundInput
--- | 0 This turn
--- | 1 Next turn
--- | 2 This turn and next turn

--- @alias ActiveRoundReturn
--- | 0 This turn
--- | 1 Next turn

--- @alias ComparerOperator
--- | "=" equal
--- | ">" the data retrieved contains input
--- | "<" input does not contain the data retrieved

--- @alias HasBuffKeywordCheckType
--- | "main" main keyword
--- | "sub" sub keyword
--- | "maub" main and sub keyword
--- | "mainsub" main and sub keyword
--- | "category" category keyword

--- @alias MapData
--- | "size" the map's size (rounded down)
--- | "active" Return 1 if the map is active (the game is using it). Return 0 if the map is inactive
--- | "id" Store this value to an mtdata on the unit equal to SetMTData(Self, dataID, MapID, GetMapData)

--- @alias EditBuffMode
--- | "adder" edit the buff's bonus maximum value
--- | "vanilla" edit the buff's vanilla maximum value
--- | "both" edit both buff's bonus maximum value and vanilla maximum value

--- @alias EditBuffLoseValueAtMax
--- | "info" if there is a problem with the function (modifier), use this
--- | "lowmax" If the buff’s current potency/count is greater than it's respective maximum value, reduce the exceeded value by the difference (current − max)
--- | "both" do both what info and lowmax do

--- @alias DamageSource
--- | "COMBAT"
--- | "BUFF"
--- | "PASSIVE"
--- | "SKILL"
--- | "EVENT"
--- | "EGO_GIFT"
--- | "STAGE"
--- | "SYSTEM"
--- | "SYSTEM_ABILITY"
--- | "FORCED"
--- | "NONE"

--- @alias PanicLevel
--- | 0 Get out of Low Morale and Panic or Corrosion (if possible)
--- | 1 Set this unit to Low Morale
--- | 2 Set this unit to Panic
--- | 3 Corrode this unit (only if it can corrode)

--- @alias PanicVariable
--- | "Forcefully" (Used for [Panic_Level]=0) Get this unit out of Low Morale and Panic if this unit normally shouldn't
--- | "Lastest" (Used for [Panic_Level]=3) Set this unit Corrosion to the last EGO used
--- | "Random" (Used for [Panic_Level]=3) Set this unit Corrosion to a random EGO
--- | "TETH" (Used for Panic-Level=3) Set this unit Corrosion to the current TETH EGO if possible
--- | "HE" (Used for [Panic_Level]=3) Set this unit Corrosion to the current HE EGO if possible
--- | "WAW" (Used for [Panic_Level]=3) Set this unit Corrosion to the current WAW EGO if possible
--- | "ALEPH" (Used for [Panic_Level]=3) Set this unit Corrosion to the current ALEPH EGO if possible

--- @alias PanicPropriety
--- | "Default" (Self Explanatory)
--- | "Cached" Couldn't find any differences with default
--- | "Buff" Use it if you want to overwrite the PanicType by using a Buff (or overwriting it), requires to have the OptBuff case filled

--- @alias PanicOptionalBuff
--- | "Keyword" Select the buff using the keyword
--- | "Current" If used from buff script, select this buff

--- @alias PanicLevelReturn
--- | -1 If this unit is in Corrosion or not in a quantifiable PanicLevel
--- | 0 If the Unit is not in Low Morale, Panic or Corrosion
--- | 1 If the unit is in Low Morale
--- | 2 If the unit is in Panic

--- @alias BreakLevelReturn
--- | 0 if the unit is not staggered
--- | 1 if the unit is staggered (Stagger)
--- | 2 if the unit is staggered+ (Stagger+)
--- | 3 if the unit is staggered++ (Stagger++)

--- @alias DataType
--- | "Scale" returns the coin power
--- | "ScaleAdder" returns the additional coin power (returns -1 if the the skill is not used this Turn) 
--- | "Final" returns the Final Power (returns -1 if the skill does not target a skill)
--- | "Clash" returns the Clash Power (returns -1 if the skill does not target a skill)
--- | "Weight" return the Attack Weight (returns -1 if the skill does not target a skill) 
--- | "ogWeight" returns the original Attack Weight
--- | "Evade" returns the additional Evade Power (returns -1 if the skill is not used this Turn, or does not target a skill)
--- | "Default" returns the skill Base Power 
--- | "Motion" returns the skill Motion (refer to the enum, will be added later) 
--- | "Level" returns the skill Level 
--- | "SkillAtkLevel" returns the skill Offense Level 
--- | "DefType" same as `getskilldeftype()`
--- | "AtkType" same as `getskillatk()`
--- | "Rank" same as `getskillrank()`
--- | "Fixed" same as `getskillfixedtarget()`
--- | "Attribute" same as `getskillattribute()`
--- | "EgoType" same as `getskillegotype()`
--- | "IsAction" returns 1 if the skill is being used this Turn 
--- | "UseCount" returns the amount of time the skill is being used this Turn 
--- | "TargetClash" returns the Clash Power of the skill this is clashing with (returns -1 if the skill is not used this Turn, or does not target a skill)
--- | "TargetType" returns the type of the skill target (refer to the enum, will be added later) (returns -1 if the the skill is not used this Turn) 
--- | "TargetCount" returns the amount of Targets (returns -1 if the the skill is not used this Turn)
--- | "RealTargetCount" returns the amount of Targets (Was added in case TargetCount bugs) (returns -1 if the the skill is not used this Turn) 
--- | "IsTargettingName" returns if the skill is targetting a unit with a specific name (returns -1 if the the skill is not used this Turn)
--- | "IsTargettingUniqueName" returns if the skill is targetting a unit with a specific unique name (returns -1 if the the skill is not used this Turn)
--- | "IsTargettingID" returns if the skill is targetting a unit with a specific ID (returns -1 if the the skill is not used this Turn)
--- | "IsTargettingMainName" returns if the skill main target has a specific name (returns -1 if the the skill is not used this Turn)
--- | "IsTargettingMainUniqueName" returns if the skill main target has a specific unqiue name (returns -1 if the the skill is not used this Turn)
--- | "IsTargettingMainID" returns if the skill main target has a specific ID (returns -1 if the the skill is not used this Turn)
--- | "ID" returns the skill ID

--- @alias VarSkillData
--- | "MTCustomScripts's Target-Coin"
--- | "MTCustomScripts's Single-Coin" used for `ScaleAdder` and `Clash`
--- | "MTCustomScripts's Multi-Coin" used for `Final`
--- | "Name" used for `IsTargettingName and `IsTargettingMainName`
--- | "UniqueName" used for `IsTargettingUniqueName` and `IsTargettingMainUniqueName`
--- | "ID" used for `IsTargettingID`and `IsTargettingMainID`

--- @alias LoopType
--- | "Restart" When a loop ends it will restart from the beginning (Default)
--- | "Yoyo" When a loop ends it will play backwards until it completes another loop, then forward again, then backwards again, and so on and on and on
--- | "Incremental" Each time a loop ends the difference between its endValue and its startValue will be added to the endValue, thus creating tweens that increase their values with each loop cycle

--- @alias ResonReturnType
--- | 1 when the selected unit is part of the sinType resonance. If sinType is not valid, it will instead try to check if the selected unit is a part of the highest resonance instead
--- | -1 when the selected unit can't be found

--- @alias SkillSendOption
--- | integer Any skill ID
--- | "S#" # being the skill tier
--- | "D#" # being the defense skill index

--- @alias TimingID
--- | 0 StartBattle
--- | 1 WhenUse
--- | 2 BeforeAttack
--- | 3 StartDuel
--- | 4 WinDuel
--- | 5 DefeatDuel
--- | 6 EndBattle
--- | 7 OnSucceedAttack
--- | 8 WhenHit
--- | 9 EndSkill
--- | 10 FakePower
--- | 11 BeforeDefense
--- | 12 OnDie
--- | 13 OnOtherDie
--- | 14 DuelClash
--- | 15 DuelClashAfter
--- | 16 OnSucceedEvade
--- | 17 OnDefeatEvade
--- | 18 OnStartBehaviour
--- | 19 BeforeBehaviour
--- | 20 OnEndBehaviour
--- | 21 EnemyKill
--- | 22 OnBreak
--- | 23 OnOtherBreak
--- | 24 OnDiscard
--- | 25 OnZeroHP
--- | 26 EnemyEndSkill
--- | 27 OnOtherBurst
--- | 28 BeforeSA
--- | 29 BeforeWhenHit
--- | 30 BeforeUse
--- | 31 Immortal
--- | 32 ImmortalOther
--- | 33 SpecialAction
--- | 34 AfterSlots
--- | 35 OnCoinToss
--- | 36 StartBattleSkill
--- | 37 OnBurst
--- | 38 StartVisualCoinToss
--- | 39 StartVisualSkillUse
--- | 40 WhenGained
--- | 41 ChangeMotion
--- | 42 IgnorePanic
--- | 43 IgnoreBreak
--- | 44 OnRetreat
--- | 45 OnGainBuff
--- | 46 OnUseBuff
--- | 47 EncounterStart
--- | 48 WinParrying
--- | 49 DefeatParrying
--- | 50 ChangeTakeDamage
--- | 51 OnCoinAfterAttack
--- | 52 EnemyBeforeAttack
--- | 53 AfterChangeShield
--- | 54 AfterChangeHP
--- | 55 CanDealTarget
--- | 56 DelayedStart
--- | 57 StartVisualDuelEnd
--- | 58 StartVisualGiveDamage
--- | 59 StartVisualDuel
--- | 60 StartVisualDie
--- | 61 StartVisualPartDestroy
--- | 62 StartVisualChaseTarget
--- | 63 BufMaxStackAdder
--- | 64 BufMaxTurnAdder
--- | 65 ChangeAttackDamage
--- | 7332 SortAction
--- | 7333 Parrying
--- | 7334 BeforeRoundStart
--- | 7335 WaitCommand
--- | 90901 OnPanic
--- | 90902 OnOtherPanic
--- | 90903 OnLowMorale
--- | 90904 OnOtherLowMorale
--- | 90905 OnRecoverBreak
--- | 90906 OnOtherRecoverBreak
--- | 90907 OnTakePiledVibration
--- | 90908 OnOtherTakePiledVibration
--- | 90909 OnTakeSwitchingVibration
--- | 90910 OnOtherTakeSwitchingVibration
--- | 90911 OnLoseBuff
--- | 90912 OnBeforeLoseBuff
--- | 90913 OnChangeSP
--- | 90914 OnOtherChangeSP
--- | 90915 OnTakeSPDamage
--- | 90916 OnOtherTakeSPDamage
--- | 90917 OnUnOpposed
--- | 90918 OnEquipDefense

--- @alias EffectType
--- | "OVERCLOCK_STABLE" Corrosion stable effect (CAN'T BE APPLIED TO TOP SLOT)
--- | "OVERCLOCK_UNSTABLE" Corrosion unstable effect (CAN'T BE APPLIED TO TOP SLOT)
--- | "BINAH_EGO" Superbia EGO effect (CAN'T BE APPLIED TO TOP SLOT)
--- | "INDEX_FINGER" Mark of the Prescript effect
--- | "RING_FAVUISM_TEST" Ring fauvist effect (ring rodya critique shiny effect thing)

--- @alias MotionID
--- | "Default" 
--- | "Dead" 
--- | "Evade" 
--- | "Guard" 
--- | "Damaged" 
--- | "Move" 
--- | "Attack" 
--- | "S1" 
--- | "S2" 
--- | "S3" 
--- | "S4" 
--- | "S5" 
--- | "S6" 
--- | "S7" 
--- | "S8" 
--- | "S9" 
--- | "S10" 
--- | "Parrying" 
--- | "Idle" 
--- | "Parrying_Range" 
--- | "Special1" 
--- | "Special2" 
--- | "Special3" 
--- | "Parrying_Lose" 
--- | "S11" 
--- | "S12" 
--- | "S13" 
--- | "S14" 
--- | "S15" 
--- | "S16" 
--- | "S17" 
--- | "S18" 
--- | "S19" 
--- | "S20" 
--- | "S21" 
--- | "Empty" 
--- | "Duel_Ready" 
--- | "Duel_Win" 
--- | "Duel_Lose" 
--- | "Damaged_2" 
--- | "Damaged_3" 
--- | "Duel_Ready_Actor" 
--- | "Duel_Ready_Target" 
--- | "Duel_Compation" 
--- | "Retire" 
--- | "Retreat" 
--- | "UnRetreat"

--- @alias ListRelatedKeywordsMode
--- | "sub" sub keywords
--- | "category" category keywords
--- --#endregion

--#region Acquisitions and Consequences

--- Return an integer representing the coin's operator type
--- @param unit "Self" | "MainTarget"
--- @param coin_index integer --The index of the coin, starting at 0. If this is higher than the highest coin index the skill has, it will be set to highest coin index
--- @return CoinOperatorReturn
--- @nodiscard
function coinoperator(unit, coin_index) return 1 end

--- @param keyword string --The buff's keyword
--- @return BuffTypeReturn
--- Return an integer representing the buff's type
--- @nodiscard
function bufftype(keyword) return 0 end

--- Overrides an attack resistance value
---@param Multi_Target string --Modular's Multi-Target
---@param atkType AtkResType
---@param newValue integer --this will be divided by 100
---@param add boolean
---@param defaultLimit? any --Adding this optional argument will limit the unit's resistance value in range of 0.00 to 2.00
function ovwatkres(Multi_Target, atkType, newValue, add, defaultLimit) return end

--- Overrides a sin resistance value
---@param Multi_Target string --Modular's Multi-Target
---@param sinType SinResType
---@param newValue integer --this will be divided by 100
---@param add boolean
---@param defaultLimit? any --Adding this optional argument will limit the unit's resistance value in range of 0.00 to 2.00
function ovwsinres(Multi_Target, sinType, newValue, add, defaultLimit) return end

--- Refreshes speed value. Useful case: Fix speed when using MaxSpeedAdder or MinSpeedAdder system abilities
---@param Multi_Target string --Modular's Multi-Target
function refreshspeed(Multi_Target) return end

--- Single buff destroy mode
---@param Multi_Target string --Modular's Multi-Target
---@param keyword string --the buff's keyword
---@param destroyRound ActiveRoundInput --the active round of the existing buff
function destroybuff(Multi_Target, keyword, destroyRound) return end

--- Buff type / buff category based destroy mode. Destroys [amount] of existing buffs chosen randomly, filtered by buff type / buff category
---@param Multi_Target string --Modular's Multi-Target
---@param mode BuffType | BuffCategory --filter which buffs to destroy
---@param destroyRound ActiveRoundInput --the active round of the existing buff
---@param amount integer | "All" --number of buffs to destroy (any integer >= 0) or "All" (all buffs filtered by [mode])
---@param includeCantBeDespelled? any --Adding this optional argument will include buffs with "canBeDespelled = false"
function destroybuff(Multi_Target, mode, destroyRound, amount, includeCantBeDespelled) return end

---Returns an integer representing the target's attack resistance value (doesn't work on abnor)
---@param Single_Target string --Modular's Single-Target
---@param atkType AtkResType
---@return integer --will be multiplied by 100. Ex: x0.75 => return 75
--- @nodiscard
function getatkres(Single_Target, atkType) return 0 end

---Returns an integer representing the target's sin resistance value (doesn't work on abnor)
---@param Single_Target string --Modular's Single-Target
---@param sinType SinResType
---@return integer --will be multiplied by 100. Ex: x0.01 => return 1
--- @nodiscard
function getsinres(Single_Target, sinType) return 0 end

---If the unit has used a defense action this turn then return 1, else return 0
---@param Single_Target string --Modular's Single-Target
---@return 0 | 1
--- @nodiscard
function useddefaction(Single_Target) return 0 end

---Deactivates 1 or all active stagger bars
---@param Multi_Target string --Modular's Multi-Target
---@param breakIndex integer --The index of the stagger bar (integer). Index starts at 0. Use -1 to deactivate all stagger bars (this will ignore [sort] and [reverseIndex] as they don't matter anymore)
---@param sort boolean --If true, the list of active stagger bars will be sorted in descending order (THIS ARGUMENT IS NOT REQUIRED IF [breakIndex] IS -1)
---@param reverseIndex? any --Index still starts at 0 but the active stagger bars list is reversed. THIS RUNS AFTER [sort]
function deactivebreak(Multi_Target, breakIndex, sort, reverseIndex) return end

---Pick [amount] of existing buffs on the target(s) then add/remove potency and count based on the buff category. Accept negative values
---@param Multi_Target string --Modular's Multi-Target
---@param buffCategory BuffCategory --affected buff category
---@param stack integer --potency/stack
---@param turn integer --count/turn
---@param activeRound ActiveRoundInput --the active round of the existing buff
---@param StackTurnAddRespectively boolean --true: adds stack potency and turn count for each selected buff. false: For buffs without Count, adds ([stack] + [turn]) potency. For buffs have Count, add random X potency and Y count (X + Y = [stack] + [turn])
---@param amount integer number of buffs affected
function bufcategory(Multi_Target, buffCategory, stack, turn, activeRound, StackTurnAddRespectively, amount) return end

---Set the unit's defense value
---@param Multi_Target string --Modular's Multi-Target
---@param newValue integer
function defcorrection(Multi_Target, newValue) return end

---Add an 'unit script' to the target(s)
---@param Multi_Target string --Modular's Multi-Target
---@param id string --unit script's id
function addunitscript(Multi_Target, id) return end

---Change the defense skill of the unit and won't change back to original defense skill until you change it manually. The defense skill MUST BE in the unit's arsenal
---@param Multi_Target string --Modular's Multi-Target
---@param defenseSkillId integer
function changedefense(Multi_Target, defenseSkillId) return end

---Returns an integer representing the unit's faction (works on both normal unit and abnor)
---@param Single_Target string --Modular's Single-Target
---@return 1 | 0 --1: PLAYER (Ally / Player faction) | 0: ENEMY (Enemy faction)
--- @nodiscard
function unitfaction(Single_Target) return 0 end

---Uses with 'SpecialAction' timing. Returns an integer representing the slot index (of that unit) which user last performed a 'Special Action' on
---@return integer
--- @nodiscard
function saslotindex() return 0 end

---Uses with 'OnGainBuff' timing. Returns an integer representing the stack/potency of the gained buff
---@return integer
--- @nodiscard
function gbstack() return 0 end

---Uses with 'OnGainBuff' timing. Returns an integer representing the turn/count of the gained buff
---@return integer
--- @nodiscard
function gbturn() return 0 end

---Uses with 'OnGainBuff' timing. Returns an integer representing the active around of the gained buff
---@return 0 | 1 --0: This turn | 1: Next turn
--- @nodiscard
function gbactiveround() return 0 end

---Uses with 'OnGainBuff' timing. Returns an integer representing the source of the gained buff
---@return BuffSource
--- @nodiscard
function gbsource() return 0 end

---Compare input to an mtdata
---@param Single_Target string --Modular's Single-Target
---@param ValueToCompare string --The string to compare
---@param Operator ComparerOperator
---@param DataID string The DataID of the mtdata
---@param DataSource? any --If this value is set, search for a data with the DataID AND DataSource
---@return 0 | 1 --Return 1 if the comparison result is true. Return 0 if the comparison result is false
--- @nodiscard
function comparer(Single_Target, ValueToCompare, Operator, DataID, DataSource) return 0 end

---Check if the [targetKeyword] buff's [checkType] has [checkKeyword].
---@param Single_Target string ----Modular's Single-Target. You should leave this empty/random input like "_" if [targetKeyword] is "current"
---@param targetKeyword string | "current" --Any buff keyword | Use "current" to target the buff itself (only for buff scripts)
---@param checkType HasBuffKeywordCheckType --which property of the buff to check
---@param checkKeyword string --Any buff / buff-category keyword
---@param print? any --If this value is set, store an mtdata to the unit equal to SetMTData(Self, BuffKeyword_CheckType, Result, HasBuffKeyword)
---@return 1 | 0 --1: true | 0: false
--- @nodiscard
function hasbuffkeyword(Single_Target, targetKeyword, checkType, checkKeyword, print) return 0 end

--- Return the [mapName]'s [data]
---@param mapName string --any map name (a string) | use "current" for the current map the game is using
---@param data MapData
---@param dataID? integer
--- @nodiscard
function getmapdata(mapName, data, dataID) return 0 end

---Add or set the maximum potency/count value of a buff
---@param Multi_Target string ----Modular's Multi-Target. You should leave this empty/random input like _ if [buffOwner] is "current"
---@param buffOwner string | "current" --Any buff keyword | "current" to target the buff itself (only for buff scripts)
---@param property "stack" | "count" | "both"
---@param mode EditBuffMode
---@param addOrSet "add" | "set" --add or set to [value]
---@param value integer --accepts negative value
---@param loseValueAtMax? EditBuffLoseValueAtMax
function editbuffmax(Multi_Target, buffOwner, property, mode, addOrSet, value, loseValueAtMax) return end

---Set the targets' HP
---@param Multi_Target string --Modular's Multi-Target
---@param newHp integer --any value >= 0
---@param damageSource DamageSource
---@param buffKeyword string | "None" --Default = "None"; Any buff keyword, only matter if [damageSource] is "BUFF" (Example: you know when Bleed got activated and you see a Bleed icon float up on the enemy right?)
---@param deactivePassedBreakSection boolean --If true, deactivate an active stagger bar if newHp value is set to a value lower than a stagger threshold value (for some reason, this can only deactivate 1 active stagger bar. Who knows why)
---@param attacker? string --Modular's Single-Target (who is the attacker, ik u will leave this empty for most of the time)
function changehp(Multi_Target, newHp, damageSource, buffKeyword, deactivePassedBreakSection, attacker) return end

---Set the targets' SP
---@param Multi_Target string --Modular's Multi-Target
---@param newSp integer
function changesp(Multi_Target, newSp) return end

---Set the units current sanity state
---@param Multi_Target string --Modular's Multi-Target
---@param Panic_Level PanicLevel
---@param Variable PanicVariable --Optional conditionals required for some Panic-Level or just as an option
function changepaniclevel(Multi_Target, Panic_Level, Variable) return end

---Set the unit's sanity type
---@param Multi_Target string --Modular's Multi-Target
---@param PanicType string --The panic-type to change into (eg. Ruins)
---@param PanicPropriety PanicPropriety --The unit propriety to change the Panic from
---@param OptBuff PanicOptionalBuff --Required field if PanicPropriety is set to Buff
function changepanictype(Multi_Target, PanicType, PanicPropriety, OptBuff) return end

---Get the current skill Final Power
---@return integer
--- @nodiscard
function getfinal() return 0 end

---Get an integer representing the unit sanity state of the unit
---@param Single_Target string --Modular's Single-Target
---@return PanicLevelReturn --the values equal to the return types
--- @nodiscard
function getpaniclevel(Single_Target) return 0 end

---Check if the unit used skill with skillId last turn. Returns 1 if true, returns 0 if false.
---@param Single_Target string --Modular's Single-Target
---@param skillId integer --the skill's ID
---@return 1 | 0 --1 if true, returns 0 if false
--- @nodiscard
function diduseskillprevturn(Single_Target, skillId) return 0 end

---Returns the buff's stack/potency the unit has gained this turn
---@param Single_Target string --Modular's Single-Target
---@param buffKeyword string --any buff keyword
---@return integer
--- @nodiscard
function getbuffstackgainedthisturn(Single_Target, buffKeyword) return 0 end

---Get the current stagger level of the unit
---@param Single_Target string --Modular's Single-Target
---@return BreakLevelReturn
--- @nodiscard
function getbreaklevel(Single_Target) return 0 end

---Returns 1 if the unit can make action, returns 0 if the unit can't make action (Staggered, immobilized,...)
---@param Single_Target string --Modular's Single-Target
---@return 1 | 0
--- @nodiscard
function isactionable(Single_Target) return 0 end

---Instantly kill the unit(s)
---@param Multi_Target string --Modular's Multi-Target
---@param ignoreImmortal boolean --If true, ignore all immortal effect(s) to kill the unit. If false, unit with immortal effect(s) won't be killed
---@param dmgSource DamageSource --source of the damage
---@param killer? string --Modular's Single-Target. Determine who is the killer
---@param action? "Self" | "MainTarget" --Specifies which action (mostly skill) is credited with the kill and therefore triggers any On Kill effects (Haven't test this one yet)
function instantdeath(Multi_Target, ignoreImmortal, dmgSource, killer, action) return end

---Staggers the target immediately (This consequence exists because break can not be used in lua modular)
---@param Multi_Target string --Modular's Multi-Target
---@param staggerType "natural" | "force" | "both"
function lbreak(Multi_Target, staggerType) return end

---Returns the ID of the skill the Target being used. Only usable in a Clash. Returns 0 if target's skill not found.
---@return integer
--- @nodiscard
function getopposkillid() return 0 end

---Removes any number of coins
---@param Single_Target string --Modular's Single-Target
---@param ... integer  --The coin's index, input -1 for coin scripts to target themselves (coin index starts at 0) (You can input as many indexes as you need)
function removecoin(Single_Target, ...) return end

---@param Multi_Target string --Modular's Multi-Target
---@param SkillID integer --the ID of the Skill
---@param Level integer --the Level of the Skill
---@param Uptie integer --the Uptie of the Skill
---@param Amount integer --the Amount of the Skill in the pool
function addskill(Multi_Target, SkillID, Level, Uptie, Amount) return end

---Remove a skill permanently. Does not remove base skill in the UI for the "Summary" tab
---@param Multi_Target string --Modular's Multi-Target
---@param Multi_Skill string --MTCustomScripts's Multi-Skill
function removeskill(Multi_Target, Multi_Skill) return end

---@param Single_Target string --Modular's Single-Target
---@param SkillID integer --The Skill ID
---@return integer --If the target has the skill then returns 1, else returns 0
---@nodiscard
function hasskill(Single_Target, SkillID) return 0 end

---@param Single_Target string --Modular's Single-Target
---@param Single_Skill string --MTCustomScripts's Single-Skill
---@param DataType DataType --The type of data to return
---@param Var VarSkillData --a variable might needed for a Data-Type
---@return integer
---@nodiscard
function getskilldata(Single_Target, Single_Skill, DataType, Var) return 0 end

---this consequence has two modes, you can either create a coin based on another coin (Single-Unit to Single-Coin); or make a coin based on specific settings (Power to Color)
---@param Multi_Target string --Modular's Multi-Target
---@param Multi_Skill string --MTCustomScripts's Multi-Skill
---@param CoinIndex integer --Set the coin current index (Origin Index is set as last, Real index is set as this number)
---@param VAR_4 string | "Power" --Modular's Single-Target or the Coin Power of the target
---@param VAR_5 string | "Operator" --MTCustomScripts's Multi-Skill or The coin operator to set
---@param VAR_6 string | "Color" --MTCustomScripts's Single-Coin or Set the color of the coin
---@param CopyStaticData any --Add this argument if you only want to copy the static data of the targetted coin
function addcoin(Multi_Target, Multi_Skill, CoinIndex, VAR_4, VAR_5, VAR_6, CopyStaticData) return end

---Get the current skill's power
---@param Target "Self" | "MainTarget"
---@return integer
---@nodiscard
function getcurrentpower(Target) return 0 end

---Clear/Remove all abilities of the skill
---@param Target "Self" | "MainTarget"
function clearskillabilities(Target) return end

---Clear/Remove all abilities of coin(s). If there are no coinIndex, clear all coins' abilities
---@param Target "Self" | "MainTarget"
---@param ...? integer --coinIndex (optional): The coin's index, input -1 for coin scripts to target themselves (coin index starts at 0) (You can input as many indexes as you need)
function clearcoinabilities(Target, ...) return end

---Add a skill script to the skill
---@param Multi_Target string --MTCustomScripts's Multi-Target
---@param Multi_Skill string --MTCustomScripts's Multi-Skill
---@param skillAbilityName string --A vanilla SkillAbility class name / If you want to put modular scripts, put "modReplace"
---@param skillScriptName string --The script name of the "skillAbilityName" / When putting modular lines, you have to start with the timing, and replace some characters. `:` with `;` | `/` with `\` | `(` with `<<` | `)` with `>>`
---@param turnLimit? integer --The amount of the ability will trigger in a turn (not needed at all for modular lines, or some abilities)
function addskillability(Multi_Target, Multi_Skill, skillAbilityName, skillScriptName, turnLimit) return end

---Add a coin script to specific coins. If there are no coinIndex, add the coin script to every coin
---@param Target "Self" | "MainTarget"
---@param coinScriptName string --A vanilla coin script name
---@param ...? integer --coinIndex (optional): The coin's index, input -1 for coin scripts to target themselves (coin index starts at 0) (You can input as many indexes as you need)
function addcoinability(Target, coinScriptName, ...) return end

---Remove all sub-targets from the skill, only let the skill attack the main target
function removealltargetexceptmaintarget() return end

---Mid-combat phase mode. Set the main target of the skill to Single-Target
---@param Mid "Mid" --Fixed
---@param Single_Target string --Modular's Single-Target
function setmaintarget(Mid, Single_Target) return end

---Pre-combat phase mode (only works with timings: SpecialAction and WaitCommand). Set the main target of Count skill(s) with skilll id = SkillID of every Attackers to Target
---@param Pre "Pre" --Fixed
---@param Attackers string --Modular's Multi-Target
---@param Target string --Modular's Single-Target
---@param SkillID integer --A Skill ID
---@param Count? integer --Any integer >= 1 (Default = 99)
function setmaintarget(Pre, Attackers, Target, SkillID, Count) return end

---Add/Remove specific sub-targets from the skill
---@param Mode "Add" | "Remove"
---@param exceptTargetedUnits boolean --If true, automatically filters out any units that are already the main target or already present in the sub-target list of the skill
---@param includeTargets string --Modular's Multi-Target
---@param excludeTargets? string --Modular's Multi-Target (Highest priority, runs after everything)
function modifysubtarget(Mode, exceptTargetedUnits, includeTargets, excludeTargets) return end

---Set the SP usage of the skill (like how EGO skills consume SP on use)
---@param newValue integer
---@param add? any --Add newValue to the skill's SP usage instead
function setspusage(newValue, add) return end

---Set level, yes
---@param Multi_Target string --Modular's Multi-Target
---@param newLevel integer --Any integer >= 1 (idk bro, if you input negative number the game might break)
function setlevel(Multi_Target, newLevel) return end

---Self-Explaination
---@param Multi_Target string --Modular's Multi-Target
---@param newMaxHp integer
function setmaxhp(Multi_Target, newMaxHp) return end

---Get the default max HP of the unit (Default max HP is the max HP when the unit's level is 0)
---@param Single_Target string --Modular's Single-Target
---@return integer
---@nodiscard
function getdefaultmaxhp(Single_Target) return 0 end

---Get the HP increment by level of the unit multiplied by 100 and rounded down
---@param Single_Target string --Modular's Single-Target
---@return integer
---@nodiscard
function gethpincrement(Single_Target) return 0 end

---Get a string value from a specific unit (This method tries to turn the value into a number, if it cannot it will return 0)
---@param Single_Target string --Modular's Single-Target (If the targetting is invalid, it will return data linked to the encounter)
---@param DataID string The ID of the Data to search (will return the first Data with ID equal to this)
---@param DataSource? any --If this valeu is set, will search for a string with the DataID and the DataSource (as a string)
---@return integer
---@nodiscard
function getmtdata(Single_Target, DataID, DataSource) return 0 end

---Set a string value to a specific unit
---@param Single_Target string --Modular's Single-Target (If the targetting is invalid, the data will become linked to the encounter)
---@param DataID string --The ID of the Data to search
---@param DataValue string --The value of the Data
---@param DataSource? string --The Source of the Data
function setmtdata(Single_Target, DataID, DataValue, DataSource) return end

---This value will crash your code if there the unit you target did not have any SP changes
---@param Single_Target string --Modular's Single-Target
---@param TargetValue "oldsp" | "newsp" --Return the SP before/after the change
---@return integer
---@nodiscard
function getchangespvalue(Single_Target, TargetValue) return 0 end

---Returns the uptie level of the unit
---@param Single_Target string --Modular's Single-Target
---@return integer
---@nodiscard
function getuptielevel(Single_Target) return 0 end

---Returns the slot index of the skill being used. 0 is the leftmost slot, increased by 1 whenever it shifts to right. Returns -1 if the unit is not using a skill
---@return integer
---@nodiscard
function getskillslotindex() return 0 end

---Returns 1 when the sinner with the specified characterID is fielded. Else, returns 0
---@param characterID integer --1 = Yi Sang, 2 = Faust, 3 = Don Quixote, 4 = Ryoshu, 5 = Meursault, 6 = Hong Lu, 7 = Heathcliff, 8 = Ishmael, 9 = Rodion, 10 = Sinclair, 11 = Outis, 12 = Gregor
---@return integer
---@nodiscard
function issinnerfielded(characterID) return 0 end

---Adds a keyword to the specified unit in the middle of the encounter
---@param Multi_Target string --Modular's Multi-Target
---@param keywordName string --Any string that could be a unitKeyword or association in the json data of a unit
---@param isAssociation? any --Whether or not the keyword is an association | If this option exists, it tries to add the keyword as an association ID instead
function addkeyword(Multi_Target, keywordName, isAssociation) return end

---Clears all temporary skill abilities (like changed sin affinities, scripts given via giveskillscript, etc.) from all of your skills in the dashboard
---@param Multi_Target string --Modular's Multi-Target
function clearalltempskillabilities(Multi_Target) return end

---Replaces all of the skill affinities on your slots with the specified affinity
---@param Multi_Target string --Modular's Multi-Target
---@param sinType SinResType
---@param includeEgo? any --Whether or not to include EGO | If this option exists, it also changes the sin affinities of EGOs. They're normally ignored
function replaceallaffinity(Multi_Target, sinType, includeEgo) return end

---Replaces a skill on a dashboard with a specified skill at the specified coordinates
---@param Single_Target string --Modular's Single-Target
---@param column integer --The Skill Column Index | Which 'column' you want the skill to spawn in. 0 is the leftmost skill 'column', with it shifting to right with each increase in number
---@param topOrBottom 0 | 1 --Top Slot or Bottom Slot | Which slot you want the skill to spawn in. 0 for the Bottom Slot, 1 for the Top Slot 
---@param skillID integer --ID of the Skill you want to spawn in
function replaceskillondashboard(Single_Target, column, topOrBottom, skillID) return end

---Converts the leftmost-bottom instance of a Skill into another Skill
---@param Single_Target string --Modular's Single-Target
---@param searchID integer --The ID of the skill you want to convert
---@param replaceID integer --The ID of the replacement skill
function upgradeskillondashboard(Single_Target, searchID, replaceID) return end

---Add a clash between 2 units instantly with determined skills
---@param unit_1 string --Modular's Single-Target
---@param unit_2 string --Modular's Single-Target
---@param skillIDforUnit_1 integer --the skill id for unit 1 to use (self-explanation) (the skill must be CLASHABLE)
---@param skillIDforUnit_2 integer --the skill id for unit 2 to use (self-explanation) (the skill must be CLASHABLE)
function addduel(unit_1, unit_2, skillIDforUnit_1, skillIDforUnit_2) return end

---Returns the world position cordType value of the unit multiplied by 100
---@param Single_Target string --Modular's Single-Target
---@param cordType "x" | "y" | "z"
---@return integer
---@nodiscard
function getworldpos(Single_Target, cordType) return 0 end

---Set world position
---@param Multi_Target string --Modular's Multi-Target
---@param x integer --the x coordinate values (this value will be divided by 100)
---@param y integer --the y coordinate values (this value will be divided by 100)
---@param z integer --the z coordinate values (this value will be divided by 100)
function setworldpos(Multi_Target, x, y, z) return end

---Move the target to the destinated world cords with determined time and easing style
---@param Multi_Target string --Modular's Multi-Target
---@param Sequences string --one or multiple sequences; A sequence must follow the format: "{x; y; z; duration; easingStyle}"
---@param loopCount? 1 | integer | -1 --If you dont want loop, dont ever input this. 1 = No loop (Default), Any positive integer, -1 = Inf loop
---@param loopType? LoopType
function setworldpos(Multi_Target, Sequences, loopCount, loopType) return end

---Checks to see if the skill being used is actually reused. Only accepts timings that use a skill
---@param Target "Self" | "MainTarget"
---@return integer
---@nodiscard
function isreusedskill(Target) return 0 end

---@param Single_Target string --Modular's Single-Target
---@param sinType SinResType
---@return ResonReturnType
---@nodiscard
function isunitpartofreson(Single_Target, sinType) return 1 end

---Scans the entire dashboard to search for the skill with the desired skillID. Returns 1 if found. Returns -1 when the selected unit can't be found
---@param Single_Target string --Modular's Single-Target
---@param skillID integer --A skill ID
---@return integer
---@nodiscard
function hasskillondashboard(Single_Target, skillID) return 0 end

---Activates the E.G.O passive on a unit. Only works on equipped E.G.Os
---@param Multi_Target string --Modular's Multi-Target
---@param egoID integer --An EGO ID
function activateegopassive(Multi_Target, egoID) return end

---This should works with others timing beside SortAction. Set the index of current battle action (or called 'current skill') to newIndex
---@param newIndex integer --Any integer >= 0. If the value is higher than the last battle action index in current battle, set to the last index instead
function setactionindex(newIndex) return end

---skillsend but if it has more supports. Sends an attack from a unit to another
---@param Attacker string --Modular's Single-Target
---@param Targets string --Modular's Multi-Target
---@param skillOption SkillSendOption
---@param actionIndex? integer --Any integer >= 0 (Default = 0). The delay (measured in total skill uses by any unit) before this skill get sent / The skill order, 0 means this skill will be sent immediately or 99 means this skill will be the last skill in that turn regardless of the unit's speed
function betterskillsend(Attacker, Targets, skillOption, actionIndex) return end

---Return the Modular's script timing ID (an integer)
---@return TimingID
---@nodiscard
function gettimingid() return 0 end

---Apply an effect for skill slots. You can stack multiple effects on a slot, those effects will disappear when you swap over to defense skills, after using the skill and after turn end
---@param Multi_Target string --Modular's Multi-Target
---@param slotIndex integer --the skill slot index. Index starts at 0
---@param topOrBottom "Top" | "Bottom" --Default: Both top and bottom slot
---@param effectType EffectType
---@param isActive boolean --To set the effect is active or not
---@param alphaValue? integer --Any integer in range of 0 to 100. This value will be divided by 100
function applyskilleffect(Multi_Target, slotIndex, topOrBottom, effectType, isActive, alphaValue) return end

---Check if the slot has a skill effect. Returns 1 if true, 0 if false and -1 if something went wrong
---@param Single_Target string --Modular's Single-Target
---@param slotIndex integer --the skill slot index. Index starts at 0
---@param topOrBottom "Top" | "Bottom"
---@param effectType EffectType
---@return integer
---@nodiscard
function hasskilleffect(Single_Target, slotIndex, topOrBottom, effectType) return 0 end

---plays a specific motion (like leiheng reload on turn end)
---@param Single_Target string --Modular's Single-Target (Unit to play the motion)
---@param Motion MotionID
---@param MotionIndex? integer --the index of the skill motion to be played (Any integer >= 0 (Default = -1))
---@param HasZoom? string --any string (If this variable is added, adds a zoom to the unit)
function playmotion(Single_Target, Motion, MotionIndex, HasZoom) return end

---Increase the skill animation speed like how facade does. Only useable on visual timings like ChangeMotion or OnVisualUse else the effect will be applied on Combat Start. The Speed does not reset to 100 (default) after the affect is applied so others skills will keep the speed increase
---@param Single_Target string --Modular's Single-Target
---@param Speed integer --This value will be divided by 100
function changeanimspeed(Single_Target, Speed) return end
--#endregion

--#region Exclusive .lua functions

---Turn a string represent JSON into a .lua table
---@param string string
---@return table
---@nodiscard
function jsontolua(string) return {} end

---Lists the folders of a given directory. This function does not list files. This function is restricted to the Plugins folder and any sub-folders. This function returns a .lua table (array-like)
---@param path string --Path of the directory
---@return table
function listdirectories(path) return {} end

---Returns a .lua table (array-like) contain all the buffs' keyword the unit has
---@param Single_Target string --Modular's Single-Target
---@return table
function listbuffs(Single_Target) return {} end

---Set a global .lua value
---@param key string --The data ID
---@param luaValue any
function setgdata(key, luaValue) return end

---Returns a global .lua value
---@param key string --The data ID
---@return any
---@nodiscard
function getgdata(key) return end

---Clear all global datas
function clearallgdata() return end

---Uses with 'OnGainBuff' timing. Returns a .lua string representing the gained buff's keyword
---@return string
---@nodiscard
function gbkeyword() return "" end

---Returns a .lua string representing the current map's ID (the map which the game is using)
---@return string
---@nodiscard
function getcurrentmapid() return "" end

---Returns a .lua table (array-like) contains all the buffKeyword's mode
---@param buffKeyword string --any buff keyword
---@param mode ListRelatedKeywordsMode
---@return table
---@nodiscard
function listrelatedkeywords(buffKeyword, mode) return {} end

---Returns a .lua string representing the unit's appearance ID 
---@param Single_Target string --Modular's Single-Target
---@return string
---@nodiscard
function getappearanceid(Single_Target) return "" end

---Returns a .lua table (array-like) contains all stagger threshold values filtered by isActiveOnly
---@param Single_Target string --Modular's Single-Target
---@param isActiveOnly boolean --If true, only return values of active stagger thresholds
---@return table
---@nodiscard
function listbreakvalues(Single_Target, isActiveOnly) return {} end

---Returns a .lua dictionary with all egos data the unit has
---@param Single_Target string --Modular's Single-Target
---@return table
---@nodiscard
function listegoskillids(Single_Target) return {} end

---Only works on Identities. Returns a .lua table (array-like) contains all 'skillKeywordList' the Identity has
---@param Single_Target string --Modular's Single-Target
---@return table
---@nodiscard
function listskillkeywords(Single_Target) return {} end

---Returns a .lua table (array-like) represent the skill order in that turn based on the timing this function get called
---@return table
---@nodiscard
function listbattleactions() return {} end
--#endregion
