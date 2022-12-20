local utils = require "utils"
local drawableSprite = require "structs.drawable_sprite"

local directions = {
    ["Right"] = "Right",
    ["Down Right"] = "Downright",
    ["Down"] = "Down",
    ["Down Left"] = "Downleft",
    ["Left"] = "Left",
    ["Up Left"] = "Upleft",
    ["Up"] = "Up",
    ["Up Right"] = "Upright",
    ["None"] = "None"
}

local forceDashCrystal = {}

forceDashCrystal.name = "vitellary/forcedashcrystal"
forceDashCrystal.depth = -100

forceDashCrystal.fieldInformation = {
    respawnTime = {
        minimumValue = 0.0
    },
    direction = {
        editable = false,
        options = directions
    }
}

forceDashCrystal.placements = {}
for _, dir in pairs(directions) do
    local placement = {
        name = string.lower(dir),
        data = {
            oneUse = false,
            direction = dir,
            respawnTime = 2.5,
            needDash = false
        }
    }

    table.insert(forceDashCrystal.placements, placement)
end

local crystalSpriteData = {
    Right = {
        ortho = true,
        rotation = 0.5 * math.pi
    },
    Downright = {
        ortho = false,
        rotation = 0.5 * math.pi
    },
    Down = {
        ortho = true,
        rotation = math.pi
    },
    Downleft = {
        ortho = false,
        rotation = math.pi
    },
    Left = {
        ortho = true,
        rotation = -0.5 * math.pi
    },
    Upleft = {
        ortho = false,
        rotation = -0.5 * math.pi
    },
    Up = {
        ortho = true,
        rotation = 0
    },
    Upright = {
        ortho = false,
        rotation = 0
    },
    None = {
        ortho = false,
        rotation = 0
    }
}

local crystalOffsets = {
    ortho = {
        {x = 0, y = -2},
        {x = -4, y = 2},
        {x = 4, y = 2}
    },
    diag = {
        {x = 2, y = -2},
        {x = -3, y = -1},
        {x = 1, y = 3}
    },
    none = {
        {x = 2, y = -2, r = 0},
        {x = 2, y = -2, r = 0.5 * math.pi},
        {x = 2, y = -2, r = math.pi},
        {x = 2, y = -2, r = -0.5 * math.pi}
    }
}

function forceDashCrystal.sprite(room, entity)
    local direction = entity.direction or "Right"
    local noDirection = direction == "None"

    local data = crystalSpriteData[direction]

    local appearance = entity.needDash and "needdash" or "dashless"
    local orientation = data.ortho and "ortho" or "diag"
    local texture = string.format("objects/crystals/forcedash/%s/%s/idle00", appearance, orientation)

    local rotation = data.rotation or 0

    local sprites = {}

    local offsets = noDirection and crystalOffsets.none or data.ortho and crystalOffsets.ortho or crystalOffsets.diag
    for _, o in ipairs(offsets) do
        local sprite = drawableSprite.fromTexture(texture, entity)
        sprite.rotation = o.r or rotation
        local cos, sin = math.cos(sprite.rotation), math.sin(sprite.rotation)

        local ox = o.x * cos - o.y * sin
        local oy = o.y * cos + o.x * sin
        sprite:addPosition(ox, oy)

        table.insert(sprites, sprite)
    end

    return sprites
end

function forceDashCrystal.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

return forceDashCrystal
