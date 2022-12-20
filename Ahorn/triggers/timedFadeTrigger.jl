module VitellaryTimedFadeTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/timedfadetrigger" TimedTrigger(x::Integer, y::Integer,
    width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    time::Number=1.0)

const placements = Ahorn.PlacementDict(
    "Timed Fade Trigger (Crystalline)" => Ahorn.EntityPlacement(
        TimedTrigger,
        "rectangle",
        Dict{String, Any}(),
        function(trigger)
            trigger.data["nodes"] = [(Int(trigger.data["x"]) + Int(trigger.data["width"]) + 8, Int(trigger.data["y"]))]
        end
    )
)

Ahorn.nodeLimits(entity::TimedTrigger) = 1, 1

end