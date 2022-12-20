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
    [CustomEntity("vitellary/custombridgeactivator")]
    public class CustomBridgeActivator : Trigger
    {
        public CustomBridgeActivator(EntityData data, Vector2 offset) : base(data, offset)
        {
            actID = data.Attr("activationID");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (CustomBridge.bridgeList.ContainsKey(actID) && CustomBridge.bridgeList[actID].ContainsKey(0))
            {
                CustomBridge bridge = CustomBridge.bridgeList[actID][0];
                bridge.Add(new Coroutine(bridge.FallRoutine(0)));
            }
            RemoveSelf();
        }

        private string actID;
    }
}
