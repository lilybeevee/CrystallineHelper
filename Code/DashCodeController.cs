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

namespace vitmod {
    [CustomEntity("vitellary/dashcodecontroller")]
    [Tracked]
    public class DashCodeController : Entity {
        public Vector2[] DashCode;
        public string FlagLabel;
        public string FailureFlag;
        public int Index;

        private int codePosition;

        private DashCodeDisplay display;

        public DashCodeController(EntityData data, Vector2 offset)
            : base(data.Position + offset) {
            DashCode = data.Attr("dashCode", "*").ToUpper()
                .Split(',').Select(s => s switch {
                    "U" => -Vector2.UnitY,
                    "D" => Vector2.UnitY,
                    "L" => -Vector2.UnitX,
                    "R" => Vector2.UnitX,
                    "UL" => new Vector2(-1, -1),
                    "UR" => new Vector2(1, -1),
                    "DL" => new Vector2(-1, 1),
                    "DR" => new Vector2(1, 1),
                    _ => Vector2.Zero,
                }).ToArray();

            FlagLabel = data.Attr("flagLabel", "");
            FailureFlag = data.Attr("flagOnFailure", "");
            Index = data.Int("index");

            //stole this code from maddie's helping hand set flag on spawn trigger hope u don't mind
            Level level = null;
            if (Engine.Scene is Level) {
                level = Engine.Scene as Level;
            } else if (Engine.Scene is LevelLoader) {
                level = (Engine.Scene as LevelLoader).Level;
            }

            if (level != null) {
                for (int i = 1; i <= DashCode.Length; i++) {
                    if (i < 10) {
                        level.Session.SetFlag(FlagLabel + "_0" + i, false);
                    } else {
                        level.Session.SetFlag(FlagLabel + "_" + i, false);
                    }
                }
                if (!string.IsNullOrEmpty(FailureFlag)) {
                    level.Session.SetFlag(FailureFlag, false);
                }
            }
            Add(new DashListener(OnDash));
            codePosition = 0;
        }


        public override void Update() {
            base.Update();
            if (display == null) {
                display = Scene.Tracker.GetEntity<DashCodeDisplay>();
            }
        }

        private void OnDash(Vector2 direction) {
            if (display != null && Index == display.Index) {
                Vector2 dir = Calc.Sign(direction);
                if (DashCode[codePosition] == Vector2.Zero || dir == DashCode[codePosition]) {
                    if (codePosition + 1 < 10) {
                        SceneAs<Level>().Session.SetFlag(FlagLabel + "_0" + (codePosition + 1), true);
                    } else {
                        SceneAs<Level>().Session.SetFlag(FlagLabel + "_" + (codePosition + 1), true);
                    }
                    codePosition++;
                    display.ValidateInput();
                    if (codePosition >= DashCode.Length) {
                        RemoveSelf();
                    }
                } else {
                    for (int i = 1; i <= DashCode.Length; i++) {
                        if (i < 10) {
                            SceneAs<Level>().Session.SetFlag(FlagLabel + "_0" + i, false);
                        } else {
                            SceneAs<Level>().Session.SetFlag(FlagLabel + "_" + i, false);
                        }
                    }
                    if (codePosition != 0 && !string.IsNullOrEmpty(FailureFlag)) {
                        SceneAs<Level>().Session.SetFlag(FailureFlag, true);
                    }
                    codePosition = 0;
                    display.Fail();
                }
            }
        }
    }
}