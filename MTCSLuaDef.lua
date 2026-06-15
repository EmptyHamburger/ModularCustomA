---@meta

--[[
    MTCustomScripts Version: v22.95.4
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
--#endregion

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

function removeskill(Multi_Target, Multi_Skill) return end

function hasskill(Single_Target, SkillID) return 0 end

function getskilldata(Single_Target, Single_Skill, DataType, Var) return 0 end

function addcoin(Multi_Target, Multi_Skill, CoinIndex, VAR_4, VAR_5, VAR_6, CopyStaticData) return end

function getcurrentpower(Target) return 0 end

function clearskillabilities(Target) return end

function clearcoinabilities(Target, ...) return end

function addskillability(Multi_Target, Multi_Skill, skillAbilityName, skillScriptName, turnLimit) return end

function addcoinability(Target, coinScriptName, ...) return end

function removealltargetexceptmaintarget() return end

function setmaintarget(Mid, Single_Target) return end

function setmaintarget(Pre, Attackers, Target, SkillID, Count) return end

function modifysubtarget(Mode, includeTargets, excludeTargets) return end

function setspusage(newValue, add) return end

function setlevel(Multi_Target, newLevel) return end

function setmaxhp(Multi_Target, newMaxHp) return end

function getdefaultmaxhp(Single_Target) return 0 end

function gethpincrement(Single_Target) return 0 end

function getmtdata(Single_Target, DataID, DataSource) return 0 end

function setmtdata(Single_Target, DataID, DataValue, DataSource) return end

function getchangespvalue(Single_Target, TargetValue) return 0 end

function getuptielevel(Single_Target) return 0 end

function getskillslotindex() return 0 end

function issinnerfielded(characterID) return 0 end

function addkeyword(Multi_Target, keywordName, isAssociation) return end

function clearalltempskillabilities(Multi_Target) return end

function replaceallaffinity(Multi_Target, sinType, includeEgo) return end

function replaceskillondashboard(Single_Target, column, topOrBottom, skillID) return end

function upgradeskillondashboard(Single_Target, searchID, replaceID) return end

function addduel(unit_1, unit_2, skillIDforUnit_1, skillIDforUnit_2) return end

function getworldpos(Single_Target, cordType) return 0 end

function setworldpos(Multi_Target, x, y, z) return end

function setworldpos(Multi_Target, Sequences, loopCount, loopType) return end

function isreusedskill(Target) return 0 end

function isunitpartofreson(Single_Target, sinType) return 0 end

function hasskillondashboard(Single_Target, skillID) return 0 end

function activateegopassive(Multi_Target, egoID) return end

function setactionindex(newIndex) return end

function betterskillsend(Single_Target, Multi_Target, skillOption, actionIndex) return end

function gettimingid() return 0 end
--#endregion

--#region Exclusive .lua functions
function jsontolua(string) return "" end

function listdirectories(path) return {} end

function listbuffs(Single_Target) return {} end

function setgdata(key, luaValue) return end

---@return any
function getgdata(key) return end

function clearallgdata() return end

function gbkeyword() return "" end

function getcurrentmapid() return "" end

function listrelatedkeywords(buffKeyword, mode) return {} end

function getappearanceid(Single_Target) return "" end

function listbreakvalues(Single_Target, isActiveOnly) return {} end

function listegoskillids(Single_Target) return {} end

function listskillkeywords(Single_Target) return {} end

function listbattleactions() return {} end
--#endregion
