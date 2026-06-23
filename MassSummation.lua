local function RemoveElem(t, key)
    for i = 1, #t, 1 do
        if t[i] == key then
            table.remove(t, i)
            return t
        end
    end
end

function BeforeUse()
    if getldata("Self", "getTargetListDone") ~= true then
        local everyTarget = selecttargets("EveryTarget")
        setldata("Self", "remainTargets", everyTarget)
        setldata("Self", "getTargetListDone", true)
    end

    removealltargetexceptmaintarget()
    local targetList = getldata("Self", "remainTargets")
    setmaintarget("Mid", targetList[1])
    targetList.remove(1)
    setldata("Self", "remainTargets", targetList)
end

function ClashWin()
    if getldata("Self", "massDone") ~= true then
        for i = 1, getcoincount("Self", "og"), 1 do
            removecoin("Self", i)
        end
    end
end

function ClashLose()
    if getldata("Self", "massDone") ~= true then

        for i = 1, getcoincount("Target", "og"), 1 do
            removecoin("MainTarget", i)
        end
    end
end

function EndBehaviour()
    if getldata("id1417592389", "usedTempSkill") ~= true then
        local atkDmgList = getldata("id1417592389", "massAtkDmgDict")
        local finalPower = getcurrentpower("Self")
        local comparisonIdSkillList = {}
        local originTargetList = getldata("id1417592389", "originTargetList")

        for instid, data in pairs(atkDmgList) do
            local comparisonResult = 0

            if data.IsOnDef == true then
                comparisonResult = -1
                skillsend(instid, instid, data.SkillId, "def")
            else
                if data.FinalPower > finalPower then
                    comparisonResult = 1 --sinner win
                    -- log("Win")
                    originTargetList = RemoveElem(originTargetList, instid)
                elseif data.FinalPower == finalPower then
                    comparisonResult = 0 --sinner draw
                    -- log("Draw")
                    originTargetList = RemoveElem(originTargetList, instid)
                else
                    comparisonResult = -1 --sinner lose
                    -- log("Lose")
                end
            end

            comparisonIdSkillList[instid] = {
                SkillId = data.SkillId,
                SkillType = data.SkillType,
                IsOnDef = data.IsOnDef,
                result = comparisonResult,
            }
            -- if (data.SkillType == 1 or data.SkillType == 2) or (data.SkillType) --HERE
        end
        setldata("id1417592389", "comparisonIdSkillList", comparisonIdSkillList)
        setldata("id1417592389", "usedTempSkill", true)
        setldata("id1417592389", "realTargetList", originTargetList)

        if #originTargetList == 0 then
            EndBehaviour()
            return
        end
        skillsend("id1417592389", originTargetList[1], getskillid())
        -- table.remove(originTargetList, 1)
    else
        local originTargetList = getldata("id1417592389", "originTargetList")
        local comparisonIdSkillList = getldata("id1417592389", "comparisonIdSkillList")
        for instid, data in pairs(comparisonIdSkillList) do
            if data.result == 1 then
                skillsend(instid, "id1317592389", data.SkillId)
            end
        end
        setldata("id1417592389","usedTempSkill", false)
        setldata("id1417592389", "comparisonIdSkillList", {})
        setldata("id1417592389", "massAtkDmgDict", {})
        setldata("id1417592389", "originTargetList", {})
        setldata("id1417592389", "realTargetList", {})
    end
end

function OSA()
    if getldata("id1417592389", "usedTempSkill") == true and getldata("id1417592389", "Phase2") == true then
        buff("EveryTarget", "FerventAdoration", 1, 0, 1)
    end
end
