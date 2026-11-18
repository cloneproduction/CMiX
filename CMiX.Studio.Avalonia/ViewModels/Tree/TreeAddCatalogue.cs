// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed class TreeAddCatalogue
    {
        private static readonly Type[] ContentTypes = { typeof(Entity), typeof(FullscreenQuad), typeof(TextEntity), typeof(Camera), typeof(LightEntity) };

        private readonly PrefabManager compositions;

        public TreeAddCatalogue(PrefabManager compositions)
        {
            this.compositions = compositions;
        }

        public IReadOnlyList<TreeAddGroup> GroupsFor(TreeNode? target)
        {
            var available = AvailableFor(target);

            TreeAdd Entry(string id, string header, TreeCategory category) =>
                available.TryGetValue(id, out var add)
                    ? add with { Header = header, Category = category }
                    : new TreeAdd(id, header, Category: category, Enabled: false);

            TreeAddGroup Held(TreeCategory category, IReadOnlyList<Type> types)
            {
                var name = TreeCategories.Label(category, 1);
                var entries = types.Select(type => Entry(TreeAddIds.ForType(type), AddableTypes.LabelOf(type), category)).ToList();
                return new TreeAddGroup("group.add-" + name.ToLowerInvariant(), name, category, entries);
            }

            var content = ContentTypes.Select(type =>
            {
                var entry = AddableTypes.ContentEntryOf(type);
                return Entry(entry.Id, entry.Label, ContentCategoryOf(type));
            }).ToList();

            return new[]
            {
                new TreeAddGroup("group.layer", "Layer", TreeCategory.Composable,
                    new[] { Entry(TreeAddIds.Layer, "Layer", TreeCategory.Composable) }),
                new TreeAddGroup("group.content", "Content", null, content),
                Held(TreeCategory.Texture, AddableTypes.ForInterface(typeof(ITextureSource))),
                Held(TreeCategory.Filter, AddableTypes.ForInterface(typeof(ITextureFilter))),
                Held(TreeCategory.Modifier, AddableTypes.AllModifiers()),
                Held(TreeCategory.Modulator, AddableTypes.ForInterface(typeof(IModulator))),
                new TreeAddGroup("group.project", "Project", TreeCategory.Composable,
                    new[] { Entry(TreeAddIds.Composition, "Composition", TreeCategory.Composable) }),
            };
        }

        // The composition can be added whatever the target is, so it comes last for every target.
        public IReadOnlyList<TreeAdd> ChoicesFor(TreeNode? target)
        {
            var choices = target == null ? new List<TreeAdd>() : OwnChoicesOf(target).ToList();
            choices.Add(CompositionChoice());
            return choices;
        }

        private IReadOnlyList<TreeAdd> OwnChoicesOf(TreeNode node)
        {
            if (node.Item is Composition composition)
                return new[] { AddOf(TreeAddIds.Layer, "Add Layer", composition.LayerManager, typeof(Layer)) }.Concat(KindChoicesOf(composition)).ToList();

            if (node.Item is Layer layer)
                return TreeGroup.GroupsOf(layer).OfType<LayerGroup>()
                    .SelectMany(group => group.AddableTypes.Select(type => LayerAddOf(group, type, string.Empty)))
                    .Concat(KindChoicesOf(layer))
                    .ToList();

            if (node.Item != null)
                return KindChoicesOf(node.Item);

            return node.Group switch
            {
                LayerGroup layerGroup => layerGroup.AddableTypes.Select(type => LayerAddOf(layerGroup, type, "Add ")).ToList(),
                LayersGroup layersGroup => new[] { AddOf(TreeAddIds.Layer, "Add Layer", layersGroup.Manager, typeof(Layer)) },
                CompositionsGroup => new[] { CompositionChoice() },
                { } held => AddsOfTypes(held),
                _ => Array.Empty<TreeAdd>(),
            };
        }

        private TreeAdd CompositionChoice() =>
            AddOf(TreeAddIds.Composition, "Add Composition", compositions, typeof(Composition));

        private static IReadOnlyList<TreeAdd> KindChoicesOf(IControl item) =>
            HeldGroupsOf(item)
                .Select(group => TreeCategories.Label(group.Category, 1))
                .Select(name => new TreeAdd(TreeAddIds.ForKindMenu(name), "Add " + name))
                .ToList();

        private Dictionary<string, TreeAdd> AvailableFor(TreeNode? target)
        {
            var available = new Dictionary<string, TreeAdd>();
            foreach (var add in AvailableAddsOf(target))
                available.TryAdd(add.Id, add);

            available.TryAdd(TreeAddIds.Composition, AddOf(TreeAddIds.Composition, "Composition", compositions, typeof(Composition)));
            return available;
        }

        private static IEnumerable<TreeAdd> AvailableAddsOf(TreeNode? target) =>
            target switch
            {
                { Item: Composition composition } =>
                    new[] { AddOf(TreeAddIds.Layer, "Layer", composition.LayerManager, typeof(Layer)) }.Concat(HeldGroupsOf(composition).SelectMany(AddsOfTypes)),
                { Item: Layer layer } => TreeGroup.GroupsOf(layer).SelectMany(AvailableAddsOfLayerGroup),
                { Item: { } item } => HeldGroupsOf(item).SelectMany(AddsOfTypes),
                { Group: LayersGroup layersGroup } => new[] { AddOf(TreeAddIds.Layer, "Layer", layersGroup.Manager, typeof(Layer)) },
                { Group: CompositionsGroup } => Array.Empty<TreeAdd>(),
                { Group: LayerGroup layerGroup } => LayerItemsOf(layerGroup),
                { Group: { } held } => AddsOfTypes(held),
                _ => Array.Empty<TreeAdd>(),
            };

        private static IEnumerable<TreeAdd> AvailableAddsOfLayerGroup(TreeGroup group) =>
            group is LayerGroup layerGroup ? LayerItemsOf(layerGroup) : AddsOfTypes(group);

        private static IEnumerable<TreeAdd> LayerItemsOf(LayerGroup group) =>
            group.AddableTypes.Select(type => LayerAddOf(group, type, string.Empty));

        private static IEnumerable<TreeGroup> HeldGroupsOf(IControl item) =>
            TreeGroup.GroupsOf(item).Where(group => group is not (LayerGroup or StructureGroup));

        private static TreeCategory ContentCategoryOf(Type type) =>
            type == typeof(Camera) ? TreeCategory.Camera : type == typeof(LightEntity) ? TreeCategory.Light : TreeCategory.Entity;

        private static IReadOnlyList<TreeAdd> AddsOfTypes(TreeGroup group) =>
            group.AddableTypes
                .Select(type => AddOf(TreeAddIds.ForType(type), AddableTypes.LabelOf(type), group.Manager, type))
                .ToList();

        private static TreeAdd LayerAddOf(LayerGroup group, Type type, string prefix)
        {
            var entry = AddableTypes.ContentEntryOf(type);
            return AddOf(entry.Id, prefix + entry.Label, group.Manager, type);
        }

        private static TreeAdd AddOf(string id, string header, PrefabManager list, Type type) =>
            new(id, header, () => list.AddItem(type));
    }
}
