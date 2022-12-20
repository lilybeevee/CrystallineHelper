using Celeste;
using Celeste.Mod.Entities;
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
    [CustomEntity("vitellary/customprologuebridge")]
    public class CustomBridge : Entity
    {
        public CustomBridge(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            width = data.Width;
            flag = data.Attr("flag", "");
            actID = data.Attr("activationID", "");
            actIndex = data.Int("activationIndex", 0);
            left = data.Bool("left", false);
            actDelay = data.Float("delay", 0f);
            actSpeed = data.Float("speed", 0.8f) / 10;
            id = data.ID;

            Add(sfx = new SoundSource());

            tileSizes = new List<Rectangle>();
            tileSizes.Add(new Rectangle(0, 0, 8, 20));
            tileSizes.Add(new Rectangle(0, 0, 8, 13));
            tileSizes.Add(new Rectangle(0, 0, 8, 13));
            tileSizes.Add(new Rectangle(0, 0, 8, 8));
            tileSizes.Add(new Rectangle(0, 0, 8, 8));
            tileSizes.Add(new Rectangle(0, 0, 8, 8));
            tileSizes.Add(new Rectangle(0, 0, 8, 7));
            tileSizes.Add(new Rectangle(0, 0, 8, 8));
            tileSizes.Add(new Rectangle(0, 0, 8, 8));
            tileSizes.Add(new Rectangle(0, 0, 16, 16));
            tileSizes.Add(new Rectangle(0, 0, 16, 16));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!bridgeList.ContainsKey(actID))
            {
                bridgeList.Add(actID, new Dictionary<int, CustomBridge>());
            }
            if (!bridgeList[actID].ContainsKey(actIndex))
            {
                bridgeList[actID].Add(actIndex, this);
            }

            Calc.PushRandom(id);
            int i = 0;
            int tilewidth = (int)Math.Floor((decimal)width / 8);
            while (i < tilewidth)
            {
                int index;
                if (i == tilewidth - 1)
                {
                    index = Calc.Random.Next(tileSizes.Count - 2);
                }
                else
                {
                    index = Calc.Random.Next(tileSizes.Count);
                }
                CustomBridgeTile tile = new CustomBridgeTile(Position + new Vector2(i * 8, 0), tileSizes[index], index);
                tiles.Add(tile);
                SceneAs<Level>().Add(tile);
                if (index >= tileSizes.Count - 2)
                {
                    i += 2;
                }
                else
                {
                    i++;
                }
            }
            Calc.PopRandom();
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if ((scene as Level).Session.GetFlag(flag))
            {
                foreach (CustomBridgeTile tile in tiles)
                {
                    tile.DisableStaticMovers();
                    tile.RemoveSelf();
                }
                RemoveSelf();
            }
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player == null || player.Dead)
            {
                sfx.Stop();
            }
        }

        public IEnumerator FallRoutine(int bridgeIndex)
        {
            yield return actDelay;
            sfx.Play("event:/game/00_prologue/bridge_rumble_loop");
            for (int i = 0; i < tiles.Count; i++)
            {
                int tileIndex = left ? (tiles.Count - 1 - i) : i;
                CustomBridgeTile tile = tiles[tileIndex];
                tile.Fall();
                yield return actSpeed;
            }
            int nextBridge = bridgeIndex + 1;
            if (bridgeList[actID].ContainsKey(nextBridge))
            {
                CustomBridge bridge = bridgeList[actID][nextBridge];
                bridge.Add(new Coroutine(bridge.FallRoutine(nextBridge)));
            }
            sfx.Stop();
            yield break;
        }

        private int width;

        private string flag;

        private string actID;

        private int actIndex;

        private bool left;

        private float actDelay;

        private float actSpeed;

        private SoundSource sfx;

        public static Dictionary<string, Dictionary<int, CustomBridge>> bridgeList = new Dictionary<string, Dictionary<int, CustomBridge>>();

        private List<Rectangle> tileSizes;

        private List<CustomBridgeTile> tiles = new List<CustomBridgeTile>();

        private int id;
    }
}
