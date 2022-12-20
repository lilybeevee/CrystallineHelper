module TimeCrystal

using ..Ahorn, Maple

@mapdef Entity "vitellary/timecrystal" Crystal(x::Integer, y::Integer,
	oneUse::Bool=false, stopLength::Number=2.0, respawnTime::Number=2.5,
	untilDash::Bool=false, immediate::Bool=false, entityTypesToIgnore::String="",
    timeScale::Number=0.0)

const placements = Ahorn.PlacementDict(
    "Time Crystal (Crystalline)" => Ahorn.EntityPlacement(
        Crystal,
		"point"
    ),
	"Time Crystal (Until Dash) (Crystalline)" => Ahorn.EntityPlacement(
        Crystal,
		"point",
		Dict{String, Any}(
			"untilDash" => true
        )
    )
)

Ahorn.editingOrder(entity::Crystal) = String["x", "y", "stopLength", "respawnTime", "timeScale",
    "entityTypesToIgnore", "oneUse", "immediate", "untilDash"]

function Ahorn.selection(entity::Crystal)
    x, y = Ahorn.position(entity)

    return Ahorn.getSpriteRectangle("objects/crystals/time/idle00.png", x, y, jx=0.5, jy=0.5)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Crystal)
	untilDash = get(entity.data, "untilDash", false)
	if untilDash
		Ahorn.drawSprite(ctx, "objects/crystals/time/untildash/idle00.png", 0, 0, jx=0.5, jy=0.5)
	else
		Ahorn.drawSprite(ctx, "objects/crystals/time/idle00.png", 0, 0, jx=0.5, jy=0.5)
	end
end

end