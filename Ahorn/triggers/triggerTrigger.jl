module VitellaryTriggerTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/triggertrigger" Ttrigger(x::Integer, y::Integer,
    width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    oneUse::Bool=false, flag::String="", invertCondition::Bool=false, delay::Number=0.0,
    activateOnTransition::Bool=false, randomize::Bool=false, matchPosition::Bool=true,
    activationType::String="Flag", comparisonType::String="EqualTo", deaths::Integer=0,
    dashCount::Integer=0, requiredSpeed::Number=0.0, absoluteValue::Bool=false, 
    timeToWait::Number=0.0, coreMode::String="none", entityTypeToCollide::String="Celeste.Strawberry",
    talkBubbleX::Integer=0, talkBubbleY::Integer=0, onlyOnEnter::Bool=false,
    collideCount::Integer=1, solidType::String="", entityType::String="")

const activationTypes = Dict{String, String}(
    "Flag (Default)" => "Flag",
    "Dashing" => "Dashing",
    "Dash Count" => "DashCount",
    "Deaths In Room" => "DeathsInRoom",
    "Deaths In Level" => "DeathsInLevel",
    "Holdable Grabbed" => "GrabHoldable",
    "Horizontal Speed" => "SpeedX",
    "Vertical Speed" => "SpeedY",
    "Jumping" => "Jumping",
    "Crouching" => "Crouching",
    "Time Since Player Moved" => "TimeSinceMovement",
    "Holdable Entered" => "OnHoldableEnter",
    "On Entity Touch" => "OnEntityCollide",
    "Core Mode" => "CoreMode",
    "On Interaction" => "OnInteraction",
    "Touched Solid" => "OnSolid",
    "Entity Entered" => "OnEntityEnter",
)

const comparisonTypes = ["LessThan", "EqualTo", "GreaterThan"]

const placements = Ahorn.PlacementDict(
    "Trigger Trigger ($name) (Crystalline)" => Ahorn.EntityPlacement(
        Ttrigger,
        "rectangle",
		Dict{String, Any}(
            "activationType" => "$acttype",
        ),
        function(trigger)
            trigger.data["nodes"] = [(Int(trigger.data["x"]) + Int(trigger.data["width"]) + 8, Int(trigger.data["y"]))]
            if trigger.data["activationType"] == "OnInteraction"
                trigger.data["talkBubbleX"] = div(Int(trigger.data["width"]), 2)
            end
        end
    ) for (name, acttype) in activationTypes
)

Ahorn.nodeLimits(entity::Ttrigger) = 1, -1

Ahorn.editingOptions(entity::Ttrigger) = Dict{String, Any}(
    "activationType" => activationTypes,
    "comparisonType" => comparisonTypes,
    "coreMode" => sort(Maple.core_modes)
)

function Ahorn.editingOrder(entity::Ttrigger)
    result = String["x", "y", "width", "height",
    "talkBubbleX", "talkBubbleY", "activationType", "delay",
    "flag", "comparisonType", "dashCount", "deaths",
    "requiredSpeed", "timeToWait", "coreMode", "entityTypeToCollide", "solidType", "entityType",
    "collideCount", "absoluteValue", "activateOnTransition", "invertCondition",
    "matchPosition", "onlyOnEnter", "oneUse", "randomize"]
    
    return result
end

function Ahorn.editingIgnored(entity::Ttrigger, multiple::Bool=false)
    atype = get(entity.data, "activationType", "Flag")
    result = String["comparisonType", "absoluteValue", "talkBubbleX", "talkBubbleY", "flag", "deaths",
        "dashCount", "requiredSpeed", "timeToWait", "coreMode", "entityTypeToCollide", "collideCount",
        "solidType", "entityType"]
    iscomparison = false
    
    if atype == "Flag"
        deleteat!(result, findall(x->x=="flag",result))
    elseif atype == "DashCount"
        deleteat!(result, findall(x->x=="dashCount",result))
        iscomparison = true
    elseif atype == "DeathsInRoom" || atype == "DeathsInLevel"
        deleteat!(result, findall(x->x=="deaths",result))
        iscomparison = true
    elseif atype == "SpeedX" || atype == "SpeedY"
        deleteat!(result, findall(x->x=="requiredSpeed",result))
        iscomparison = true
    elseif atype == "TimeSinceMovement"
        deleteat!(result, findall(x->x=="timeToWait",result))
        iscomparison = true
    elseif atype == "CoreMode"
        deleteat!(result, findall(x->x=="coreMode",result))
    elseif atype == "OnEntityCollide"
        deleteat!(result, findall(x->x=="entityType",result))
        deleteat!(result, findall(x->x=="collideCount",result))
        iscomparison = true
    elseif atype == "OnInteraction"
        deleteat!(result, findall(x->x=="talkBubbleX",result))
        deleteat!(result, findall(x->x=="talkBubbleY",result))
    elseif atype == "OnSolid"
        deleteat!(result, findall(x->x=="solidType",result))
    elseif atype == "OnEntityEnter"
        deleteat!(result, findall(x->x=="entityType",result))
    end
    
    if iscomparison
        deleteat!(result, findall(x->x=="comparisonType",result))
        deleteat!(result, findall(x->x=="absoluteValue",result))
    end
    
    if multiple
        insert!(result, "x", 1)
        insert!(result, "y", 2)
        insert!(result, "width", 3)
        insert!(result, "height", 4)       
    end
    
    return result
end

end