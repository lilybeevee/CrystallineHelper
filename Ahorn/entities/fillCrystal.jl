module FillCrystal

using ..Ahorn, Maple

@mapdef Entity "vitellary/fillcrystal" Crystal(x::Integer, y::Integer, oneUse::Bool=false, respawnTime::Number=2.5)

const placements = Ahorn.PlacementDict(
    "Fill Crystal (Crystalline)" => Ahorn.EntityPlacement(
        Crystal
    )
)

function Ahorn.selection(entity::Crystal)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle("objects/crystals/fill/idle00.png", x, y, jx=0.5, jy=0.5)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Crystal) = Ahorn.drawSprite(ctx, "objects/crystals/fill/idle00.png", 0, 0, jx=0.5, jy=0.5)

end