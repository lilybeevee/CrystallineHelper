local timedFadeTrigger = {}

timedFadeTrigger.name = "vitellary/timedfadetrigger"
timedFadeTrigger.nodeLimits = {1, 1}

timedFadeTrigger.fieldInformation = {
    time = {
        minimumValue = 0.0
    }
}

timedFadeTrigger.placements = {
    {
        name = "timed_fade_trigger",
        data = {
            time = 1.0
        }
    }
}

return timedFadeTrigger
