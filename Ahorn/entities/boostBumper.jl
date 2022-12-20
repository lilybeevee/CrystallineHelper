module BoostBumper

using ..Ahorn, Maple

@mapdef Entity "vitellary/boostbumper" Booster(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Bumper Booster (Crystalline)" => Ahorn.EntityPlacement(
        Booster
    )
)

function Ahorn.selection(entity::Booster)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x-9, y-9, 18, 18)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Booster) = Ahorn.drawSprite(ctx, "objects/boostBumper/booster00.png", 0, 0, jx=0.5, jy=0.5)

end