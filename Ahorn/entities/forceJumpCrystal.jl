module ForceJumpCrystal

using ..Ahorn, Maple

@mapdef Entity "vitellary/forcejumpcrystal" Crystal(x::Integer, y::Integer, oneUse::Bool=false,
    respawnTime::Number=2.5, holdJump::Bool=true)

const placements = Ahorn.PlacementDict(
    "Force Jump Crystal (Crystalline)" => Ahorn.EntityPlacement(
        Crystal
    )
)

function Ahorn.selection(entity::Crystal)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x-8, y-8, 16, 16)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Crystal)
    Ahorn.drawSprite(ctx, "objects/crystals/forcejump/idle00.png", -4, 0, jx=0.5, jy=0.5)
    Ahorn.drawSprite(ctx, "objects/crystals/forcejump/idle02.png", 4, 0, jx=0.5, jy=0.5, sx=-1)
end

end