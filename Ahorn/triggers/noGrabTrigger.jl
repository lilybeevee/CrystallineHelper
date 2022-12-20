module NoGrabTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/nograbtrigger" NoGrab(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight)

const placements = Ahorn.PlacementDict(
    "No Grab Trigger (Crystalline)" => Ahorn.EntityPlacement(
        NoGrab,
        "rectangle"
    )
)

end