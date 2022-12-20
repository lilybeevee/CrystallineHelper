local starCrystal = {}

starCrystal.name = "vitellary/starcrystal"
starCrystal.depth = -100

starCrystal.fieldInformation = {
    time = {
        minimumValue = 0.0
    },
    respawnTime = {
        minimumValue = 0.0
    }
}

starCrystal.placements = {
    {
        name = "star_crystal",
        data = {
            oneUse = false,
            time = 2.0,
            changeDashes = true,
            changeInvuln = true,
            changeStamina = true,
            respawnTime = 2.5
        }
    }
}

starCrystal.texture = "objects/crystals/star/idle00"

return starCrystal
