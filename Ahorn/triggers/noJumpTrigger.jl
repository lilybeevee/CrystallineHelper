module NoJumpTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/nojumptrigger" NoJump(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight)

const placements = Ahorn.PlacementDict(
    "No Jump Trigger (Crystalline)" => Ahorn.EntityPlacement(
        NoJump,
        "rectangle"
    )
)

end