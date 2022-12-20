module CustomBridgeActivator

using ..Ahorn, Maple

@mapdef Trigger "vitellary/custombridgeactivator" Activator(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, activationID::String="")

const placements = Ahorn.PlacementDict(
    "Custom Bridge Activator (Crystalline)" => Ahorn.EntityPlacement(
        Activator,
        "rectangle"
    )
)

end