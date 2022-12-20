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
    [CustomEntity("vitellary/remotetrigger")]
    [Tracked(false)]
    public class RemoteTrigger : Trigger
    {
        public RemoteTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            remote = data.Int("value", 1);
        }

        public override void OnEnter(Player player)
        {
            if (remote > 0)
            {
                foreach (var entity in player.SceneAs<Level>())
                {
                    if (entity is VitMoveBlock)
                    {
                        var moveBlock = entity as VitMoveBlock;

                        if (moveBlock.remote == remote)
                        {
                            moveBlock.triggered = true;
                        }
                    }
                }
            }
        }

        private int remote;
    }
}