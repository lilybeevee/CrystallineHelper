module CancelTimeCrystal

using ..Ahorn, Maple

@mapdef Trigger "vitellary/canceltimecrystaltrigger" CancelTimestop(x::Integer, y::Integer,
width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight)

const placements = Ahorn.PlacementDict(
    "Cancel Time Crystal Trigger (Crystalline)" => Ahorn.EntityPlacement(
        CancelTimestop,
        "rectangle"
    )
)

end