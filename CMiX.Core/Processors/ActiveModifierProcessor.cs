// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Processors
{
    // Gives the ids of the modifiers that apply to a target, in the order of its modifier list. A hidden modifier is not in the list.
    // A target is a control that has a ModifierManager. The list of a target is built again only after its modifier list or a visibility changed.
    // Call it from the thread that changes the controls.
    public class ActiveModifierProcessor
    {
        private readonly ControlRepository repository;
        private readonly VisibilityProcessor visibility;
        private readonly Dictionary<Guid, Target> targets = new();

        public ActiveModifierProcessor(ControlRepository repository, VisibilityProcessor visibility)
        {
            this.repository = repository;
            this.visibility = visibility;

            foreach (var control in repository.Controls)
                Add(control);

            repository.Controls.CollectionChanged += OnControlsChanged;
            visibility.Changed += MarkAllStale;
        }

        public IReadOnlyList<Guid> GetActiveIds(Guid targetId) =>
            targets.TryGetValue(targetId, out var target) ? target.GetIds(visibility) : Array.Empty<Guid>();

        private void OnControlsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var target in targets.Values)
                    target.Release();
                targets.Clear();

                foreach (var control in repository.Controls)
                    Add(control);
            }

            if (e.OldItems != null)
                foreach (IControl control in e.OldItems)
                    Remove(control);

            if (e.NewItems != null)
                foreach (IControl control in e.NewItems)
                    Add(control);
        }

        private void Add(IControl control)
        {
            if (control is not IModifiable { ModifierManager: not null } modifiable || targets.ContainsKey(control.ID))
                return;

            targets[control.ID] = new Target(modifiable);
        }

        private void Remove(IControl control)
        {
            if (targets.Remove(control.ID, out var target))
                target.Release();
        }

        private void MarkAllStale()
        {
            foreach (var target in targets.Values)
                target.MarkStale();
        }

        private sealed class Target
        {
            private readonly IModifiable control;
            private Guid[] ids;

            public Target(IModifiable control)
            {
                this.control = control;
                control.ModifierManager.ManagerData.Items.CollectionChanged += OnItemsChanged;
            }

            public void MarkStale() => ids = null;

            public void Release() => control.ModifierManager.ManagerData.Items.CollectionChanged -= OnItemsChanged;

            public IReadOnlyList<Guid> GetIds(VisibilityProcessor visibility) =>
                ids ??= control.ModifierManager.ManagerData.Items
                    .Where(item => !visibility.IsHidden(item.ID))
                    .Select(item => item.ID)
                    .ToArray();

            private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => MarkStale();
        }
    }
}
