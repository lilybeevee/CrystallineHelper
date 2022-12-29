module SetFlagSequenceController

using ..Ahorn, Maple

@mapdef Entity "vitellary/flagsequencecontroller" Controller(x::Integer, y::Integer,
    prefix::String="", state::Bool=false, startNumber::Integer=1, endNumber::Integer=99, onlyOnRespawn::Bool=false)

const placements = Ahorn.PlacementDict(
    "Set Flag Sequence On Spawn Controller (Crystalline)" => Ahorn.EntityPlacement(
        Controller
    )
)

function Ahorn.selection(entity::Controller)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 12, y - 12, 24, 24)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Controller, room::Maple.Room) = Ahorn.drawSprite(ctx, "ahorn_flagsequencecontroller.png", 0, 0)

Ahorn.editingOrder(entity::Controller) = String["x", "y", "startNumber", "endNumber", "prefix", "state"]

end