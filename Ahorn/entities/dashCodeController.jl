module VitellaryDashSequenceController

using ..Ahorn, Maple

@mapdef Entity "vitellary/dashcodecontroller" DashController(x::Integer, y::Integer,
    dashCode::String="", flagLabel::String="", flagOnFailure::String="", index::Integer=0)

const placements = Ahorn.PlacementDict(
    "Dash Code Flag Sequence Controller (Crystalline)" => Ahorn.EntityPlacement(
        DashController
    )
)

function Ahorn.selection(entity::DashController)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle("ahorn_dashcodecontroller", x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DashController)
    Ahorn.drawSprite(ctx, "ahorn_dashcodecontroller", 0, 0)
end

end