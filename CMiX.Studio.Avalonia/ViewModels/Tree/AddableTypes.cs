// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class AddableTypes
    {
        private static readonly Dictionary<Type, IReadOnlyList<Type>> cache = new();

        public static IReadOnlyList<Type> ForOwner(Type ownerType) =>
            Cached(ownerType, () => typeof(ModifierPanelAttribute).Assembly
                .GetTypes()
                .Concat(ownerType.Assembly.GetTypes())
                .Distinct()
                .Where(IsAddable)
                .Where(type => type.IsDefined(typeof(ModifierPanelAttribute), true) && ListRules.CanHold(ownerType, type)));

        public static IReadOnlyList<Type> AllModifiers() =>
            Cached(typeof(ModifierPanelAttribute), () => typeof(ModifierPanelAttribute).Assembly
                .GetTypes()
                .Where(IsAddable)
                .Where(type => type.IsDefined(typeof(ModifierPanelAttribute), true)));

        public static IReadOnlyList<Type> ForInterface(Type interfaceType) =>
            Cached(interfaceType, () => interfaceType.Assembly
                .GetTypes()
                .Where(IsAddable)
                .Where(interfaceType.IsAssignableFrom));

        // One column up to 4 items, two up to 9, else three. Each column fills top down.
        public static IReadOnlyList<IReadOnlyList<T>> Columns<T>(IReadOnlyList<T> items)
        {
            int columns = items.Count switch { <= 4 => 1, <= 9 => 2, _ => 3 };
            int rows = (int)Math.Ceiling((double)items.Count / columns);

            return Enumerable.Range(0, columns)
                .Select(column => (IReadOnlyList<T>)items.Skip(column * rows).Take(rows).ToList())
                .ToList();
        }

        private static readonly string[] Suffixes = { "Modifier", "Modulator" };

        // A modifier or a modulator drops the word Modifier or Modulator: TranslateModifier gives Translate.
        public static string LabelOf(Type type)
        {
            if (ControlFactory.StringHelper.LabelOf(type) is { } label)
                return label;

            var name = type.Name;
            if (IsModifierOrModulator(type))
            {
                foreach (var suffix in Suffixes)
                {
                    if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal))
                    {
                        name = name[..^suffix.Length];
                        break;
                    }
                }
            }

            return ControlFactory.StringHelper.PascalCaseToDisplay(name);
        }

        public static AddEntry ContentEntryOf(Type type) =>
            type == typeof(FullscreenQuad) ? new AddEntry(TreeAddIds.FullscreenQuad, "Fullscreen Quad")
                : type == typeof(TextEntity) ? new AddEntry(TreeAddIds.TextEntity, "Text")
                : type == typeof(Camera) ? new AddEntry(TreeAddIds.Camera, "Camera")
                : type == typeof(LightEntity) ? new AddEntry(TreeAddIds.Light, "Light")
                : new AddEntry(TreeAddIds.Entity, "Entity");

        private static bool IsModifierOrModulator(Type type) =>
            typeof(IModifier).IsAssignableFrom(type)
            || typeof(IModulator).IsAssignableFrom(type)
            || type.IsDefined(typeof(ModifierPanelAttribute), true);

        private static bool IsAddable(Type type) =>
            !type.IsAbstract && !type.IsInterface && type.GetCustomAttribute<HiddenInMenuAttribute>() == null;

        private static IReadOnlyList<Type> Cached(Type key, Func<IEnumerable<Type>> discover)
        {
            lock (cache)
            {
                if (!cache.TryGetValue(key, out var types))
                    cache[key] = types = discover().OrderBy(LabelOf).ToList();

                return types;
            }
        }
    }
}
