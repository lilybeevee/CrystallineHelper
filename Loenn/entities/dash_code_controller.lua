local dashCodeController = {}

dashCodeController.name = "vitellary/dashcodecontroller"
dashCodeController.depth = -100

dashCodeController.fieldInformation = {
    index = {
        fieldType = "integer",
    },
}

dashCodeController.placements = {
    {
        name = "dash_code_controller",
        data = {
            dashCode = "",
            flagLabel = "",
            flagOnFailure = "",
            index = 0,
        }
    }
}

dashCodeController.texture = "ahorn_dashcodecontroller"

return dashCodeController
