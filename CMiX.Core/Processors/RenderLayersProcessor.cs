// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Runtime.CompilerServices;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Processors
{
    // Keeps the ids of the layers to render: the visible layers of the composition that is selected in the project, in the order of its layer list.
    // The list is empty when no composition is selected or when the selected composition is hidden.
    // Ids is a new array after each change, so another thread can read it without a lock.
    // GetItemIds gives what one layer draws: its visible entities, then its cameras, then its lights.
    public class RenderLayersProcessor
    {
        private readonly ManagerData compositions;
        private readonly VisibilityProcessor visibility;
        private readonly VisibleItemsProcessor items;
        private readonly ConditionalWeakTable<Layer, LayerItems> layerItems = new();
        private Composition selected;
        private ManagerData layers;

        public RenderLayersProcessor(Project project, VisibilityProcessor visibility, VisibleItemsProcessor items)
        {
            this.visibility = visibility;
            this.items = items;

            compositions = project.CompositionManager.ManagerData;
            compositions.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ManagerData.SelectedItem))
                    Select();
            };
            visibility.Changed += Refresh;

            Select();
        }

        public IReadOnlyList<Guid> Ids { get; private set; } = Array.Empty<Guid>();

        public IReadOnlyList<Guid> GetItemIds(Layer layer)
        {
            if (layer == null)
                return Array.Empty<Guid>();

            return layerItems
                .GetValue(layer, _ => new LayerItems())
                .Get(items.GetVisibleIds(layer.ModelEntityManager), items.GetVisibleIds(layer.CameraManager), items.GetVisibleIds(layer.LightManager));
        }

        private void Select()
        {
            if (layers != null)
                layers.Items.CollectionChanged -= OnLayersChanged;

            selected = compositions.SelectedItem as Composition;
            layers = selected?.LayerManager.ManagerData;

            if (layers != null)
                layers.Items.CollectionChanged += OnLayersChanged;

            Refresh();
        }

        private void OnLayersChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => Refresh();

        private void Refresh() =>
            Ids = selected == null || layers == null || visibility.IsHidden(selected.ID)
                ? Array.Empty<Guid>()
                : layers.Items
                    .Where(layer => !visibility.IsHidden(layer.ID))
                    .Select(layer => layer.ID)
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
