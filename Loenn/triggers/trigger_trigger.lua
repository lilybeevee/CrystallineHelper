local enums = require "consts.celeste_enums"

local activationTypes = {
    ["Flag (Default)"] = "Flag",
    ["Dashing"] = "Dashing",
    ["Dash Count"] = "DashCount",
    ["Deaths In Room"] = "DeathsInRoom",
    ["Deaths In Level"] = "DeathsInLevel",
    ["Holdable Grabbed"] = "GrabHoldable",
    ["Horizontal Speed"] = "SpeedX",
    ["Vertical Speed"] = "SpeedY",
    ["Jumping"] = "Jumping",
    ["Crouching"] = "Crouching",
    ["Time Since Player Moved"] = "TimeSinceMovement",
    ["Holdable Entered"] = "OnHoldableEnter",
    ["On Entity Touch"] = "OnEntityCollide",
    ["Core Mode"] = "CoreMode",
    ["On Interaction"] = "OnInteraction",
    ["Touched Solid"] = "OnSolid",
    ["Entity Entered"] = "OnEntityEnter"
}

local comparisonTypes = {
    "LessThan",
    "EqualTo",
    "GreaterThan"
}

local triggerTrigger = {}

triggerTrigger.name = "vitellary/triggertrigger"

triggerTrigger.nodeLimits = {1, -1}
triggerTrigger.nodeLineRenderType = "fan"

triggerTrigger.fieldInformation = {
    activationType = {
        editable = false,
        options = activationTypes
    },
    comparisonType = {
        editable = false,
        options = comparisonTypes
    },
    coreMode = {
        editable = false,
        options = enums.core_modes
    }
}

function triggerTrigger.ignoredFields(entity)
    local ignored = {
        "_id",
        "_name",
        "comparisonType",
        "absoluteValue",
        "talkBubbleX",
        "talkBubbleY",
        "flag",
        "deaths",
        "dashCount",
        "requiredSpeed",
        "timeToWait",
        "coreMode",
        "entityTypeToCollide",
        "collideCount",
        "solidType",
        "entityType"
    }

    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end

    local atype = entity.activationType or "Flag"
    local iscomparison = false

    if atype == "Flag" then
        doNotIgnore("flag")
    elseif atype == "DashCount" then
        doNotIgnore("dashCount")
        iscomparison = true
    elseif atype == "DeathsInRoom" or atype == "DeathsInLevel" then
        doNotIgnore("deaths")
        iscomparison = true
    elseif atype == "SpeedX" or atype == "SpeedY" then
        doNotIgnore("requiredSpeed")
        iscomparison = true
    elseif atype == "TimeSinceMovement" then
        doNotIgnore("timeToWait")
        iscomparison = true
    elseif atype == "CoreMode" then
        doNotIgnore("coreMode")
    elseif atype == "OnEntityCollide" then
        doNotIgnore("entityType")
        doNotIgnore("collideCount")
        iscomparison = true
    elseif atype == "OnInteraction" then
        doNotIgnore("talkBubbleX")
        doNotIgnore("talkBubbleY")
    elseif atype == "OnSolid" then
        doNotIgnore("solidType")
    elseif atype == "OnEntityEnter" then
        doNotIgnore("entityType")
    end

    if iscomparison then
        doNotIgnore("comparisonType")
        doNotIgnore("absoluteValue")
    end

    return ignored
end

triggerTrigger.placements = {}
for _, mode in pairs(activationTypes) do
    local placement = {
        name = string.lower(mode),
        data = {
            oneUse = false,
            flag = "",
            invertCondition = false,
            delay = 0.0,
            activateOnTransition = false,
            randomize = false,
            matchPosition = true,
            activationType = mode,
            comparisonType = "EqualTo",
            deaths = 0,
            dashCount = 0,
            requiredSpeed = 0.0,
            absoluteValue = false,
            timeToWait = 0.0,
            coreMode = "None",
            entityTypeToCollide = "Celeste.Strawberry",
            talkBubbleX = 0,
            talkBubbleY = 0,
            onlyOnEnter = false,
            collideCount = 1,
            solidType = "",
            entityType = ""
        }
    }
    table.insert(triggerTrigger.placements, placement)
end

return triggerTrigger
