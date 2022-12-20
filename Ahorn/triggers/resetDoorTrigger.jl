module ResetDoorTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/resetdoortrigger" Remote(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, oneUse::Bool=false, animate::Bool=true, onlyInRoom::Bool=false)

const placements = Ahorn.PlacementDict(
    "Reset Door Trigger (Crystalline)" => Ahorn.EntityPlacement(
        Remote,
        "rectangle"
    )
)

end