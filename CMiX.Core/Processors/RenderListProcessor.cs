// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using CMiX.Core.Compositing;

namespace CMiX.Core.Processors
{
    // Gives the lists that the render stage draws.
    // CompositionIds holds the visible compositions of the project, in the order of the list.
    // ProjectId is the ID of the project in the repository.
    // SelectedCompositionId is the ID of the selected composition, or an empty ID when none is selected.
    // GetLayerIds gives the visible layers of one composition, in the order of its layer list.
    // GetItemIds gives what one layer draws: its visible entities, then its cameras, then its lights.
    // A list is built again only after the list or a visibility changed.
    public class RenderListProcessor
    {
        private readonly Project project;
        private readonly VisibleItemsProcessor items;
        private readonly ConditionalWeakTable<Layer, LayerItems> layerItems = new();

        public RenderListProcessor(Project project, VisibleItemsProcessor items)
        {
            this.project = project;
            this.items = items;
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
