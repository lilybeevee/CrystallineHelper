using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace vitmod
{
    [CustomEntity("vitellary/nodashtrigger")]
    [Tracked(false)]
    public class NoDashTrigger : Trigger
    {
        public static Hook hook;
        public delegate bool orig_CanDash(Player self);
        public static void Load()
        {
            // On.Celeste.Player.StartDash += Player_StartDash;
            hook = new Hook(typeof(Player).GetProperty("CanDash", BindingFlags.Public | BindingFlags.Instance).GetMethod, Player_CanDash);
            hook.Apply();
        }

        public static void Unload()
        {
            // On.Celeste.Player.StartDash -= Player_StartDash;
            hook.Dispose();
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

        public static bool Player_CanDash(orig_CanDash orig, Player self) {
            if (!self.Dead && self.CollideCheck<NoDashTrigger>()) {
                return false;
            }
            return orig(self);
        }
    }
}
