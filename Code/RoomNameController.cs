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
    [CustomEntity("vitellary/roomname")]
    [Tracked]
    public class RoomNameController : Entity
    {
        public string Name;

        public RoomNameController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Name = data.Attr("roomName");
        }
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            var display = scene.Tracker.GetEntity<RoomNameDisplay>();
            if (display == null)
            {
                display = new RoomNameDisplay();
                Scene.Add(display);
            }
            display.SetName(Name);
        }
    }
}
