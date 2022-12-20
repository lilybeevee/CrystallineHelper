local fillCrystal = {}

fillCrystal.name = "vitellary/fillcrystal"
fillCrystal.depth = -100

fillCrystal.fieldInformation = {
    respawnTime = {
        minimumValue = 0.0
    }
}

fillCrystal.placements = {
    {
        name = "fill_crystal",
        data = {
            oneUse = false,
            respawnTime = 2.5
        }
    }
}

fillCrystal.texture = "objects/crystals/fill/idle00"

return fillCrystal
