module CustomWindSnow

using ..Ahorn, Maple

@mapdef Effect "CrystallineHelper/CustomWindSnow" Snow(only::String="*", exclude::String="", colors::String="FFFFFF", alphas::String="1", amount::Integer=240, speedX::Number=0.0, speedY::Number=0.0, ignoreWind::Bool=false)

placements = Snow

Ahorn.editingOrder(entity::Snow) = ["name", "only", "exclude", "tag", "flag", "notflag", "colors", "alphas", "speedX", "speedY", "amount", "fg", "ignoreWind"]

function Ahorn.canFgBg(effect::Snow)
    return true, true
end

end