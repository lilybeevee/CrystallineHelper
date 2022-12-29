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
	[CustomEntity("vitellary/flagsequencecontroller")]
    public class FlagSequenceController : Entity
    {
		public FlagSequenceController(EntityData data, Vector2 offset) : base(data.Position + offset)
		{
			Level level = Engine.Scene as Level;
			if (level == null)
			{
				LevelLoader levelLoader = Engine.Scene as LevelLoader;
				level = (levelLoader != null) ? levelLoader.Level : null;
			}
			if (level != null && (!data.Bool("onlyOnRespawn") || respawning))
			{
				string prefix = data.Attr("prefix", "");
				if (!string.IsNullOrEmpty(prefix))
                {
					bool state = data.Bool("state", false);
					for (int i = Math.Min(data.Int("startNumber", 1), 0); i <= data.Int("endNumber", 2); i++)
                    {
						if (i < 10)
                        {
							level.Session.SetFlag(prefix + "_0" + i, state);
                        }
                        else
                        {
							level.Session.SetFlag(prefix + "_" + i, state);
						}
					}
                }
			}
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			RemoveSelf();
		}

        public static bool respawning;

        public static void Load() {
            On.Celeste.Level.LoadLevel += Level_LoadLevel;
        }

        public static void Unload() {
            On.Celeste.Level.LoadLevel -= Level_LoadLevel;
        }

        private static void Level_LoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader) {
            respawning = playerIntro > Player.IntroTypes.Transition;
            orig(self, playerIntro, isFromLoader);
            respawning = false;
        }
    }
}
