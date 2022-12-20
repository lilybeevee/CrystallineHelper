using Celeste;
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
    public class CustomBridgeTile : Solid
    {
        public CustomBridgeTile(Vector2 position, Rectangle tileSize, int imageIndex) : base(position, tileSize.Width, tileSize.Height, false)
        {
            Add(image = new Image(GFX.Game["objects/customBridge/tile" + imageIndex]));
            image.Origin = new Vector2(image.Width / 2f, 0f);
            image.X = image.Width / 2f;
            image.Y = 0f;
        }

        public override void Update()
        {
            base.Update();
            if (fallen)
            {
                if (shakeTimer > 0f)
                {
                    shakeTimer -= Engine.DeltaTime;
                    if (Scene.OnInterval(0.02f))
                    {
                        shakeOffset = Calc.Random.ShakeVector();
                    }
                    if (shakeTimer <= 0f)
                    {
                        SceneAs<Level>().Shake(0.1f);
                        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    }
                    image.Position = new Vector2(image.Width / 2f, 0f) + shakeOffset;
                }
                else
                {
                    colorLerp = Calc.Approach(colorLerp, 1f, 10f * Engine.DeltaTime);
                    image.Color = Color.Lerp(Color.White, Color.Gray, colorLerp);
                    shakeOffset = Vector2.Zero;
                    speedY = Calc.Approach(speedY, 200f, 900f * Engine.DeltaTime);
                    MoveV(speedY * Engine.DeltaTime);
                    if (Top > SceneAs<Level>().Bounds.Bottom)
                    {
                        DisableStaticMovers();
                        RemoveSelf();
                    }
                }
            }
        }

        public void Fall()
        {
            fallen = true;
            Calc.PushRandom(11 / 6);
            shakeTimer = Calc.Random.Range(0.1f, 0.5f);
            Calc.PopRandom();
        }

        private Image image;

        private bool fallen;

        private float shakeTimer;

        private Vector2 shakeOffset;

        private float colorLerp;

        private float speedY;
    }
}
