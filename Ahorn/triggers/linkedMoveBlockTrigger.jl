module RemoteTrigger

using ..Ahorn, Maple

@mapdef Trigger "vitellary/remotetrigger" Remote(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, value::Integer=1)

end