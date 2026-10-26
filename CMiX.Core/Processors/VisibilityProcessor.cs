// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CMiX.Core.Components;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Processors
{
    // Keeps the ids of the visible controls. It reads the repository and does not change it.
    // VisibleIds is a new array after each change, so another thread can read it without a lock.
    public class VisibilityProcessor
    {
        private readonly ControlRepository repository;
        private readonly Dictionary<Guid, VisibilityComponent> components = new();

        public VisibilityProcessor(ControlRepository repository)
        {
            this.repository = repository;

            foreach (var prefab in repository.Controls.OfType<IPrefab>())
                Add(prefab);
            Refresh();

            repository.Controls.CollectionChanged += OnControlsChanged;
        }

        public IReadOnlyList<Guid> VisibleIds { get; private set; } = Array.Empty<Guid>();

        public bool IsVisible(Guid controlId) =>
            components.TryGetValue(controlId, out var component) && component.IsVisible;

        private void OnControlsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var component in components.Values)
                    component.Visible.PropertyChanged -= OnVisibleChanged;
                components.Clear();

                foreach (var prefab in repository.Controls.OfType<IPrefab>())
                    Add(prefab);
            }

            if (e.OldItems != null)
                foreach (var prefab in e.OldItems.OfType<IPrefab>())
                    Remove(prefab);

            if (e.NewItems != null)
                foreach (var prefab in e.NewItems.OfType<IPrefab>())
                    Add(prefab);

            Refresh();
        }

        private void Add(IPrefab prefab)
        {
            var visible = prefab.PrefabService?.Visibility;
            if (visible == null || components.ContainsKey(prefab.ID))
                return;

            components[prefab.ID] = new VisibilityComponent(prefab.ID, visible);
            visible.PropertyChanged += OnVisibleChanged;
        }

        private void Remove(IPrefab prefab)
        {
            if (!components.Remove(prefab.ID, out var component))
                return;

            component.Visible.PropertyChanged -= OnVisibleChanged;
        }

        private void OnVisibleChanged(object sender, PropertyChangedEventArgs e) => Refresh();

        private void Refresh() =>
            VisibleIds = components.Values.Where(component => component.IsVisible).Select(component => component.ControlId).ToArray();
    }
}
