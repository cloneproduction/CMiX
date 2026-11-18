// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public static class TreeDropText
    {
        public static string CannotHold(IControl item, Type ownerType) =>
            $"{Capitalized(WithArticle(ItemName(item)))} cannot go to {WithArticle(OwnerName(ownerType))}";

        public static string CannotTake(TreeNode inside, TreeNode source) =>
            $"\"{NameOfList(inside)}\" cannot take \"{source.Header}\"";

        public static string AlsoHeldBy(IReadOnlyList<Layer> sharing)
        {
            var names = string.Join(" and ", sharing.Select(layer => $"\"{layer.PrefabService.Name.Value}\""));
            return sharing.Count == 1 ? $"Layer {names} also holds it" : $"Layers {names} also hold it";
        }

        public static string SharesWithOtherLayers(IReadOnlyList<IPrefab> blocking) =>
            $"It shares {string.Join(", ", blocking.Select(item => $"\"{item.PrefabService.Name.Value}\""))} with other layers";

        public static string DescribeBlock(TreeNode source, TreeNode target, IReadOnlyList<IPrefab> blocking)
        {
            if (source.Item is not Layer)
            {
                var toLayer = (target.Item is Layer ? target : target.Item == null ? target.Parent : target.Parent?.Parent)?.Header ?? string.Empty;
                var fromComposition = TreeRowOrder.CompositionOf(source)?.Header ?? string.Empty;
                var layers = string.Join(", ", blocking.Select(layer => $"\"{layer.PrefabService.Name.Value}\""));

                return $"Cannot move \"{source.Header}\" to \"{toLayer}\".\n"
                    + $"This item is also used by other layers of \"{fromComposition}\": {layers}.\n"
                    + "An item can belong to one composition only. "
                    + "Remove the item from the other layers first, or duplicate it, then move it.";
            }

            var layer = source.Header;
            var from = TreeRowOrder.CompositionOf(source)?.Header ?? string.Empty;
            var to = TreeRowOrder.CompositionOf(target)?.Header ?? string.Empty;
            var names = string.Join(", ", blocking.Select(item => item.PrefabService.Name.Value));

            return $"Cannot move \"{layer}\" to \"{to}\".\n"
                + $"This layer shares items with other layers of \"{from}\": {names}.\n"
                + "An item can belong to one composition only. "
                + "Remove the item from the other layers first, or duplicate it, then move the layer.";
        }

        private static string NameOfList(TreeNode inside)
        {
            var holder = inside.Item == null ? inside : inside.Parent;
            return (holder?.Item == null ? holder?.Parent : holder)?.Header ?? string.Empty;
        }

        // TranslateModifier is Translate.
        private static string ItemName(IControl item)
        {
            var name = item.GetType().Name;
            foreach (var suffix in new[] { "Modifier", "Modulator", "Filter" })
                if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal))
                    return name[..^suffix.Length];

            return name;
        }

        // LightEntity is light entity.
        private static string OwnerName(Type type) =>
            string.Concat(type.Name.Select((letter, index) => index > 0 && char.IsUpper(letter) ? " " + char.ToLowerInvariant(letter) : char.ToLowerInvariant(letter).ToString()));

        private static string WithArticle(string name) =>
            (name.Length > 0 && "aeiouAEIOU".Contains(name[0]) ? "an " : "a ") + name;

        private static string Capitalized(string text) =>
            text.Length > 0 ? char.ToUpperInvariant(text[0]) + text[1..] : text;
    }
}
