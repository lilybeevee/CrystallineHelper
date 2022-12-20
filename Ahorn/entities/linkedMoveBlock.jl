module VitMoveBlock

using ..Ahorn, Maple

@mapdef Entity "vitellary/vitmoveblock" Block(x::Integer, y::Integer, width::Integer=16, height::Integer=16,
	canSteer::Bool=false, direction::String="Right", remote::Integer=0, canActivate::Bool=true,
	spritePath::String="objects/vitMoveBlock", moveSpeed::Number=75.0, idleSingleColor::String="465EB5", idleLinkedColor::String="9E45B2",
	activeSingleColor::String="4FD6FF", activeLinkedColor::String="FF8CF5", breakingColor::String="CC2541")

Ahorn.editingOptions(entity::Block) = Dict{String, Any}(
    "direction" => Maple.move_block_directions
)
Ahorn.editingOrder(entity::Block) = String["x", "y", "width", "height", "direction", "remote", "moveSpeed", "spritePath", "canSteer", "canActivate"]
Ahorn.editingIgnored(entity::Block, multiple::Bool=false) = multiple ? String["x", "y", "width", "height", "direction"] : String[]
Ahorn.minimumSize(entity::Block) = 16, 16
Ahorn.resizable(entity::Block) = true, true

Ahorn.selection(entity::Block) = Ahorn.getEntityRectangle(entity)

arrows = Dict{String, String}(
    "up" => "objects/vitMoveBlock/arrow02",
    "left" => "objects/vitMoveBlock/arrow04",
    "right" => "objects/vitMoveBlock/arrow00",
    "down" => "objects/vitMoveBlock/arrow06",
)

button = "objects/vitMoveBlock/button_ahorn"

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Block, room::Maple.Room)
    x = Int(get(entity.data, "x", 0))
    y = Int(get(entity.data, "y", 0))

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    canSteer = get(entity.data, "canSteer", false)
    direction = lowercase(get(entity.data, "direction", "up"))
	remote = get(entity.data, "remote", 0)
    arrowSprite = Ahorn.getSprite(arrows[lowercase(direction)], "Gameplay")
	
	midColor = (17, 17, 22) ./ 255
	buttonColor = (70, 94, 181, 255) ./ 255
	highlightColor = (50, 71, 145) ./ 255
	if remote > 0
		buttonColor = (158, 69, 178, 255) ./ 255
		highlightColor = (113, 52, 142) ./ 255
	end

    frame = "objects/vitMoveBlock/base"
    if canSteer
        if direction == "up" || direction == "down"
            frame = "objects/vitMoveBlock/base_v"
        else
            frame = "objects/vitMoveBlock/base_h"
        end
    end

    Ahorn.drawRectangle(ctx, 2, 2, width - 4, height - 4, highlightColor, highlightColor)
    Ahorn.drawRectangle(ctx, 8, 8, width - 16, height - 16, midColor)

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, 0, 8, 0, 8, 8)
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, height - 8, 8, 16, 8, 8)

        if canSteer && (direction != "up" && direction != "down")
            Ahorn.drawImage(ctx, button, (i - 1) * 8, -2, 6, 0, 8, 6, tint=buttonColor)
        end
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, 0, (i - 1) * 8, 0, 8, 8, 8)
        Ahorn.drawImage(ctx, frame, width - 8, (i - 1) * 8, 16, 8, 8, 8)

        if canSteer && (direction == "up" || direction == "down")
            Ahorn.Cairo.save(ctx)

            Ahorn.rotate(ctx, -pi / 2)
            Ahorn.drawImage(ctx, button, i * 8 - height - 8, -2, 6, 0, 8, 6, tint=buttonColor)
            Ahorn.scale(ctx, 1, -1)
            Ahorn.drawImage(ctx, button, i * 8 - height - 8, -2 - width, 6, 0, 8, 6, tint=buttonColor)

            Ahorn.Cairo.restore(ctx)
        end
    end

    Ahorn.drawImage(ctx, frame, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, 0, 16, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, 0, height - 8, 0, 16, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, height - 8, 16, 16, 8, 8)

    if canSteer && (direction != "up" && direction != "down")
        Ahorn.Cairo.save(ctx)

        Ahorn.drawImage(ctx, button, 2, -2, 0, 0, 6, 6, tint=buttonColor)
        Ahorn.scale(ctx, -1, 1)
        Ahorn.drawImage(ctx, button, 2 - width, -2, 0, 0, 6, 6, tint=buttonColor)

        Ahorn.Cairo.restore(ctx)
    end

    if canSteer && (direction == "up" || direction == "down")
        Ahorn.Cairo.save(ctx)

        Ahorn.rotate(ctx, -pi / 2)
        Ahorn.drawImage(ctx, button, -height + 2, -2, 0, 0, 8, 6, tint=buttonColor)
        Ahorn.drawImage(ctx, button, -10, -2, 14, 0, 8, 6, tint=buttonColor)
        Ahorn.scale(ctx, 1, -1)
        Ahorn.drawImage(ctx, button, -height + 2, -2 -width, 0, 0, 8, 6, tint=buttonColor)
        Ahorn.drawImage(ctx, button, -10, -2 -width, 14, 0, 8, 6, tint=buttonColor)

        Ahorn.Cairo.restore(ctx)
    end

    Ahorn.drawRectangle(ctx, div(width - arrowSprite.width, 2) + 1, div(height - arrowSprite.height, 2) + 1, 8, 8, highlightColor, highlightColor)
    Ahorn.drawImage(ctx, arrowSprite, div(width - arrowSprite.width, 2), div(height - arrowSprite.height, 2))
end

end