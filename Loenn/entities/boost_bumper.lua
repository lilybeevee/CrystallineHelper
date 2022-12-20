local utils = require "utils"

local boostBumper = {}

boostBumper.name = "vitellary/boostbumper"
boostBumper.depth = 0

boostBumper.nodeLimits = {0, 1}
boostBumper.nodeLineRenderType = "line"

boostBumper.placements = {
    name = "boost_bumper"
}

boostBumper.texture = "objects/boostBumper/booster00"

function boostBumper.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 9, y - 9, 18, 18)
end

return boostBumper