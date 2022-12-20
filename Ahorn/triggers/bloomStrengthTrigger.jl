module VitellaryBloomStrengthTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/bloomstrengthtrigger" BloomTrigger(x::Integer, y::Integer,
    width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    bloomStrengthFrom::Number=1.0, bloomStrengthTo::Number=1.0, positionMode::String="NoEffect")

const placements = Ahorn.PlacementDict(
    "Bloom Strength Trigger (Crystalline)" => Ahorn.EntityPlacement(
        BloomTrigger,
        "rectangle"
    )
)

function Ahorn.editingOptions(trigger::BloomTrigger)
    return Dict{String, Any}(
        "positionMode" => Maple.trigger_position_modes
    )
end

end