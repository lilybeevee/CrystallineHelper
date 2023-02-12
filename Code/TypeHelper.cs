using Celeste.Mod;
using Celeste.Mod.Helpers;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace vitmod {
    /// <summary>
    /// Provides utility functions for turning strings to types
    /// </summary>
    internal static class TypeHelper {
        private static Dictionary<string, HashSet<Type>> TypeListCache = new(StringComparer.Ordinal);
        private static Type[] AllEntityTypes;

        /// <summary>
        /// Parses a comma-seperated list of c# type full names or short names. Cached.
        /// </summary>
        public static HashSet<Type> ParseTypeList(string list) {
            if (TypeListCache.TryGetValue(list, out var result)) {
                return result;
            }

            var split = list.Split(',');

            AllEntityTypes ??= FakeAssembly.GetFakeEntryAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Entity))).ToArray();

            result = new(AllEntityTypes.Where(t => split.Contains(t.FullName, StringComparer.Ordinal) || split.Contains(t.Name, StringComparer.Ordinal)));
            TypeListCache[list] = result;

            return result;
        }

        internal static void Load() {
            On.Celeste.Mod.Everest.Loader.LoadModAssembly += Loader_LoadModAssembly;
        }

        internal static void Unload() {
            On.Celeste.Mod.Everest.Loader.LoadModAssembly -= Loader_LoadModAssembly;
        }

        // Clear the cache if a mod is loaded/hot reloaded
        private static void Loader_LoadModAssembly(On.Celeste.Mod.Everest.Loader.orig_LoadModAssembly orig, EverestModuleMetadata meta, Assembly asm) {
            orig(meta, asm);
            AllEntityTypes = null;
            TypeListCache.Clear();
        }
    }
}
