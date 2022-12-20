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
    [CustomEntity("vitellary/nomovetrigger")]
    [Tracked(false)]
    public class NoMoveTrigger : Trigger
    {
        public static void Load()
        {
            On.Monocle.VirtualIntegerAxis.Update += VirtualIntegerAxis_Update;
            On.Monocle.VirtualButton.Update += VirtualButton_Update;
        }

        public static void Unload()
        {
            On.Monocle.VirtualIntegerAxis.Update -= VirtualIntegerAxis_Update;
            On.Monocle.VirtualButton.Update -= VirtualButton_Update;
        }

        public NoMoveTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            stopLength = data.Float("stopLength", 2f);
        }

        public override void OnEnter(Player player)
        {
            if (!alreadyIn)
            {
                VitModule.noMoveScaleTimer = 0f;
                stopTimer = stopLength;
                stopStage = 1;
            }
            alreadyIn = true;
        }

        public override void OnLeave(Player player)
        {
            if (player.Scene != null && !player.CollideCheck<NoMoveTrigger>())
            {
                alreadyIn = false;
            }
        }

        static Dictionary<VirtualButton, List<VirtualButton.Node>> buttonNodes = new Dictionary<VirtualButton, List<VirtualButton.Node>>();
        public static void VirtualButton_Update(On.Monocle.VirtualButton.orig_Update orig, VirtualButton self)
        {
            if (self == Input.Jump || self == Input.Grab)
            {
                if (stopStage > 0 && GetDeltaMult() == 0)
                {
                    if (!buttonNodes.ContainsKey(self))
                    {
                        buttonNodes.Add(self, self.Nodes);
                        self.Nodes = new List<VirtualButton.Node>();
                    }
                }
                else
                {
                    if (buttonNodes.ContainsKey(self))
                    {
                        self.Nodes = buttonNodes[self];
                        buttonNodes.Remove(self);
                    }
                }
            }

            orig(self);
        }

        public static void VirtualIntegerAxis_Update(On.Monocle.VirtualIntegerAxis.orig_Update orig, VirtualIntegerAxis self)
        {
            orig(self);

            if (self == Input.MoveX || self == Input.MoveY)
            {
                if (stopStage > 0 && GetDeltaMult() == 0)
                {
                    self.Value = 0;
                }
            }
        }

        private float stopLength;
        public static float stopTimer;
        public static float stopStage;

        public static bool alreadyIn;

        public static float GetDeltaMult()
        {
            float delta_mult = 1;

            if (stopStage == 1)
            {
                delta_mult = Math.Max(0, 1 - (VitModule.noMoveScaleTimer / 0.3f));
            }
            else if (stopStage == 2)
            {
                delta_mult = Math.Min(1, VitModule.noMoveScaleTimer / 0.3f);
            }

            return delta_mult;
        }
    }
}