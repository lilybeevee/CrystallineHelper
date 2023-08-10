using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Celeste.Mod.Code.Entities
{
    [CustomEntity("vitellary/roomnametrigger")]
    [Tracked]
    public class RoomNameTrigger : Trigger
    {
        public string Name;

        public RoomNameTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Name = data.Attr("roomName");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            var display = Scene.Tracker.GetEntity<RoomNameDisplay>();
            if (display == null)
            {
                display = new RoomNameDisplay();
                Scene.Add(display);
            }
            display.SetName(Name);
        }
    }
}
