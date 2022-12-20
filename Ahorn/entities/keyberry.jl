module KeyBerry

using ..Ahorn, Maple

@mapdef Entity "vitellary/keyberry" Berry(x::Integer, y::Integer, winged::Bool=false)
@mapdef Entity "vitellary/returnkeyberry" ReturnBerry(x::Integer, y::Integer, winged::Bool=false)

const placements = Ahorn.PlacementDict(
	"Keyberry (Crystalline)" => Ahorn.EntityPlacement(
		Berry,
		"point",
		Dict{String, Any}(
			"winged" => false
		)
	),
	"Keyberry (Winged) (Crystalline)" => Ahorn.EntityPlacement(
		Berry,
		"point",
		Dict{String, Any}(
			"winged" => true
		)
	),
	"Keyberry With Return (Crystalline)" => Ahorn.EntityPlacement(
		ReturnBerry,
		"point",
		Dict{String, Any}(
			"winged" => false
		),
		function(entity::ReturnBerry)
            entity.data["nodes"] = [
                (Int(entity.data["x"]) + 32, Int(entity.data["y"])),
                (Int(entity.data["x"]) + 64, Int(entity.data["y"]))
            ]
        end
	),
	"Keyberry With Return (Winged) (Crystalline)" => Ahorn.EntityPlacement(
		ReturnBerry,
		"point",
		Dict{String, Any}(
			"winged" => true
		),
		function(entity::ReturnBerry)
            entity.data["nodes"] = [
                (Int(entity.data["x"]) + 32, Int(entity.data["y"])),
                (Int(entity.data["x"]) + 64, Int(entity.data["y"]))
            ]
        end
	)
)

sprite = "collectables/keyberry/normal03"
spriteWinged = "collectables/keyberry/wings00"
spriteSeed = "collectables/keyberry/seed00"

Ahorn.nodeLimits(entity::Berry) = 0, -1
Ahorn.nodeLimits(entity::ReturnBerry) = 2, -1

function Ahorn.selection(entity::Berry)
	x, y = Ahorn.position(entity)
	
	res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(sprite, x, y)]
	
	nodes = get(entity.data, "nodes", ())
	for node in nodes
		nx, ny = Int.(node)
		
		push!(res, Ahorn.getSpriteRectangle(spriteSeed, nx, ny))
	end
	
	return res
end

function Ahorn.selection(entity::ReturnBerry)
	x, y = Ahorn.position(entity)
	
	res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(sprite, x, y)]
	
	nodes = get(entity.data, "nodes", ())
	nodelength = length(nodes)
	for (i, node) in enumerate(nodes)
		nx, ny = Int.(node)
		
		if i > nodelength - 2
			push!(res, Ahorn.Rectangle(nx - 12, ny - 12, 24, 24))
		else
			push!(res, Ahorn.getSpriteRectangle(spriteSeed, nx, ny))
		end
	end
	
	return res
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::Berry)
	x, y = Ahorn.position(entity)

	nodes = get(entity.data, "nodes", ())
	for node in nodes
		nx, ny = Int.(node)

		theta = atan(y - ny, x - nx)
		Ahorn.drawArrow(ctx, x, y, nx + cos(theta) * 8, ny + sin(theta) * 8, Ahorn.colors.selection_selected_fc, headLength=6)
		Ahorn.drawSprite(ctx, spriteSeed, nx, ny)
	end
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::ReturnBerry)
	x, y = Ahorn.position(entity)

	nodes = get(entity.data, "nodes", ())
	nodelength = length(nodes)
	for (i, node) in enumerate(nodes)
		nx, ny = Int.(node)

		theta = atan(y - ny, x - nx)
		if i == nodelength
			px, py = Int.(nodes[nodelength - 1])
			bubbleTheta = atan(py - ny, px - nx)
			Ahorn.drawArrow(ctx, px, py, nx + cos(bubbleTheta) * 8, ny + sin(bubbleTheta) * 8, Ahorn.colors.selection_selected_fc, headLength=6)
		else
			Ahorn.drawArrow(ctx, x, y, nx + cos(theta) * 8, ny + sin(theta) * 8, Ahorn.colors.selection_selected_fc, headLength=6)
		end
		if i > nodelength - 2
			Ahorn.Cairo.save(ctx)

			Ahorn.set_antialias(ctx, 1)
			Ahorn.set_line_width(ctx, 1)

			Ahorn.drawCircle(ctx, nx, ny, 12, (1.0, 1.0, 1.0, 1.0))

			Ahorn.Cairo.restore(ctx)
		else
			Ahorn.drawSprite(ctx, spriteSeed, nx, ny)
		end
	end
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::Berry, room::Maple.Room)
	x, y = Ahorn.position(entity)
	
	if get(entity.data, "winged", false)
		Ahorn.drawSprite(ctx, spriteWinged, x, y+1)
	else
		Ahorn.drawSprite(ctx, sprite, x, y)
	end
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::ReturnBerry, room::Maple.Room)
	x, y = Ahorn.position(entity)
	
	if get(entity.data, "winged", false)
		Ahorn.drawSprite(ctx, spriteWinged, x, y+1)
	else
		Ahorn.drawSprite(ctx, sprite, x, y)
	end
end

end