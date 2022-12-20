module StarCrystal

using ..Ahorn, Maple

@mapdef Entity "vitellary/starcrystal" Crystal(x::Integer, y::Integer, oneUse::Bool=false, time::Number=2.0,
changeDashes::Bool=true, changeInvuln::Bool=true, changeStamina::Bool=true, respawnTime::Number=2.5)

const placements = Ahorn.PlacementDict(
    "Star Crystal (Crystalline)" => Ahorn.EntityPlacement(
        Crystal
    )
)

Ahorn.editingOrder(entity::Crystal) = String["x", "y", "time", "respawnTime", "changeInvuln", "changeDashes", "changeStamina", "oneUse"]

function Ahorn.selection(entity::Crystal)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle("objects/crystals/star/idle00.png", x, y, jx=0.5, jy=0.5)
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Crystal) = Ahorn.drawSprite(ctx, "objects/crystals/star/idle00.png", 0, 0, jx=0.5, jy=0.5)

end