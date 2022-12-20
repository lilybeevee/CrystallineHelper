local drawableSprite = require "structs.drawable_sprite"
local utils = require "utils"

local lockedIntroCar = {}

local bodyTexture = "scenery/car/body"
local wheelsTexture = "scenery/car/wheels"

lockedIntroCar.name = "vitellary/lockedintrocar"

lockedIntroCar.placements = {
    {
        name = "locked_intro_car",
        data = {}
    }
}

function lockedIntroCar.sprite(room, entity)
    local sprites = {}

    local bodySprite = drawableSprite.fromTexture(bodyTexture, entity)
    bodySprite:setJustification(0.5, 1.0)
    bodySprite.depth = 1

    local wheelSprite = drawableSprite.fromTexture(wheelsTexture, entity)
    wheelSprite:setJustification(0.5, 1.0)
    wheelSprite.depth = 3

    table.insert(sprites, bodySprite)
    table.insert(sprites, wheelSprite)

    return sprites
end

function lockedIntroCar.selection(room, entity)
    local bodySprite = drawableSprite.fromTexture(bodyTexture, entity)
    local wheelSprite = drawableSprite.fromTexture(wheelsTexture, entity)

    bodySprite:setJustification(0.5, 1.0)
    wheelSprite:setJustification(0.5, 1.0)

    return utils.rectangle(utils.coverRectangles({bodySprite:getRectangle(), wheelSprite:getRectangle()}))
end

return lockedIntroCar
