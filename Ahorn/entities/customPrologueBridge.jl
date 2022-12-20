module CustomIntroBridge

using ..Ahorn, Maple

@mapdef Entity "vitellary/customprologuebridge" Bridge(x::Integer, y::Integer, width::Integer=8, flag::String="", activationID::String="", activationIndex::Integer=0, left::Bool=false, delay::Number=0.2, speed::Number=0.8)

const placements = Ahorn.PlacementDict(
    "Custom Prologue Bridge (Crystalline)" => Ahorn.EntityPlacement(
        Bridge,
		"rectangle"
    )
)

Ahorn.editingOrder(entity::Bridge) = String["x", "y", "width", "activationID", "activationIndex", "delay", "speed", "flag", "left"]
Ahorn.editingIgnored(entity::Bridge, multiple::Bool=false) = multiple ? String["x", "y", "width", "activationIndex", "left"] : String[]
Ahorn.minimumSize(entity::Bridge) = 8, 8
Ahorn.resizable(entity::Bridge) = true, false

function Ahorn.selection(entity::Bridge)
    x, y = Ahorn.position(entity)
	width = Int(get(entity.data, "width", 8))

    return Ahorn.Rectangle(x, y, width, 8)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Bridge)
	width = Int(get(entity.data, "width", 8))
	for i = 0:(width / 8) - 1
		Ahorn.drawSprite(ctx, "objects/customBridge/tile" * string(Int(i % 6 + 3)) * ".png", i * 8, 0, jx=0, jy=0)
	end
end

end