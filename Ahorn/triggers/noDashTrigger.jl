module NoDashTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/nodashtrigger" NoDash(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight)

const placements = Ahorn.PlacementDict(
    "No Dash Trigger (Crystalline)" => Ahorn.EntityPlacement(
        NoDash,
        "rectangle"
    )
)

end