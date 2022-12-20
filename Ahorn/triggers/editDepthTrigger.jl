module VitellaryEditDepthTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/editdepthtrigger" NewDepth(x::Integer, y::Integer,
    width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    depth::Integer=-9000, entitiesToAffect::String="Celeste.MoveBlock", debug::Bool=false,
    updateOnEntry::Bool=false)

const placements = Ahorn.PlacementDict(
    "Edit Depth Trigger (Crystalline)" => Ahorn.EntityPlacement(
        NewDepth,
        "rectangle"
    )
)

end