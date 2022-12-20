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
	[CustomEntity("vitellary/bloomstrengthtrigger")]
	[Tracked(false)]
	public class BloomStrengthTrigger : Trigger
    {
		public BloomStrengthTrigger(EntityData data, Vector2 offset) : base(data, offset)
		{
			bloomAddFrom = data.Float("bloomStrengthFrom", 1f);
			bloomAddTo = data.Float("bloomStrengthTo", 1f);
			positionMode = data.Enum("positionMode", PositionModes.NoEffect);
		}

		public override void OnStay(Player player)
		{
			(Scene as Level).Bloom.Strength = bloomAddFrom + (bloomAddTo - bloomAddFrom) * MathHelper.Clamp(GetPositionLerp(player, positionMode), 0f, 1f);
		}

		public float bloomAddFrom;

		public float bloomAddTo;

		// Token: 0x0400199B RID: 6555
		public PositionModes positionMode;
	}
}
