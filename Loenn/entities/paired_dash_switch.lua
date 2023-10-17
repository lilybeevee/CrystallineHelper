local utils = require "utils"
local enums = require "consts.celeste_enums"
local drawableSprite = require "structs.drawable_sprite"

local pairedSwitch = {}

pairedSwitch.name = "vitellary/paireddashswitch"
pairedSwitch.placements = {}

for _,dir in ipairs(enums.dash_switch_sides) do
    table.insert(pairedSwitch.placements, {
        name = "pairedswitch_"..string.lower(dir),
        data = {
            direction = dir,
            groupId = "",
            flag = "",
            sprite = "dashSwitch_default",
            pressed = false,
            affectedByFlag = false,
        }
    })
end
pairedSwitch.fieldInformation = {
    direction = {
        editable = false,
        options = enums.dash_switch_sides,
    },
}
pairedSwitch.nodeLimits = {0, -1}
pairedSwitch.nodeLineRenderType = "fan"

pairedSwitch.justification = {0.5, 0.5}
function pairedSwitch.sprite(room, entity)
    local pressed = entity.pressed
    local sprite = drawableSprite.fromTexture("objects/temple/dashButton"..(pressed and "26" or "00"), entity)

    if entity.direction == "Right" then
        sprite.rotation = math.pi
        sprite:addPosition(0, 8)
        if pressed then sprite.x -= 6 end
    elseif entity.direction == "Down" then
        sprite.rotation = -math.pi/2
        sprite:addPosition(8, 0)
        if pressed then sprite.y -= 6 end
    elseif entity.direction == "Up" then
        sprite.rotation = math.pi/2
        sprite:addPosition(8, 8)
        if pressed then sprite.y += 6 end
    else
        sprite.rotation = 0
        sprite:addPosition(8, 8)
        if pressed then sprite.x += 6 end
    end

    return sprite
end

function pairedSwitch.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local w, h = 0, 0
    if entity.direction == "Right" then
        w, h = 8, 16
    elseif entity.direction == "Down" then
        w, h = 16, 8
    elseif entity.direction == "Up" then
        w, h = 16, 8
    else
        w, h = 8, 16
    end

    local nodes = entity.nodes or {{x = 0, y = 0}, {x = 0, y = 0}}
    local rects = {}
    for i, node in ipairs(nodes) do
        table.insert(rects, utils.rectangle(node.x, node.y, w, h))
    end
    return utils.rectangle(x, y, w, h), rects
end

return pairedSwitch