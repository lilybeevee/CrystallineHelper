local noMoveTrigger = {}

noMoveTrigger.name = "vitellary/nomovetrigger"

noMoveTrigger.fieldInformation = {
    stopLength = {
        minimumValue = 0.0
    }
}

noMoveTrigger.placements = {
    {
        name = "no_move_trigger",
        data = {
            stopLength = 2.0
        }
    }
}

return noMoveTrigger
