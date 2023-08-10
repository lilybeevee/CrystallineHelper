using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Celeste.Mod.Code.Entities
{
    [Tracked]
    public class RoomNameDisplay : Entity
    {
        private float drawLerp;
        private float textLerp;
        private string text;
        private string nextText;
        public RoomNameDisplay() {
            Tag = Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate;
            Depth = -100;
            drawLerp = 0f;
            textLerp = 0f;
        }

        public override void Update()
        {
            base.Update();
            float speed = 1.5f;
            if (Scene.Tracker.GetEntity<RoomNameController>() != null)
            {
                if (drawLerp > 0f || !(Scene as Level).Transitioning) {
                    drawLerp = Calc.Approach(drawLerp, 1f, Engine.DeltaTime * speed);
                }
            }
            else
            {
                drawLerp = Calc.Approach(drawLerp, 0f, Engine.DeltaTime * speed);
                if (drawLerp == 0f)
                {
                    RemoveSelf();
                }
            }

            if (text == nextText)
            {
                textLerp = Calc.Approach(textLerp, 1f, Engine.DeltaTime * speed);
            }
            else
            {
                textLerp = Calc.Approach(textLerp, 0f, Engine.DeltaTime * speed);
                if (textLerp == 0f)
                {
                    text = nextText;
                }
            }
        }

        public override void Render()
        {
            base.Render();
            var y = Calc.LerpClamp(1080f, 1032f, Ease.CubeOut(drawLerp));
            Draw.Rect(-2f, y, 1920f + 4f, 48f + 2f, Color.Black);
            if (text != "")
            {
                var texty = Calc.LerpClamp(1080f, 1032f, Ease.CubeOut(Calc.Min(textLerp, drawLerp)));
                ActiveFont.Draw(text, new Vector2(960, texty - 6f), new Vector2(0.5f, 0f), new Vector2(1f, 1f), Color.White);
            }
        }

        public void SetName(string name)
        {
            nextText = name;
        }
    }
}
