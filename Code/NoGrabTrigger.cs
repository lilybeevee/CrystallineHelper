using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vitmod
{
    [CustomEntity("vitellary/nograbtrigger")]
    [Tracked(false)]
    public class NoGrabTrigger : Trigger
    {
        public static void Load()
        {
            On.Celeste.Player.ClimbUpdate += Player_ClimbUpdate;
        }

        public static void Unload()
        {
            On.Celeste.Player.ClimbUpdate -= Player_ClimbUpdate;
        }

        public NoGrabTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Add(new ClimbBlocker(true));
        }

        public static int Player_ClimbUpdate(On.Celeste.Player.orig_ClimbUpdate orig, Player self)
        {
            if (!self.Dead && self.CollideCheck<NoGrabTrigger>()) {
                self.Speed += (Vector2)self.GetType().GetProperty("LiftBoost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(self);
                return 0;
            }
            return orig(self);
        }
    }
}
