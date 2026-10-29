// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;

namespace CMiX.Core.Processors
{
    // Gives the lists that the render stage draws.
    // CompositionIds holds the visible compositions of the project, in the order of the list.
    // ProjectId is the ID of the project in the repository.
    // SelectedCompositionId is the ID of the selected composition, or an empty ID when none is selected.
    // GetLayerIds gives the visible layers of one composition, in the order of its layer list.
    // GetItemIds gives what one layer draws: its visible entities, then its cameras, then its lights.
    // GetTextureModifierIds gives the visible texture filters of a layer or a composition, in the order of its list.
    // GetVisibleIds gives the visible items of any list, in the order of the list.
    // GetTextureSourceIds gives the visible texture sources of one composition, in the order of the repository.
    // A list is built again only after the list or a visibility changed.
    public class RenderListProcessor
    {
        private readonly Project project;
        private readonly VisibleItemsProcessor items;
        private readonly ControlRepository repository;
        private readonly VisibilityProcessor visibility;
        private readonly ConditionalWeakTable<Layer, LayerItems> layerItems = new();

        public RenderListProcessor(Project project, VisibleItemsProcessor items, ControlRepository repository, VisibilityProcessor visibility)
        {
            this.project = project;
            this.items = items;
            this.repository = repository;
            this.visibility = visibility;
        }

        public IReadOnlyList<Guid> CompositionIds => items.GetVisibleIds(project.CompositionManager);

        public Guid ProjectId => project.ID;

        public Guid SelectedCompositionId => project.CompositionManager.SelectedItem?.ID ?? Guid.Empty;

        public IReadOnlyList<Guid> GetLayerIds(Composition composition) =>
            composition == null ? Array.Empty<Guid>() : items.GetVisibleIds(composition.LayerManager);

        public IReadOnlyList<Guid> GetItemIds(Layer layer)
        {
            if (layer == null)
                return Array.Empty<Guid>();

            return layerItems
                .GetValue(layer, _ => new LayerItems())
                .Get(items.GetVisibleIds(layer.ModelEntityManager), items.GetVisibleIds(layer.CameraManager), items.GetVisibleIds(layer.LightManager));
        }

        public IReadOnlyList<Guid> GetTextureModifierIds(ITextureModifiable owner) =>
            owner == null ? Array.Empty<Guid>() : items.GetVisibleIds(owner.TextureModifierManager);

        public IReadOnlyList<Guid> GetVisibleIds(PrefabManager manager) => items.GetVisibleIds(manager);

        public IReadOnlyList<Guid> GetTextureSourceIds(Guid compositionId) =>
            repository.Textures
                .OfType<IHasCompositionID>()
                .Where(source => source.CompositionID == compositionId && !visibility.IsHidden(((IControl)source).ID))
                .Select(source => ((IControl)source).ID)
                .ToArray();

        private sealed class LayerItems
        {
            private IReadOnlyList<Guid> entities;
            private IReadOnlyList<Guid> cameras;
            private IReadOnlyList<Guid> lights;
            private Guid[] ids = Array.Empty<Guid>();

            public IReadOnlyList<Guid> Get(IReadOnlyList<Guid> entities, IReadOnlyList<Guid> cameras, IReadOnlyList<Guid> lights)
            {
                if (!ReferenceEquals(this.entities, entities) || !ReferenceEquals(this.cameras, cameras) || !ReferenceEquals(this.lights, lights))
                {
                    ids = entities.Concat(cameras).Concat(lights).ToArray();
                    this.entities = entities;
                    this.cameras = cameras;
                    this.lights = lights;
                }

                return ids;
            }
        }
    }
}
