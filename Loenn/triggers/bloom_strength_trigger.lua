local enums = require "consts.celeste_enums"

local bloomStrengthTrigger = {}

bloomStrengthTrigger.name = "vitellary/bloomstrengthtrigger"

bloomStrengthTrigger.fieldInformation = {
    positionMode = {
        editable = false,
        options = enums.trigger_position_modes
    }
}

bloomStrengthTrigger.placements = {
    {
        name = "bloom_strength_trigger",
        data = {
            bloomStrengthFrom = 1.0,
            bloomStrengthTo = 1.0,
            positionMode = "NoEffect"
        }
    }
}

return bloomStrengthTrigger
