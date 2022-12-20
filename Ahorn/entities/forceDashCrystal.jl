module ForceDashCrystal

using ..Ahorn, Maple

@mapdef Entity "vitellary/forcedashcrystal" Crystal(x::Integer, y::Integer, oneUse::Bool=false,
    direction::String="Right", respawnTime::Number=2.5, needDash::Bool=false)

const directions = ["Right", "Downright", "Down", "Downleft", "Left", "Upleft", "Up", "Upright", "None"]

const directionIsOrtho = Dict{String, Bool}(
	"Right" => true,
	"Downright" => false,
	"Down" => true,
	"Downleft" => false,
	"Left" => true,
	"Upleft" => false,
	"Up" => true,
	"Upright" => false,
    "None" => false
)

const rotations = Dict{String, Number}(
	"Right" => pi * 0.5,
	"Downright" => pi * 0.5,
	"Down" => pi,
	"Downleft" => pi,
	"Left" => -pi * 0.5,
	"Upleft" => -pi * 0.5,
	"Up" => 0,
	"Upright" => 0
)

const placements = Ahorn.PlacementDict(
    "Force Dash Crystal ($(dir)) (Crystalline)" => Ahorn.EntityPlacement(
        Crystal,
		"point",
		Dict{String, Any}(
            "direction" => dir
        )
    ) for dir in directions
)

Ahorn.editingOptions(entity::Crystal) = Dict{String, Any}(
    "direction" => directions
)

function Ahorn.selection(entity::Crystal)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x-8, y-8, 16, 16)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Crystal)
	dir = get(entity.data, "direction", "Right")
    needdash = get(entity.data, "needDash", false)
    crystalType = needdash ? "needdash" : "dashless"
    sprite = "objects/crystals/forcedash/" * crystalType * "/" * (directionIsOrtho[dir] ? "ortho" : "diag") * "/idle00.png"
	
    if dir == "None"
        Ahorn.Cairo.save(ctx)
        
        Ahorn.drawSprite(ctx, sprite, 2, -2, jx=0.5, jy=0.5)
        
        Ahorn.Cairo.rotate(ctx, pi * 0.5)
        Ahorn.drawSprite(ctx, sprite, 2, -2, jx=0.5, jy=0.5)
        
        Ahorn.Cairo.rotate(ctx, pi * 0.5)
        Ahorn.drawSprite(ctx, sprite, 2, -2, jx=0.5, jy=0.5)
        
        Ahorn.Cairo.rotate(ctx, pi * 0.5)
        Ahorn.drawSprite(ctx, sprite, 2, -2, jx=0.5, jy=0.5)
            
        Ahorn.Cairo.restore(ctx)
    else
        Ahorn.Cairo.save(ctx)
        Ahorn.Cairo.rotate(ctx, rotations[dir])
        if directionIsOrtho[dir]
            Ahorn.drawSprite(ctx, sprite, 0, -2, jx=0.5, jy=0.5)
            Ahorn.drawSprite(ctx, sprite, 4, 2, jx=0.5, jy=0.5)
            Ahorn.drawSprite(ctx, sprite, -4, 2, jx=0.5, jy=0.5)
        else
            Ahorn.drawSprite(ctx, sprite, 2, -2, jx=0.5, jy=0.5)
            Ahorn.drawSprite(ctx, sprite, -3, -1, jx=0.5, jy=0.5)
            Ahorn.drawSprite(ctx, sprite, 1, 3, jx=0.5, jy=0.5)
        end
        Ahorn.Cairo.restore(ctx)
    end
end

end