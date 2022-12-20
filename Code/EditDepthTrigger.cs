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
    [CustomEntity("vitellary/editdepthtrigger")]
    public class EditDepthTrigger : Trigger
    {
        public EditDepthTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            newDepth = data.Int("depth", 0);
            entityNames = data.Attr("entitiesToAffect", "").Split(',');
            debug = data.Bool("debug", false);
            update = data.Bool("updateOnEntry", false);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            foreach (Entity entity in scene.Entities)
            {
                if (entity.CollideCheck(this))
                {
                    if (entityNames.Contains(entity.GetType().FullName) || entityNames.Contains(entity.GetType().Name))
                    {
                        entity.Depth = newDepth;
                    }
                    if (debug)
                    {
                        Console.WriteLine(entity.GetType().FullName + ": " + entity.Depth);
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (update)
            {
                foreach (Entity entity in Scene.Entities)
                {
                    if (entity.CollideCheck(this))
                    {
                        if (entityNames.Contains(entity.GetType().FullName) || entityNames.Contains(entity.GetType().Name))
                        {
                            entity.Depth = newDepth;
                        }
                    }
                }
            }
        }

        private int newDepth;
        private string[] entityNames;
        private bool debug;
        private bool update;
    }
}
