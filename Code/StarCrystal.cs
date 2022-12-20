﻿using Celeste;
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
    [CustomEntity("vitellary/starcrystal")]
    public class StarCrystal : Entity
    {
        public static void Load()
        {
            On.Celeste.Player.DashBegin += Player_DashBegin;
            On.Celeste.Solid.Update += Solid_Update;
        }

        public static void Unload()
        {
            On.Celeste.Player.DashBegin -= Player_DashBegin;
        }

        public StarCrystal(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            oneUse = data.Bool("oneUse", false);
            time = data.Float("time", 2f);
            useTime = data.Float("respawnTime", 2.5f);
            changeDashes = data.Bool("changeDashes", true);
            changeInvuln = data.Bool("changeInvuln", true);
            changeStamina = data.Bool("changeStamina", true);

            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
            if (!oneUse)
            {
                Add(outline = new Image(GFX.Game["objects/crystals/star/outline"]));
                outline.CenterOrigin();
                outline.Visible = false;
            }
            string spritetype = oneUse ? "idlenr" : "idle";
            Add(sprite = new Sprite(GFX.Game, "objects/crystals/star/" + spritetype));
            sprite.AddLoop(spritetype, "", 0.1f);
            sprite.Play(spritetype, false, false);
            sprite.CenterOrigin();
            Add(flash = new Sprite(GFX.Game, "objects/crystals/star/flash"));
            flash.Add("flash", "", 0.05f);
            flash.OnFinish = delegate (string anim)
            {
                flash.Visible = false;
            };
            flash.CenterOrigin();
            Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v)
            {
                sprite.Scale = (flash.Scale = Vector2.One * (1f + v * 0.2f));
            }, false, false));
            Add(new MirrorReflection());
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
            Add(sine = new SineWave(0.6f));
            sine.Randomize();
            UpdateY();
            Depth = -100;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            if (respawnTimer > 0f)
            {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f)
                {
                    Respawn();
                }
            }
            else if (Scene.OnInterval(0.1f))
            {
                level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
            }
            UpdateY();
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
            if (Scene.OnInterval(2f) && sprite.Visible)
            {
                flash.Play("flash", true, false);
                flash.Visible = true;
            }
        }

        private void Respawn()
        {
            if (!Collidable)
            {
                Collidable = true;
                sprite.Visible = true;
                outline.Visible = false;
                Depth = -100;
                wiggler.Start();
                Audio.Play("event:/game/general/diamond_return", Position);
                level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
            }
        }

        private void UpdateY()
        {
            flash.Y = (sprite.Y = (bloom.Y = sine.Value * 2f));
        }

        public override void Render()
        {
            if (sprite.Visible)
            {
                sprite.DrawOutline(1);
            }
            base.Render();
        }

        private void OnPlayer(Player player)
        {
            if (changeDashes)
            {
                player.Dashes = player.MaxDashes;
            }
            Audio.Play("event:/game/general/diamond_touch", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            Add(new Coroutine(RefillRoutine(player), true));
            if (changeDashes)
            {
                starDashTimer = time;
            }
            if (changeInvuln)
            {
                starInvulnTimer = time;
            }
            if (changeStamina)
            {
                starStaminaTimer = time;
            }
            if (oneUse)
            {
                RemoveSelf();
            }
            else
            {
                respawnTimer = useTime;
            }
        }

        private IEnumerator RefillRoutine(Player player)
        {
            Celeste.Celeste.Freeze(0.1f);
            yield return null;
            level.Shake(0.3f);
            sprite.Visible = (flash.Visible = false);
            if (!oneUse)
            {
                outline.Visible = true;
            }
            Depth = 8999;
            yield return 0.05f;
            float num = player.Speed.Angle();
            level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num - 1.57079637f);
            level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, num + 1.57079637f);
            SlashFx.Burst(Position, num);
            yield break;
        }

        public static void Player_DashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
        {
            orig(self);
            if (starDashTimer > 0f)
            {
                self.Dashes = self.MaxDashes;
            }
        }

        private static void Solid_Update(On.Celeste.Solid.orig_Update orig, Solid self)
        {
            orig(self);
            if (starInvulnTimer > 0f && self.Components.Get<SolidOnInvinciblePlayer>() == null && self.Collidable)
            {
                Player player = self.CollideFirst<Player>();
                if (player != null && player.StateMachine.State != 9 && player.StateMachine.State != 21)
                {
                    self.Add(new SolidOnInvinciblePlayer());
                }
            }
        }

        private bool oneUse;

        private float time;

        private float useTime;

        private bool changeDashes;

        private bool changeInvuln;

        private bool changeStamina;

        public static ParticleType P_Shatter;

        public static ParticleType P_Regen;

        public static ParticleType P_Glow;

        public static float starDashTimer;

        public static float starInvulnTimer;

        public static float starStaminaTimer;

        private Sprite sprite;

        private Sprite flash;

        private Image outline;

        private Wiggler wiggler;

        private BloomPoint bloom;

        private VertexLight light;

        private Level level;

        private SineWave sine;

        private float respawnTimer;
    }
}
