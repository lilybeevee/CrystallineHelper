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
    [CustomEntity("vitellary/nodashtrigger")]
    [Tracked(false)]
    public class NoDashTrigger : Trigger
    {
        public static void Load()
        {
            On.Celeste.Player.StartDash += Player_StartDash;
        }

        public static void Unload()
        {
            On.Celeste.Player.StartDash -= Player_StartDash;
        }

        public NoDashTrigger(EntityData data, Vector2 offset) : base(data, offset) { }

        public static int Player_StartDash(On.Celeste.Player.orig_StartDash orig, Player self)
        {
            if (!self.Dead && self.CollideCheck<NoDashTrigger>())
            {
                self.Speed -= (Vector2) self.GetType().GetProperty("LiftBoost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(self);
                return self.StateMachine.State;
            }
            return orig(self);
        }
    }
}
