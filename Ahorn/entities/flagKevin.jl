module FlushelineFlagKevin

using ..Ahorn, Maple

@mapdef Entity "vitellary/flagkevin" FlagKevin(x::Integer, y::Integer, width::Integer=Maple.defaultBlockWidth, height::Integer=Maple.defaultBlockHeight, axes::String="horizontal", flagDirection::String="Right", flag::String="", customPath::String="crushblock", inverted::Bool = false, chillout::Bool = false, lavaSpeed::Number=1.0, playerCanHit::Bool = true, repeatWhileFlag::Bool = false, setFlagOnHit::Bool=false, crushSpeed::Number=240.0, returnSpeed::Number=60.0)

const placements = Ahorn.PlacementDict(
    "Flag-Activated Kevin (Reskinnable) (Crystalline)" => Ahorn.EntityPlacement(
        FlagKevin,
        "rectangle"
    ),
    "Flag-Activated Kevin (Reskinnable, Lava Aware) (Crystalline)" => Ahorn.EntityPlacement(
        FlagKevin,
        "rectangle",
        Dict{String, Any}(
            "lavaSpeed" => 0.5
        )
    )
)

Ahorn.editingOptions(entity::FlagKevin) = Dict{String, Any}(
    "axes" => Maple.kevin_axes,
    "flagDirection" => Maple.move_block_directions
)

Ahorn.editingOrder(entity::FlagKevin) = String["x", "y", "width", "height", "axes", "customPath", "flag", "flagDirection", "crushSpeed", "returnSpeed", "lavaSpeed", "chillout", "inverted", "playerCanHit", "repeatWhileFlag", "setFlagOnHit"]
Ahorn.minimumSize(entity::FlagKevin) = 24, 24
Ahorn.resizable(entity::FlagKevin) = true, true
Ahorn.selection(entity::FlagKevin) = Ahorn.getEntityRectangle(entity)

const rotations = Dict{String, Number}(
    "Up" => 0,
    "Right" => pi / 2,
    "Down" => pi,
    "Left" => pi * 3 / 2
)

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::FlagKevin)
    theta = rotations[entity.flagDirection] - pi / 2

    width = Int(get(entity.data, "width", 0))
    height = Int(get(entity.data, "height", 0))

    x, y = Ahorn.position(entity)
    cx, cy = x + floor(Int, width / 2), y + floor(Int, height / 2)

    Ahorn.drawArrow(ctx, cx, cy, cx + cos(theta) * 24, cy + sin(theta) * 24, Ahorn.colors.selection_selected_fc, headLength=6)
end


function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FlagKevin, room::Maple.Room)
    axes = lowercase(get(entity.data, "axes", "horizontal"))
    chillout = get(entity.data, "chillout", false)

    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    giant = height >= 48 && width >= 48 && chillout
    face = giant ? "objects/" * get(entity.data, "customPath", "crushblock") * "/giant_block00" : "objects/" * get(entity.data, "customPath", "crushblock") * "/idle_face"
    faceSprite = Ahorn.getSprite(face, "Gameplay")

    if lowercase(axes) == "none"
        frame = "objects/" * get(entity.data, "customPath", "crushblock") * "/block00"
    elseif lowercase(axes) == "horizontal"
        frame = "objects/" * get(entity.data, "customPath", "crushblock") * "/block01"
    elseif lowercase(axes) == "vertical"
        frame = "objects/" * get(entity.data, "customPath", "crushblock") * "/block02"
    elseif lowercase(axes) == "both"
        frame = "objects/" * get(entity.data, "customPath", "crushblock") * "/block03"
    end


    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)
        Ahorn.drawRectangle(ctx, 2, 2, width - 4, height - 4, Ahorn.Kevin.kevinColor)
    Ahorn.drawImage(ctx, faceSprite, div(width - faceSprite.width, 2), div(height - faceSprite.height, 2))

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, 0, 8, 0, 8, 8)
        Ahorn.drawImage(ctx, frame, (i - 1) * 8, height - 8, 8, 24, 8, 8)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, 0, (i - 1) * 8, 0, 8, 8, 8)
        Ahorn.drawImage(ctx, frame, width - 8, (i - 1) * 8, 24, 8, 8, 8)
    end

    Ahorn.drawImage(ctx, frame, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, 0, 24, 0, 8, 8)
    Ahorn.drawImage(ctx, frame, 0, height - 8, 0, 24, 8, 8)
    Ahorn.drawImage(ctx, frame, width - 8, height - 8, 24, 24, 8, 8)
end

function Ahorn.rotated(entity::FlagKevin, steps::Int)
    while steps > 0
        if entity.flagDirection == "Left"
            entity.flagDirection = "Up"
        elseif entity.flagDirection == "Up"
            entity.flagDirection = "Right"
        elseif entity.flagDirection == "Right"
            entity.flagDirection = "Down"
        elseif entity.flagDirection == "Down"
            entity.flagDirection = "Left"
        end
        steps -= 1
    end
    while steps < 0
        if entity.flagDirection == "Left"
            entity.flagDirection = "Down"
        elseif entity.flagDirection == "Down"
            entity.flagDirection = "Right"
        elseif entity.flagDirection == "Right"
            entity.flagDirection = "Up"
        elseif entity.flagDirection == "Up"
            entity.flagDirection = "Left"
        end
        steps += 1
    end
    return entity
end

end