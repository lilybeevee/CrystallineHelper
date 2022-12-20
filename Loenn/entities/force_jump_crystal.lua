local utils = require "utils"
local drawableSprite = require "structs.drawable_sprite"

local forceJumpCrystal = {}

forceJumpCrystal.name = "vitellary/forcejumpcrystal"
forceJumpCrystal.depth = -100

forceJumpCrystal.fieldInformation = {
    respawnTime = {
        minimumValue = 0.0
    }
}

forceJumpCrystal.placements = {
    {
        name = "force_jump_crystal",
        data = {
            oneUse = false,
            respawnTime = 2.5,
            holdJump = true
        }
    }
}

function forceJumpCrystal.sprite(room, entity)
    local left = drawableSprite.fromTexture("objects/crystals/forcejump/idle00", entity)
    left:addPosition(-4, 0)

    local right = drawableSprite.fromTexture("objects/crystals/forcejump/idle00", entity)
    right:addPosition(4, 0)
    right:setScale(-1, 1)

    return {left, right}
end

function forceJumpCrystal.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 8, y - 8, 16, 16)
end

return forceJumpCrystal
