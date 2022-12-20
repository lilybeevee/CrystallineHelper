using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace vitmod
{
    [CustomEntity("vitellary/forcejumpcrystal")]
    public class ForceJumpCrystal : Entity
    {
        public ForceJumpCrystal(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            oneUse = data.Bool("oneUse", false);
            useTime = data.Float("respawnTime", 2.5f);
            holdJump = data.Bool("holdJump", true);
            onlyIfPossible = data.Bool("onlyJumpIfPossible", false);

            string useType = oneUse ? "idlenr" : "idle";

            sprites = new List<Sprite>();
            flashSprites = new List<Sprite>();
            sines = new List<SineWave>();

            Sprite leftSprite = new Sprite(GFX.Game, "objects/crystals/forcejump/" + useType);
            leftSprite.AddLoop("idle", "", 0.1f);
            leftSprite.Play("idle", false, true);
            leftSprite.CenterOrigin();
            leftSprite.X -= 4;
            Add(leftSprite);
            sprites.Add(leftSprite);

            Sprite leftFlash = new Sprite(GFX.Game, "objects/crystals/forcejump/flash");
            leftFlash.Add("flash", "", 0.05f);
            leftFlash.OnFinish = delegate (string anim)
            {
                leftFlash.Visible = false;
            };
            leftFlash.CenterOrigin();
            leftFlash.X -= 4;
            Add(leftFlash);
            flashSprites.Add(leftFlash);

            Sprite rightSprite = new Sprite(GFX.Game, "objects/crystals/forcejump/" + useType);
            rightSprite.AddLoop("idle", "", 0.1f);
            rightSprite.Play("idle", false, true);
            rightSprite.CenterOrigin();
            rightSprite.X += 4;
            rightSprite.FlipX = true;
            Add(rightSprite);
            sprites.Add(rightSprite);

            Sprite rightFlash = new Sprite(GFX.Game, "objects/crystals/forcejump/flash");
            rightFlash.Add("flash", "", 0.05f);
            rightFlash.OnFinish = delegate (string anim)
            {
                rightFlash.Visible = false;
            };
            rightFlash.CenterOrigin();
            rightFlash.X += 4;
            rightFlash.FlipX = true;
            Add(rightFlash);
            flashSprites.Add(rightFlash);

            outline = new Image(GFX.Game["objects/crystals/forcejump/outline"]);
            outline.CenterOrigin();
            outline.Visible = false;
            Add(outline);

            for (int i = 0; i < 2; i++)
            {
                SineWave sine = new SineWave(0.6f, Calc.Random.Range(0f, 1f));
                Add(sine);
                sines.Add(sine);
            }

            Collider = new Hitbox(16f, 16f, -8f, -8f);
            Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));

            Add(wiggler = Wiggler.Create(1f, 4f, (float v) =>
            {
                for (int i = 0; i < 2; i++)
                {
                    flashSprites[i].Scale = sprites[i].Scale = Vector2.One * (1f + v * 0.2f);
                }
            }, false, false));
            Add(new MirrorReflection());
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));

            Depth = -100;
            UpdateSprite();
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
                SceneAs<Level>().ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
            }
            UpdateSprite();
            light.Alpha = Calc.Approach(light.Alpha, sprites[0].Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
            if (Scene.OnInterval(2f) && sprites[0].Visible)
            {
                foreach (Sprite flash in flashSprites)
                {
                    flash.Play("flash", true, false);
                    flash.Visible = true;
                }
            }
        }

        private void Respawn()
        {
            if (!Collidable)
            {
                Collidable = true;
                foreach (Sprite sprite in sprites)
                {
                    sprite.Visible = true;
                }
                outline.Visible = false;
                Depth = -100;
                wiggler.Start();
                Audio.Play("event:/game/general/diamond_return", Position);
                SceneAs<Level>().ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
            }
        }

        private void UpdateSprite()
        {
            for (int i = 0; i < 2; i++)
            {
                flashSprites[i].Y = sprites[i].Y = sines[i].Value;
            }
        }

        public override void Render()
        {
            foreach (Sprite sprite in sprites)
            {
                if (sprite.Visible)
                {
                    sprite.DrawOutline(1);
                }
            }
            base.Render();
        }

        private void OnPlayer(Player player)
        {
            DynData<Player> playerData = new DynData<Player>(player);
            player.StateMachine.State = 0;
            if (player.DashAttacking)
            {
                if (playerData.Get<bool>("SuperWallJumpAngleCheck"))
                {
                    if ((bool)m_WallJumpCheck.Invoke(player, new object[] { -1 }))
                    {
                        m_SuperWallJump.Invoke(player, new object[] { 1 });
                    }
                    else if ((bool)m_WallJumpCheck.Invoke(player, new object[] { 1 }))
                    {
                        m_SuperWallJump.Invoke(player, new object[] { -1 });
                    }
                    else
                    {
                        m_SuperWallJump.Invoke(player, new object[] { 0 });
                    }
                }
                else
                {
                    m_SuperJump.Invoke(player, new object[] { });
                }
            }
            else
            {
                if ((bool)m_WallJumpCheck.Invoke(player, new object[] { (int)player.Facing }))
                {
                    if (Input.GrabCheck && player.Stamina > 0f && player.Holding == null &&
                        !ClimbBlocker.Check(Scene, player, player.Position + Vector2.UnitX * 3f))
                    {
                        m_ClimbJump.Invoke(player, new object[] { });
                    }
                    else
                    {
                        m_WallJump.Invoke(player, new object[] { -(int)player.Facing });
                    }
                }
                else if ((bool)m_WallJumpCheck.Invoke(player, new object[] { -(int)player.Facing }))
                {
                    m_WallJump.Invoke(player, new object[] { (int)player.Facing });
                }
                else
                {
                    player.Jump();
                }
            }
            player.AutoJump = holdJump;
            Audio.Play("event:/game/general/diamond_touch", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            Add(new Coroutine(RefillRoutine(player), true));
            if (!oneUse)
            {
                respawnTimer = useTime;
            }
        }

        private IEnumerator RefillRoutine(Player player)
        {
            Celeste.Celeste.Freeze(0.05f);
            yield return null;
            Level level = SceneAs<Level>();
            level.Shake(0.3f);
            for (int i = 0; i < 2; i++)
            {
                flashSprites[i].Visible = sprites[i].Visible = false;
            }
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
            if (oneUse)
            {
                RemoveSelf();
            }
            yield break;
        }

        private bool oneUse;

        private float useTime;

        private bool holdJump;

        private bool onlyIfPossible;

        private List<Sprite> sprites;

        private List<Sprite> flashSprites;

        private List<SineWave> sines;

        private Image outline;

        private Wiggler wiggler;

        private BloomPoint bloom;

        private VertexLight light;

        private float respawnTimer;

        public static ParticleType P_Glow;

        public static ParticleType P_Regen;
        
        public static ParticleType P_Shatter;

        //functions for making player jump
        private static MethodInfo m_SuperJump = typeof(Player).GetMethod(
            "SuperJump", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo m_ClimbJump = typeof(Player).GetMethod(
            "ClimbJump", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo m_WallJumpCheck = typeof(Player).GetMethod(
            "WallJumpCheck", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo m_SuperWallJump = typeof(Player).GetMethod(
            "SuperWallJump", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo m_WallJump = typeof(Player).GetMethod(
            "WallJump", BindingFlags.Instance | BindingFlags.NonPublic);
    }
}
