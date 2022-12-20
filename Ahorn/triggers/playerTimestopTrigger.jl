module NoMoveTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/nomovetrigger" NoMove(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, stopLength::Number=2.0)

const placements = Ahorn.PlacementDict(
    "Player Timestop Trigger (Crystalline)" => Ahorn.EntityPlacement(
        NoMove,
        "rectangle"
    )
)

end