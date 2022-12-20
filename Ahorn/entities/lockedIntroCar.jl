module LockedIntroCar

using ..Ahorn, Maple

@mapdef Entity "vitellary/lockedintrocar" IntroCar(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Locked Intro Car (Crystalline)" => Ahorn.EntityPlacement(
        IntroCar
    )
)

bodySprite = "scenery/car/body"
wheelsSprite = "scenery/car/wheels"

function Ahorn.selection(entity::IntroCar)
    x, y = Ahorn.position(entity)

    rectangles = Ahorn.Rectangle[
        Ahorn.getSpriteRectangle(bodySprite, x, y, jx=0.5, jy=1.0),
        Ahorn.getSpriteRectangle(wheelsSprite, x, y, jx=0.5, jy=1.0),
    ]

    return Ahorn.coverRectangles(rectangles)
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::IntroCar, room::Maple.Room)
    x, y = Ahorn.position(entity)

    Ahorn.drawSprite(ctx, wheelsSprite, x, y, jx=0.5, jy=1.0)
    Ahorn.drawSprite(ctx, bodySprite, x, y, jx=0.5, jy=1.0)
end

end