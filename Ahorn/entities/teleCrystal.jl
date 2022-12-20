module TeleCrystal

using ..Ahorn, Maple

@mapdef Entity "vitellary/telecrystal" Crystal(x::Integer, y::Integer, direction::Int=0, oneUse::Bool=false)
@mapdef Entity "vitellary/goodtelecrystal" NewCrystal(x::Integer, y::Integer, direction::String="Right", oneUse::Bool=false, preventCrash::Bool=true, respawnTime::Number=0.2)

const placements = Ahorn.PlacementDict(
    "Tele Crystal (Right) (Crystalline)" => Ahorn.EntityPlacement(
        NewCrystal,
        "point",
		Dict{String, Any}(
            "direction" => "Right"
        )
    ),

    "Tele Crystal (Down) (Crystalline)" => Ahorn.EntityPlacement(
        NewCrystal,
		"point",
		Dict{String, Any}(
            "direction" => "Down"
        )
    ),

    "Tele Crystal (Left) (Crystalline)" => Ahorn.EntityPlacement(
        NewCrystal,
		"point",
		Dict{String, Any}(
            "direction" => "Left"
        )
    ),

    "Tele Crystal (Up) (Crystalline)" => Ahorn.EntityPlacement(
        NewCrystal,
		"point",
		Dict{String, Any}(
            "direction" => "Up"
        )
    )
)

spriteRight = "objects/crystals/tele/right/idle00.png"
spriteDown = "objects/crystals/tele/down/idle00.png"
spriteLeft = "objects/crystals/tele/left/idle00.png"
spriteUp = "objects/crystals/tele/up/idle00.png"

function getSprite(entity::Crystal)
    direction = get(entity.data, "direction", 3)

    if direction == 0
        return spriteRight
    elseif direction == 1
        return spriteDown
    elseif direction == 2
        return spriteLeft
    else
        return spriteUp
    end
end

function getSprite(entity::NewCrystal)
    direction = get(entity.data, "direction", "Up")

    if direction == "Right"
        return spriteRight
    elseif direction == "Down"
        return spriteDown
    elseif direction == "Left"
        return spriteLeft
    else
        return spriteUp
    end
end

Ahorn.editingOptions(entity::NewCrystal) = Dict{String, Any}(
    "direction" => ["Right", "Down", "Left", "Up"]
)

function Ahorn.selection(entity::Crystal)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle("objects/crystals/tele/up/idle00.png", x, y, jx=0.5, jy=0.5)
end

function Ahorn.selection(entity::NewCrystal)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle("objects/crystals/tele/up/idle00.png", x, y, jx=0.5, jy=0.5)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Crystal, room::Maple.Room)
    sprite = getSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::NewCrystal, room::Maple.Room)
    sprite = getSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end