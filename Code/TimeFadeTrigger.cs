using Celeste;
using Celeste.Mod.Entities;
using MonoMod.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vitmod
{
    [CustomEntity("vitellary/timedfadetrigger")]
    [Tracked(false)]
    public class TimeFadeTrigger : Trigger
    {
        public static void Load()
        {
            On.Celeste.Trigger.GetPositionLerp += Trigger_GetPositionLerp;
        }

        public static void Unload()
        {
            On.Celeste.Trigger.GetPositionLerp -= Trigger_GetPositionLerp;
        }

        public TimeFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            nodes = data.NodesOffset(offset);
            totalTime = data.Float("time", 1f);
            oneUse = data.Bool("oneUse", true);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Trigger trigger = scene.CollideFirst<Trigger>(nodes[0]);
            if (trigger == null)
            {
                trigger = scene.Tracker.GetNearestEntity<Trigger>(nodes[0]);
            }
            if (trigger != null)
            {
                trigger.Collidable = false;
                triggerToFade = trigger;
            }
        }

        public override void Update()
        {
            base.Update();
            if (playerInside != null && activatedTimer < totalTime)
            {
                activatedTimer += Engine.DeltaTime;
                if (activatedTimer >= totalTime)
                {
                    activatedTimer = totalTime;
                    if (triggerToFade != null)
                    {
                        triggerToFade.OnLeave(playerInside);
                        if (oneUse) {
                            RemoveSelf();
                        } else {
                            playerInside = null;
                            activatedTimer = 0;
                        }
                    }
                }
                else
                {
                    if (triggerToFade != null)
                    {
                        triggerToFade.OnStay(playerInside);
                    }
                }
            }
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (triggerToFade != null)
            {
                triggerToFade.OnEnter(player);
            }
            playerInside = player;
        }

        private static float Trigger_GetPositionLerp(On.Celeste.Trigger.orig_GetPositionLerp orig, Trigger self, Player player, PositionModes mode)
        {
            foreach (TimeFadeTrigger trigger in self.SceneAs<Level>().Tracker.GetEntities<TimeFadeTrigger>())
            {
                if (trigger.triggerToFade == self)
                {
                    return trigger.activatedTimer / trigger.totalTime;
                }
            }
            return orig(self, player, mode);
        }

        private Vector2[] nodes;

        public float totalTime;
        public bool oneUse;

        public Trigger triggerToFade;

        private Player playerInside;

        public float activatedTimer;
    }
}
