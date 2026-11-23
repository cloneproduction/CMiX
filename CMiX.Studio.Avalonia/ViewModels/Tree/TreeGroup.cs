// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // What the inspector shows for a group row. Each kind is its own type, so a data template picks its panel.
    public abstract class TreeGroup
    {
        protected TreeGroup(IControl owner, PrefabManager manager)
        {
            Owner = owner;
            Manager = manager;
        }

        public IControl Owner { get; }
        public PrefabManager Manager { get; }

        public Type OwnerType => Owner.GetType();

        public abstract string Header { get; }

        public abstract string IconKey { get; }

        public abstract TreeCategory Category { get; }

        public abstract IReadOnlyList<Type> AddableTypes { get; }

        public static IReadOnlyList<TreeGroup> GroupsOf(IControl? owner)
        {
            var groups = new List<TreeGroup>();
            if (owner is Project project)
                groups.Add(new CompositionsGroup(project, project.CompositionManager));

            if (owner is Composition composition)
                groups.Add(new LayersGroup(composition, composition.LayerManager));

            if (owner is Layer entityLayer)
                groups.Add(new EntitiesGroup(entityLayer, entityLayer.ModelEntityManager));

            if (owner is ITexturable textured && textured.Texture?.DiffuseTexture?.TextureManager is { } textures)
                groups.Add(new TexturesGroup(owner, textures));

            if (owner is ITextureModifiable filtered && filtered.TextureModifierManager is { } filters)
                groups.Add(new FiltersGroup(owner, filters));

            if (owner is IModifiable modified && modified.ModifierManager is { } modifiers)
                groups.Add(new ModifiersGroup(owner, modifiers));

            if (owner is Layer layer)
            {
                groups.Add(new CamerasGroup(layer, layer.CameraManager));
                groups.Add(new LightsGroup(layer, layer.LightManager));
            }

            return groups;
        }

        public static TreeGroup? For(IControl? owner, PrefabManager? list) =>
            list == null ? null : GroupsOf(owner).FirstOrDefault(group => ReferenceEquals(group.Manager, list));
    }

    public abstract class StructureGroup : TreeGroup
    {
        protected StructureGroup(IControl owner, PrefabManager manager) : base(owner, manager) { }
    }

    public sealed class CompositionsGroup : StructureGroup
    {
        public const string Title = "Compositions";
        private static readonly Type[] addable = { typeof(Composition) };

        public CompositionsGroup(Project project, PrefabManager manager) : base(project, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.CompositionKey;
        public override TreeCategory Category => TreeCategory.Composable;
        public override IReadOnlyList<Type> AddableTypes => addable;
    }

    public sealed class LayersGroup : StructureGroup
    {
        public const string Title = "Layers";
        private static readonly Type[] addable = { typeof(Layer) };

        public LayersGroup(Composition composition, PrefabManager manager) : base(composition, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.LayerKey;
        public override TreeCategory Category => TreeCategory.Composable;
        public override IReadOnlyList<Type> AddableTypes => addable;
    }

    // The panels of a layer take the layer as their DataContext.
    public abstract class LayerGroup : TreeGroup
    {
        protected LayerGroup(Layer layer, PrefabManager manager) : base(layer, manager)
        {
            Layer = layer;
        }

        public Layer Layer { get; }
    }

    public sealed class EntitiesGroup : LayerGroup
    {
        public const string Title = "Entities";
        private static readonly Type[] addable = { typeof(Entity), typeof(FullscreenQuad), typeof(TextEntity) };

        public EntitiesGroup(Layer layer, PrefabManager manager) : base(layer, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.EntityKey;
        public override TreeCategory Category => TreeCategory.Entity;
        public override IReadOnlyList<Type> AddableTypes => addable;
    }

    public sealed class CamerasGroup : LayerGroup
    {
        public const string Title = "Cameras";
        private static readonly Type[] addable = { typeof(Camera) };

        public CamerasGroup(Layer layer, PrefabManager manager) : base(layer, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.CameraKey;
        public override TreeCategory Category => TreeCategory.Camera;
        public override IReadOnlyList<Type> AddableTypes => addable;
    }

    public sealed class LightsGroup : LayerGroup
    {
        public const string Title = "Lights";
        private static readonly Type[] addable = { typeof(LightEntity) };

        public LightsGroup(Layer layer, PrefabManager manager) : base(layer, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.LightKey;
        public override TreeCategory Category => TreeCategory.Light;
        public override IReadOnlyList<Type> AddableTypes => addable;
    }

    public sealed class TexturesGroup : TreeGroup
    {
        public const string Title = "Textures";

        public TexturesGroup(IControl owner, PrefabManager manager) : base(owner, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.TextureKey;
        public override TreeCategory Category => TreeCategory.Texture;
        public override IReadOnlyList<Type> AddableTypes => Tree.AddableTypes.ForInterface(typeof(ITextureSource));
    }

    public sealed class FiltersGroup : TreeGroup
    {
        public const string Title = "Filters";

        public FiltersGroup(IControl owner, PrefabManager manager) : base(owner, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.FilterKey;
        public override TreeCategory Category => TreeCategory.Filter;
        public override IReadOnlyList<Type> AddableTypes => Tree.AddableTypes.ForInterface(typeof(ITextureFilter));
    }

    // ListRules.CanHold picks the modifiers that the owner can take.
    public sealed class ModifiersGroup : TreeGroup
    {
        public const string Title = "Modifiers";

        public ModifiersGroup(IControl owner, PrefabManager manager) : base(owner, manager) { }

        public override string Header => Title;
        public override string IconKey => TreeIcons.ModifierKey;
        public override TreeCategory Category => TreeCategory.Modifier;
        public override IReadOnlyList<Type> AddableTypes => Tree.AddableTypes.ForOwner(OwnerType);
    }
}
