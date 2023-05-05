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
    [CustomEntity("vitellary/resetdoortrigger")]
    [Tracked(false)]
    public class ResetDoorTrigger : Trigger
    {
        public static void Load()
        {
            On.Celeste.LockBlock.ctor_EntityData_Vector2_EntityID += LockBlock_ctor_EntityData_Vector2_EntityID;
            On.Celeste.LockBlock.UnlockRoutine += LockBlock_UnlockRoutine;
            On.Celeste.Key.ctor_EntityData_Vector2_EntityID += Key_ctor_EntityData_Vector2_EntityID;
            On.Celeste.Key.OnPlayer += Key_OnPlayer;
        }

        public static void Unload()
        {
            On.Celeste.LockBlock.ctor_EntityData_Vector2_EntityID -= LockBlock_ctor_EntityData_Vector2_EntityID;
            On.Celeste.LockBlock.UnlockRoutine -= LockBlock_UnlockRoutine;
            On.Celeste.Key.ctor_EntityData_Vector2_EntityID -= Key_ctor_EntityData_Vector2_EntityID;
            On.Celeste.Key.OnPlayer -= Key_OnPlayer;
        }

        public ResetDoorTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            oneUse = data.Bool("oneUse", false);
            animate = data.Bool("animate", true);
            onlyInRoom = data.Bool("onlyInRoom", false);
        }

        public override void OnEnter(Player player)
        {
            Level level = SceneAs<Level>();
            Session session = level.Session;
            foreach (EntityID entityID in VitModule.Session.doorsToReset)
            {
                if (entityID.Level == session.Level || !onlyInRoom)
                {
                    session.DoNotLoad.Remove(entityID);
                }
                if (entityID.Level == session.Level) //currently loaded room
                {
                    LockBlock door = new LockBlock(VitModule.Session.doorEntityDatas[entityID], VitModule.Session.doorOffsets[entityID], entityID);
                    level.Add(door);
                    if (animate)
                    {
                        door.Add(new Coroutine(DoorAnimation(door)));
                    }
                }
            }
            VitModule.Session.doorsToReset.Clear();
            foreach (EntityID entityID in VitModule.Session.introCarsToReset)
            {
                if (entityID.Level == session.Level || !onlyInRoom)
                {
                    session.DoNotLoad.Remove(entityID);
                }
                if (entityID.Level == session.Level) //currently loaded room
                {
                    IntroLockedCar car = new IntroLockedCar(VitModule.Session.introCarEntityDatas[entityID], VitModule.Session.introCarOffsets[entityID], entityID);
                    level.Add(car);
                    if (animate)
                    {
                        car.Add(new Coroutine(CarAnimation(car)));
                    }
                }
            }
            VitModule.Session.introCarsToReset.Clear();
            foreach (EntityID entityID in VitModule.Session.keysToReset)
            {
                if (!session.Keys.Contains(entityID))
                {
                    if (entityID.Level == session.Level || !onlyInRoom)
                    {
                        session.DoNotLoad.Remove(entityID);
                    }
                    if (entityID.Level == session.Level && VitModule.Session.keyEntityDatas.ContainsKey(entityID))
                    {
                        Key key = new Key(VitModule.Session.keyEntityDatas[entityID], VitModule.Session.keyOffsets[entityID], entityID);
                        level.Add(key);
                        if (animate)
                        {
                            level.ParticlesFG.Emit(P_Respawn, 10, key.Position, Vector2.One * 4f);
                        }
                    }
                }
            }
            VitModule.Session.keysToReset.Clear();
            foreach (EntityID entityID in VitModule.Session.keyberriesToReset)
            {
                if (!session.Keys.Contains(entityID))
                {
                    if (entityID.Level == session.Level || !onlyInRoom)
                    {
                        session.DoNotLoad.Remove(entityID);
                    }
                    if (entityID.Level == session.Level)
                    {
                        KeyBerry keyberry = new KeyBerry(VitModule.Session.keyberryEntityDatas[entityID], VitModule.Session.keyberryOffsets[entityID], entityID);
                        level.Add(keyberry);
                        if (animate)
                        {
                            level.ParticlesFG.Emit(P_Respawn, 10, keyberry.Position, Vector2.One * 4f);
                        }
                    }
                }
            }
            VitModule.Session.keyberriesToReset.Clear();
            if (VitModule.frostHelperLoaded)
            {
                KeyIceReset(level, session);
            }
            if (oneUse)
            {
                RemoveSelf();
            }
        }

        private void KeyIceReset(Level level, Session session)
        {
            foreach (EntityID entityID in VitModule.Session.keyIcesToReset)
            {
                if (!session.Keys.Contains(entityID))
                {
                    if (entityID.Level == session.Level || !onlyInRoom)
                    {
                        session.DoNotLoad.Remove(entityID);
                    }
                    if (entityID.Level == session.Level)
                    {
                        FrostHelper.KeyIce key = new FrostHelper.KeyIce(VitModule.Session.keyIceEntityDatas[entityID], VitModule.Session.keyIceOffsets[entityID], entityID, VitModule.Session.keyIceNodes[entityID]);
                        level.Add(key);
                        if (animate)
                        {
                            level.ParticlesFG.Emit(P_IceRespawn, 10, key.Position, Vector2.One * 4f);
                        }
                    }
                }
            }
            VitModule.Session.keyIcesToReset.Clear();
        }

        private IEnumerator DoorAnimation(LockBlock door)
        {
            Sprite sprite = door.sprite;
            //reverse is broken, so we have to do it manually
            sprite.Play("burst", true);
            for (int i = sprite.CurrentAnimationTotalFrames-1; i >= 0; i--)
            {
                sprite.SetAnimationFrame(i);
                yield return null;
            }
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            yield return 0.06f;
            sprite.Play("open", true);
            for (int i = sprite.CurrentAnimationTotalFrames-1; i >= 0; i--)
            {
                sprite.SetAnimationFrame(i);
                yield return null;
            }
            sprite.Play("idle", true);
            yield break;
        }

        private IEnumerator CarAnimation(IntroLockedCar car)
        {
            car.bodySprite.Play("burst", true);
            car.wheelSprite.Play("burst", true);
            for (int i = car.bodySprite.CurrentAnimationTotalFrames - 1; i >= 0; i--)
            {
                car.bodySprite.SetAnimationFrame(i);
                car.wheelSprite.SetAnimationFrame(i);
                yield return null;
            }
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            yield return 0.06f;
            car.bodySprite.Play("open", true);
            car.wheelSprite.Play("idle", true);
            for (int i = car.bodySprite.CurrentAnimationTotalFrames - 1; i >= 0; i--)
            {
                car.bodySprite.SetAnimationFrame(i);
                yield return null;
            }
            car.bodySprite.Play("idle", true);
            yield break;
        }

        public static void LockBlock_ctor_EntityData_Vector2_EntityID(On.Celeste.LockBlock.orig_ctor_EntityData_Vector2_EntityID orig, LockBlock self, EntityData data, Vector2 offset, EntityID id)
        {
            orig(self, data, offset, id);
            if (!VitModule.Session.doorEntityDatas.ContainsKey(id))
            {
                VitModule.Session.doorEntityDatas.Add(id, data);
                VitModule.Session.doorOffsets.Add(id, offset);
            }
        }

        public static IEnumerator LockBlock_UnlockRoutine(On.Celeste.LockBlock.orig_UnlockRoutine orig, LockBlock self, Follower fol)
        {
            IEnumerator orig_enum = orig(self, fol);
            CustomWindController wind = self.Scene.Tracker.GetEntity<CustomWindController>();
            while (orig_enum.MoveNext())
            {
                if (wind != null && wind.activateType == "Locked Door" && self.sprite.CurrentAnimationID == "burst")
                {
                    wind.ActivateWind();
                }
                yield return orig_enum.Current;
            }
            VitModule.Session.doorsToReset.Add(self.ID);
            yield break;
        }

        public static void Key_ctor_EntityData_Vector2_EntityID(On.Celeste.Key.orig_ctor_EntityData_Vector2_EntityID orig, Key self, EntityData data, Vector2 offset, EntityID id)
        {
            orig(self, data, offset, id);
            if (!VitModule.Session.keyEntityDatas.ContainsKey(id))
            {
                VitModule.Session.keyEntityDatas.Add(id, data);
                VitModule.Session.keyOffsets.Add(id, offset);
            }
        }

        public static void Key_OnPlayer(On.Celeste.Key.orig_OnPlayer orig, Key self, Player player)
        {
            orig(self, player);
            VitModule.Session.keysToReset.Add(self.ID);
        }

        private delegate void KeyIce_orig_ctor(Entity self, EntityData data, Vector2 offset, EntityID id, Vector2[] nodes);
        private static void KeyIce_ctor(KeyIce_orig_ctor orig, Entity self, EntityData data, Vector2 offset, EntityID id, Vector2[] nodes)
        {
            orig(self, data, offset, id, nodes);
            if (!VitModule.Session.keyIceEntityDatas.ContainsKey(id))
            {
                VitModule.Session.keyIceEntityDatas.Add(id, data);
                VitModule.Session.keyIceOffsets.Add(id, offset);
                VitModule.Session.keyIceNodes.Add(id, nodes);
            }
        }

        private delegate void KeyIce_orig_Update(Entity self);
        private static void KeyIce_Update(KeyIce_orig_Update orig, Entity self)
        {
            orig(self);
            FrostHelper.KeyIce key = self as FrostHelper.KeyIce;
            if (self.SceneAs<Level>().Session.DoNotLoad.Contains(key.ID) && !VitModule.Session.keyIcesToReset.Contains(key.ID))
            {
                VitModule.Session.keyIcesToReset.Add(key.ID);
            }
            else if (!self.SceneAs<Level>().Session.DoNotLoad.Contains(key.ID) && VitModule.Session.keyIcesToReset.Contains(key.ID) && !key.IsUsed)
            {
                VitModule.Session.keyIcesToReset.Remove(key.ID);
            }
        }

        private delegate IEnumerator KeyIce_orig_DissolveRoutine(Entity self);
        private static IEnumerator KeyIce_DissolveRoutine(KeyIce_orig_DissolveRoutine orig, Entity self)
        {
            IEnumerator orig_enum = orig(self);
            while (orig_enum.MoveNext())
            {
                yield return orig_enum.Current;
            }
            Key key = self as Key;
            if (VitModule.Session.keyIcesToReset.Contains(key.ID))
            {
                VitModule.Session.keyIcesToReset.Remove(key.ID);
            }
            yield break;
        }

        private bool oneUse;

        private bool animate;

        private bool onlyInRoom;

        public static ParticleType P_IceRespawn;

        public static ParticleType P_Respawn;
    }
}
