using Celeste;
using Celeste.Mod.Entities;
using FMOD.Studio;
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
	[CustomEntity("vitellary/vitmoveblock")]
	public class VitMoveBlock : Solid
	{
		public VitMoveBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, false)
		{
			body = new List<Image>();
			topButton = new List<Image>();
			leftButton = new List<Image>();
			rightButton = new List<Image>();
			arrows = new List<MTexture>();
			fillColor = (remote == 0 ? idleBgFill : idleRemoteBgFill);
			Depth = -9000;
			startPosition = data.Position + offset;
			direction = data.Enum<VitMoveBlock.Directions>("direction");
			remote = data.Int("remote", 0);
			canSteer = data.Bool("canSteer", false);
			canActivate = data.Bool("canActivate", true);
			spritePath = data.Attr("spritePath", "objects/vitMoveBlock");
			moveSpeed = data.Float("moveSpeed", 75f);
			idleBgFill = Calc.HexToColor(data.Attr("idleSingleColor", "465eb5"));
			activeBgFill = Calc.HexToColor(data.Attr("activeSingleColor", "4fd6ff"));
			idleRemoteBgFill = Calc.HexToColor(data.Attr("idleLinkedColor", "9e45b2"));
			activeRemoteBgFill = Calc.HexToColor(data.Attr("activeLinkedColor", "ff8cf5"));
			breakingBgFill = Calc.HexToColor(data.Attr("breakingColor", "cc2541"));

			switch (direction)
			{
				case VitMoveBlock.Directions.Left:
					homeAngle = (targetAngle = (angle = 3.14159274f));
					angleSteerSign = -1;
					break;
				default:
					homeAngle = (targetAngle = (angle = 0f));
					angleSteerSign = 1;
					break;
				case VitMoveBlock.Directions.Up:
					homeAngle = (targetAngle = (angle = -1.57079637f));
					angleSteerSign = 1;
					break;
				case VitMoveBlock.Directions.Down:
					homeAngle = (targetAngle = (angle = 1.57079637f));
					angleSteerSign = -1;
					break;
			}
			int tileWidth = data.Width / 8;
			int tileHeight = data.Height / 8;
			MTexture mtexture = GFX.Game[spritePath + "/base"];
			MTexture mtexture2 = GFX.Game[spritePath + "/button"];
			if (canSteer && (direction == VitMoveBlock.Directions.Left || direction == VitMoveBlock.Directions.Right))
			{
				for (int i = 0; i < tileWidth; i++)
				{
					int num3 = (i == 0) ? 0 : ((i < tileWidth - 1) ? 1 : 2);
					AddImage(mtexture2.GetSubtexture(num3 * 8, 0, 8, 8, null), new Vector2((i * 8), -4f), 0f, new Vector2(1f, 1f), topButton);
				}
				mtexture = GFX.Game[spritePath + "/base_h"];
			}
			else if (canSteer && (direction == VitMoveBlock.Directions.Up || direction == VitMoveBlock.Directions.Down))
			{
				for (int j = 0; j < tileHeight; j++)
				{
					int num4 = (j == 0) ? 0 : ((j < tileHeight - 1) ? 1 : 2);
					AddImage(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8, null), new Vector2(-4f, (j * 8)), 1.57079637f, new Vector2(1f, -1f), leftButton);
					AddImage(mtexture2.GetSubtexture(num4 * 8, 0, 8, 8, null), new Vector2(((tileWidth - 1) * 8 + 4), (j * 8)), 1.57079637f, new Vector2(1f, 1f), rightButton);
				}
				mtexture = GFX.Game[spritePath + "/base_v"];
			}
			for (int k = 0; k < tileWidth; k++)
			{
				for (int l = 0; l < tileHeight; l++)
				{
					int num5 = (k == 0) ? 0 : ((k < tileWidth - 1) ? 1 : 2);
					int num6 = (l == 0) ? 0 : ((l < tileHeight - 1) ? 1 : 2);
					AddImage(mtexture.GetSubtexture(num5 * 8, num6 * 8, 8, 8, null), new Vector2(k, l) * 8f, 0f, new Vector2(1f, 1f), body);
				}
			}
			arrows = GFX.Game.GetAtlasSubtextures(spritePath + "/arrow");
			Add(moveSfx = new SoundSource());
			Add(new Coroutine(Controller(), true));
			UpdateColors();
			Add(new LightOcclude(0.5f));
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			scene.Add(border = new Border(this));
		}
		private IEnumerator Controller()
		{
			while (true)
			{
				triggered = false;
				state = VitMoveBlock.MovementState.Idling;
				while (!triggered && ((remote > 0 && !canActivate) || !HasPlayerRider()))
				{
					yield return null;
				}
				if (remote > 0)
				{
					foreach (var entity in SceneAs<Level>())
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
				Audio.Play("event:/game/04_cliffside/arrowblock_activate", Position);
				state = VitMoveBlock.MovementState.Moving;
				StartShaking(0.2f);
				ActivateParticles();
				yield return 0.2f;
				targetSpeed = moveSpeed;
				moveSfx.Play("event:/game/04_cliffside/arrowblock_move", null, 0f);
				moveSfx.Param("arrow_stop", 0f);
				StopPlayerRunIntoAnimation = false;
				float crashTimer = 0.15f;
				float crashResetTimer = 0.1f;
				float noSteerTimer = 0.2f;
				while (true)
				{
					heldTopButton = false;
					heldSideButton = false;
					if (canSteer)
					{
						targetAngle = homeAngle;
						bool flag;
						if (direction == VitMoveBlock.Directions.Right || direction == VitMoveBlock.Directions.Left)
						{
							flag = HasPlayerOnTop();
							heldTopButton = HasPlayerOnTop();
						}
						else
						{
							flag = HasPlayerClimbing();
							heldSideButton = HasPlayerClimbing();
						}
						if (!flag)
						{
							foreach (var entity in SceneAs<Level>())
							{
								if (entity is VitMoveBlock)
								{
									var moveBlock = entity as VitMoveBlock;

									if (moveBlock.remote == remote
									&& moveBlock.state == VitMoveBlock.MovementState.Moving
									&& (
										   (moveBlock.heldTopButton && (direction == VitMoveBlock.Directions.Left || direction == VitMoveBlock.Directions.Right))
										|| (moveBlock.heldSideButton && (direction == VitMoveBlock.Directions.Up || direction == VitMoveBlock.Directions.Down))))
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (flag && noSteerTimer > 0f)
						{
							noSteerTimer -= Engine.DeltaTime;
						}
						if (flag)
						{
							if (noSteerTimer <= 0f)
							{
								if (direction == VitMoveBlock.Directions.Right || direction == VitMoveBlock.Directions.Left)
								{
									targetAngle = homeAngle + 0.7853982f * angleSteerSign * Input.MoveY.Value;
								}
								else
								{
									targetAngle = homeAngle + 0.7853982f * angleSteerSign * Input.MoveX.Value;
								}
							}
						}
						else
						{
							noSteerTimer = 0.2f;
						}
					}
					if (Scene.OnInterval(0.02f))
					{
						MoveParticles();
					}
					speed = Calc.Approach(speed, targetSpeed, 300f * Engine.DeltaTime);
					angle = Calc.Approach(angle, targetAngle, 50.2654839f * Engine.DeltaTime);
					Vector2 vector = Calc.AngleToVector(angle, speed) * Engine.DeltaTime;
					bool flag2;
					if (direction == VitMoveBlock.Directions.Right || direction == VitMoveBlock.Directions.Left)
					{
						flag2 = MoveCheck(vector.XComp());
						if (heldTopButton)
						{
							noSquish = Scene.Tracker.GetEntity<Player>();
						}
						MoveVCollideSolids(vector.Y, false, null);
						noSquish = null;
						if (Scene.OnInterval(0.03f))
						{
							if (vector.Y > 0f)
							{
								ScrapeParticles(Vector2.UnitY);
							}
							else if (vector.Y < 0f)
							{
								ScrapeParticles(-Vector2.UnitY);
							}
						}
					}
					else
					{
						flag2 = MoveCheck(vector.YComp());
						if (heldSideButton)
						{
							noSquish = Scene.Tracker.GetEntity<Player>();
						}
						MoveHCollideSolids(vector.X, false, null);
						noSquish = null;
						if (Scene.OnInterval(0.03f))
						{
							if (vector.X > 0f)
							{
								ScrapeParticles(Vector2.UnitX);
							}
							else if (vector.X < 0f)
							{
								ScrapeParticles(-Vector2.UnitX);
							}
						}
						if (direction == VitMoveBlock.Directions.Down && Top > (SceneAs<Level>().Bounds.Bottom + 32))
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						moveSfx.Param("arrow_stop", 1f);
						crashResetTimer = 0.1f;
						if (crashTimer <= 0f)
						{
							break;
						}
						crashTimer -= Engine.DeltaTime;
					}
					else
					{
						moveSfx.Param("arrow_stop", 0f);
						if (crashResetTimer > 0f)
						{
							crashResetTimer -= Engine.DeltaTime;
						}
						else
						{
							crashTimer = 0.15f;
						}
					}
					Level level = Scene as Level;
					if (Left < level.Bounds.Left || Top < level.Bounds.Top || Right > level.Bounds.Right)
					{
						break;
					}
					yield return null;
				}
				Audio.Play("event:/game/04_cliffside/arrowblock_break", Position);
				moveSfx.Stop(true);
				state = VitMoveBlock.MovementState.Breaking;
				speed = targetSpeed = 0f;
				angle = (targetAngle = homeAngle);
				StartShaking(0.2f);
				StopPlayerRunIntoAnimation = true;
				yield return 0.2f;
				BreakParticles();
				List<Debris> debris = new List<Debris>();
				int num = 0;
				while (num < Width)
				{
					int num2 = 0;
					while (num2 < Height)
					{
						Vector2 value = new Vector2(num + 4f, num2 + 4f);
						Debris debris2 = Engine.Pooler.Create<Debris>().Init(Position + value, Center, startPosition + value);
						debris.Add(debris2);
						Scene.Add(debris2);
						num2 += 8;
					}
					num += 8;
				}
				MoveStaticMovers(startPosition - Position);
				DisableStaticMovers();
				Position = startPosition;
				Visible = (Collidable = false);
				yield return 2.2f;
				using (List<Debris>.Enumerator enumerator = debris.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Debris debris3 = enumerator.Current;
						debris3.StopMoving();
					}
					goto IL_6A5;
				}
			IL_6A5:
				if (!CollideCheck<Actor>() && !CollideCheck<Solid>())
				{
					Collidable = true;
					EventInstance instance = Audio.Play("event:/game/04_cliffside/arrowblock_reform_begin", debris[0].Position);
					Coroutine routine;
					Add(routine = new Coroutine(SoundFollowsDebrisCenter(instance, debris), true));
					foreach (Debris debris4 in debris)
					{
						debris4.StartShaking();
					}
					yield return 0.2f;
					foreach (Debris debris5 in debris)
					{
						debris5.ReturnHome(0.65f);
					}
					yield return 0.6f;
					routine.RemoveSelf();
					foreach (Debris debris6 in debris)
					{
						debris6.RemoveSelf();
					}
					routine = null;
					Audio.Play("event:/game/04_cliffside/arrowblock_reappear", Position);
					Visible = true;
					EnableStaticMovers();
					speed = (targetSpeed = 0f);
					angle = (targetAngle = homeAngle);
					noSquish = null;
					fillColor = (remote == 0 ? idleBgFill : idleRemoteBgFill);
					UpdateColors();
					flash = 1f;
					continue;
				}
			}
		}
		private IEnumerator SoundFollowsDebrisCenter(EventInstance instance, List<Debris> debris)
		{
			while (true)
			{
				PLAYBACK_STATE playback_STATE;
				instance.getPlaybackState(out playback_STATE);
				if (playback_STATE == PLAYBACK_STATE.STOPPED)
				{
					break;
				}
				Vector2 vector = Vector2.Zero;
				foreach (Debris debris2 in debris)
				{
					vector += debris2.Position;
				}
				vector /= debris.Count;
				Audio.Position(instance, vector);
				yield return null;
			}
			yield break;
		}
		public override void Update()
		{
			base.Update();
			if (canSteer)
			{
				bool flag = (direction == VitMoveBlock.Directions.Up || direction == VitMoveBlock.Directions.Down) && CollideCheck<Player>(Position + new Vector2(-1f, 0f));
				bool flag2 = (direction == VitMoveBlock.Directions.Up || direction == VitMoveBlock.Directions.Down) && CollideCheck<Player>(Position + new Vector2(1f, 0f));
				bool flag3 = (direction == VitMoveBlock.Directions.Left || direction == VitMoveBlock.Directions.Right) && CollideCheck<Player>(Position + new Vector2(0f, -1f));
				foreach (Image image in topButton)
				{
					image.Y = (flag3 ? 2 : 0);
				}
				foreach (Image image2 in leftButton)
				{
					image2.X = (flag ? 2 : 0);
				}
				foreach (Image image3 in rightButton)
				{
					image3.X = Width + (flag2 ? -2 : 0);
				}
				if ((flag && !leftPressed) || (flag3 && !topPressed) || (flag2 && !rightPressed))
				{
					Audio.Play("event:/game/04_cliffside/arrowblock_side_depress", Position);
				}
				if ((!flag && leftPressed) || (!flag3 && topPressed) || (!flag2 && rightPressed))
				{
					Audio.Play("event:/game/04_cliffside/arrowblock_side_release", Position);
				}
				leftPressed = flag;
				rightPressed = flag2;
				topPressed = flag3;
			}
			if (moveSfx != null && moveSfx.Playing)
			{
				int num = (int)Math.Floor(((-(double)(Calc.AngleToVector(angle, 1f) * new Vector2(-1f, 1f)).Angle() + 6.28318548f) % 6.28318548f / 6.28318548f * 8f + 0.5f));
				moveSfx.Param("arrow_influence", (num + 1));
			}
			border.Visible = Visible;
			flash = Calc.Approach(flash, 0f, Engine.DeltaTime * 5f);
			UpdateColors();
		}
		public override void MoveHExact(int move)
		{
			if (noSquish != null)
			{
				if (move >= 0 || noSquish.X >= X)
				{
					if (move <= 0 || noSquish.X <= X)
					{
						goto IL_6E;
					}
				}
				while (move != 0 && noSquish.CollideCheck<Solid>(noSquish.Position + Vector2.UnitX * move))
				{
					move -= Math.Sign(move);
				}
			}
		IL_6E:
			base.MoveHExact(move);
		}
		public override void MoveVExact(int move)
		{
			if (noSquish != null && move < 0 && noSquish.Y <= Y)
			{
				while (move != 0 && noSquish.CollideCheck<Solid>(noSquish.Position + Vector2.UnitY * move))
				{
					move -= Math.Sign(move);
				}
			}
			base.MoveVExact(move);
		}
		private bool MoveCheck(Vector2 speed)
		{
			if (speed.X != 0f)
			{
				if (MoveHCollideSolids(speed.X, false, null))
				{
					for (int i = 1; i <= 3; i++)
					{
						for (int j = 1; j >= -1; j -= 2)
						{
							Vector2 value = new Vector2(Math.Sign(speed.X), (i * j));
							if (!CollideCheck<Solid>(Position + value))
							{
								MoveVExact(i * j);
								MoveHExact(Math.Sign(speed.X));
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
			else
			{
				if (speed.Y == 0f)
				{
					return false;
				}
				if (MoveVCollideSolids(speed.Y, false, null))
				{
					for (int k = 1; k <= 3; k++)
					{
						for (int l = 1; l >= -1; l -= 2)
						{
							Vector2 value2 = new Vector2((k * l), Math.Sign(speed.Y));
							if (!CollideCheck<Solid>(Position + value2))
							{
								MoveHExact(k * l);
								MoveVExact(Math.Sign(speed.Y));
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
		}
		private void UpdateColors()
		{
			Color value = (remote == 0 ? idleBgFill : idleRemoteBgFill);
			if (state == VitMoveBlock.MovementState.Moving)
			{
				value = (remote == 0 ? activeBgFill : activeRemoteBgFill);
			}
			else if (state == VitMoveBlock.MovementState.Breaking)
			{
				value = breakingBgFill;
			}
			fillColor = Color.Lerp(fillColor, value, 10f * Engine.DeltaTime);
			foreach (Image image in topButton)
			{
				image.Color = fillColor;
			}
			foreach (Image image2 in leftButton)
			{
				image2.Color = fillColor;
			}
			foreach (Image image3 in rightButton)
			{
				image3.Color = fillColor;
			}
		}
		private void AddImage(MTexture tex, Vector2 position, float rotation, Vector2 scale, List<Image> addTo)
		{
			Image image = new Image(tex);
			image.Position = position + new Vector2(4f, 4f);
			image.CenterOrigin();
			image.Rotation = rotation;
			image.Scale = scale;
			Add(image);
			if (addTo != null)
			{
				addTo.Add(image);
			}
		}
		private void SetVisible(List<Image> images, bool visible)
		{
			foreach (Image image in images)
			{
				image.Visible = visible;
			}
		}
		public override void Render()
		{
			Vector2 position = Position;
			Position += Shake;
			foreach (Image image in leftButton)
			{
				image.Render();
			}
			foreach (Image image2 in rightButton)
			{
				image2.Render();
			}
			foreach (Image image3 in topButton)
			{
				image3.Render();
			}
			Draw.Rect(X + 3f, Y + 3f, Width - 6f, Height - 6f, fillColor);
			foreach (Image image4 in body)
			{
				image4.Render();
			}
			Draw.Rect(Center.X - 4f, Center.Y - 4f, 8f, 8f, fillColor);
			if (state != VitMoveBlock.MovementState.Breaking)
			{
				int value = (int)Math.Floor((-(double)angle + 6.28318548f) % 6.28318548f / 6.28318548f * 8f + 0.5f);
				arrows[Calc.Clamp(value, 0, 7)].DrawCentered(Center);
			}
			else
			{
				GFX.Game[spritePath + "/x"].DrawCentered(Center);
			}
			float num = flash * 4f;
			Draw.Rect(X - num, Y - num, Width + num * 2f, Height + num * 2f, Color.White * flash);
			Position = position;
		}
		private void ActivateParticles()
		{
			bool flag = direction == VitMoveBlock.Directions.Down || direction == VitMoveBlock.Directions.Up;
			if ((!canSteer || !flag) && !CollideCheck<Player>(Position - Vector2.UnitX))
			{
				SceneAs<Level>().ParticlesBG.Emit(P_Activate, (int)(Height / 2f), CenterLeft, Vector2.UnitY * (Height - 4f) * 0.5f, 3.14159274f);
			}
			if ((!canSteer || !flag) && !CollideCheck<Player>(Position + Vector2.UnitX))
			{
				SceneAs<Level>().ParticlesBG.Emit(P_Activate, (int)(Height / 2f), CenterRight, Vector2.UnitY * (Height - 4f) * 0.5f, 0f);
			}
			if ((!canSteer || flag) && !CollideCheck<Player>(Position - Vector2.UnitY))
			{
				SceneAs<Level>().ParticlesBG.Emit(P_Activate, (int)(Width / 2f), TopCenter, Vector2.UnitX * (Width - 4f) * 0.5f, -1.57079637f);
			}
			SceneAs<Level>().ParticlesBG.Emit(P_Activate, (int)(Width / 2f), BottomCenter, Vector2.UnitX * (Width - 4f) * 0.5f, 1.57079637f);
		}
		private void BreakParticles()
		{
			Vector2 center = Center;
			int num = 0;
			while (num < Width)
			{
				int num2 = 0;
				while (num2 < Height)
				{
					Vector2 vector = Position + new Vector2((2 + num), (2 + num2));
					SceneAs<Level>().Particles.Emit(P_Break, 1, vector, Vector2.One * 2f, (vector - center).Angle());
					num2 += 4;
				}
				num += 4;
			}
		}
		private void MoveParticles()
		{
			Vector2 position;
			Vector2 vector;
			float num;
			float num2;
			if (direction == VitMoveBlock.Directions.Right)
			{
				position = CenterLeft + Vector2.UnitX;
				vector = Vector2.UnitY * (Height - 4f);
				num = 3.14159274f;
				num2 = Height / 32f;
			}
			else if (direction == VitMoveBlock.Directions.Left)
			{
				position = CenterRight;
				vector = Vector2.UnitY * (Height - 4f);
				num = 0f;
				num2 = Height / 32f;
			}
			else if (direction == VitMoveBlock.Directions.Down)
			{
				position = TopCenter + Vector2.UnitY;
				vector = Vector2.UnitX * (Width - 4f);
				num = -1.57079637f;
				num2 = Width / 32f;
			}
			else
			{
				position = BottomCenter;
				vector = Vector2.UnitX * (Width - 4f);
				num = 1.57079637f;
				num2 = Width / 32f;
			}
			particleRemainder += num2;
			int num3 = (int)particleRemainder;
			particleRemainder -= num3;
			vector *= 0.5f;
			if (num3 > 0)
			{
				SceneAs<Level>().ParticlesBG.Emit(P_Move, num3, position, vector, num);
			}
		}
		private void ScrapeParticles(Vector2 dir)
		{
			bool collidable = Collidable;
			Collidable = false;
			if (dir.X != 0f)
			{
				float x;
				if (dir.X > 0f)
				{
					x = Right;
				}
				else
				{
					x = Left - 1f;
				}
				int num = 0;
				while (num < Height)
				{
					Vector2 vector = new Vector2(x, Top + 4f + num);
					if (Scene.CollideCheck<Solid>(vector))
					{
						SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector);
					}
					num += 8;
				}
			}
			else
			{
				float y;
				if (dir.Y > 0f)
				{
					y = Bottom;
				}
				else
				{
					y = Top - 1f;
				}
				int num2 = 0;
				while (num2 < Width)
				{
					Vector2 vector2 = new Vector2(Left + 4f + num2, y);
					if (Scene.CollideCheck<Solid>(vector2))
					{
						SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, vector2);
					}
					num2 += 8;
				}
			}
			Collidable = true;
		}

		public static ParticleType P_Activate;

		public static ParticleType P_Break;

		public static ParticleType P_Move;

		public int remote;

		public bool triggered;

		public bool heldTopButton;

		public bool heldSideButton;

		private bool canSteer;

		private bool canActivate;

		private Directions direction;

		private string spritePath;

		private float moveSpeed;

		private float homeAngle;

		private int angleSteerSign;

		private Vector2 startPosition;

		public MovementState state;

		private bool leftPressed;

		private bool rightPressed;

		private bool topPressed;

		private float speed;

		private float targetSpeed;

		private float angle;

		private float targetAngle;

		private Player noSquish;

		private List<Image> body;

		private List<Image> topButton;

		private List<Image> leftButton;

		private List<Image> rightButton;

		private List<MTexture> arrows;

		private Border border;

		private Color fillColor;

		private Color idleBgFill;

		private Color activeBgFill = Calc.HexToColor("4fd6ff");

		private Color idleRemoteBgFill = Calc.HexToColor("9e45b2");

		private Color activeRemoteBgFill = Calc.HexToColor("ff8cf5");

		private Color breakingBgFill = Calc.HexToColor("cc2541");

		private float flash;

		private SoundSource moveSfx;

		private float particleRemainder;

		public enum Directions
		{
			Left,
			Right,
			Up,
			Down
		}

		public enum MovementState
		{
			Idling,
			Moving,
			Breaking
		}

		private class Border : Entity
		{
			public Border(VitMoveBlock parent)
			{
				Parent = parent;
				Depth = 1;
			}

			public override void Update()
			{
				if (Parent.Scene != Scene)
				{
					RemoveSelf();
				}
				base.Update();
			}

			public override void Render()
			{
				Draw.Rect(Parent.X + Parent.Shake.X - 1f, Parent.Y + Parent.Shake.Y - 1f, Parent.Width + 2f, Parent.Height + 2f, Color.Black);
			}

			public VitMoveBlock Parent;
		}

		private class Debris : Actor
		{
			public Debris() : base(Vector2.Zero)
			{
				Tag = Tags.TransitionUpdate;
				Collider = new Hitbox(4f, 4f, -2f, -2f);
				Add(sprite = new Image(Calc.Random.Choose(GFX.Game.GetAtlasSubtextures("objects/vitMoveBlock/debris"))));
				sprite.CenterOrigin();
				sprite.FlipX = Calc.Random.Chance(0.5f);
				onCollideH = delegate (CollisionData c)
				{
					speed.X = -speed.X * 0.5f;
				};
				onCollideV = delegate (CollisionData c)
				{
					if (firstHit || speed.Y > 50f)
					{
						Audio.Play("event:/game/general/debris_stone", Position, "debris_velocity", Calc.ClampedMap(speed.Y, 0f, 600f, 0f, 1f));
					}
					if (speed.Y > 0f && speed.Y < 40f)
					{
						speed.Y = 0f;
					}
					else
					{
						speed.Y = -speed.Y * 0.25f;
					}
					firstHit = false;
				};
			}
			public override void OnSquish(CollisionData data) { }

			public VitMoveBlock.Debris Init(Vector2 position, Vector2 center, Vector2 returnTo)
			{
				Collidable = true;
				Position = position;
				speed = (position - center).SafeNormalize(60f + Calc.Random.NextFloat(60f));
				home = returnTo;
				sprite.Position = Vector2.Zero;
				sprite.Rotation = Calc.Random.NextAngle();
				returning = false;
				shaking = false;
				sprite.Scale.X = 1f;
				sprite.Scale.Y = 1f;
				sprite.Color = Color.White;
				alpha = 1f;
				firstHit = false;
				spin = Calc.Random.Range(3.49065852f, 10.4719753f) * Calc.Random.Choose(1, -1);
				return this;
			}

			public override void Update()
			{
				base.Update();
				if (!returning)
				{
					if (Collidable)
					{
						speed.X = Calc.Approach(speed.X, 0f, Engine.DeltaTime * 100f);
						if (!OnGround(1))
						{
							speed.Y = speed.Y + 400f * Engine.DeltaTime;
						}
						MoveH(speed.X * Engine.DeltaTime, onCollideH, null);
						MoveV(speed.Y * Engine.DeltaTime, onCollideV, null);
					}
					if (shaking && Scene.OnInterval(0.05f))
					{
						sprite.X = (-1 + Calc.Random.Next(3));
						sprite.Y = (-1 + Calc.Random.Next(3));
					}
				}
				else
				{
					Position = returnCurve.GetPoint(Ease.CubeOut(returnEase));
					returnEase = Calc.Approach(returnEase, 1f, Engine.DeltaTime / returnDuration);
					sprite.Scale = Vector2.One * (1f + returnEase * 0.5f);
				}
				if ((Scene as Level).Transitioning)
				{
					alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime * 4f);
					sprite.Color = Color.White * alpha;
				}
				sprite.Rotation += spin * Calc.ClampedMap(Math.Abs(speed.Y), 50f, 150f, 0f, 1f) * Engine.DeltaTime;
			}

			public void StopMoving()
			{
				Collidable = false;
			}

			public void StartShaking()
			{
				shaking = true;
			}

			public void ReturnHome(float duration)
			{
				if (Scene != null)
				{
					Camera camera = (Scene as Level).Camera;
					if (X < camera.X)
					{
						X = camera.X - 8f;
					}
					if (Y < camera.Y)
					{
						Y = camera.Y - 8f;
					}
					if (X > camera.X + 320f)
					{
						X = camera.X + 320f + 8f;
					}
					if (Y > camera.Y + 180f)
					{
						Y = camera.Y + 180f + 8f;
					}
				}
				returning = true;
				returnEase = 0f;
				returnDuration = duration;
				Vector2 vector = (home - Position).SafeNormalize();
				Vector2 control = (Position + home) / 2f + new Vector2(vector.Y, -vector.X) * (Calc.Random.NextFloat(16f) + 16f) * Calc.Random.Facing();
				returnCurve = new SimpleCurve(Position, home, control);
			}

			private Image sprite;

			private Vector2 home;

			private Vector2 speed;

			private bool shaking;

			private bool returning;

			private float returnEase;

			private float returnDuration;

			private SimpleCurve returnCurve;

			private bool firstHit;

			private float alpha;

			private Collision onCollideH;

			private Collision onCollideV;

			private float spin;
		}
	}
}