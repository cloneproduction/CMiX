// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;

namespace CMiX.Core.Processors
{
    // Keeps the ids of the controls that must update, in the order of the update: the highest render priority first.
    // A hidden control is not in the list. OrderedIds is a new array after each change, so another thread can read it without a lock.
    public class UpdateOrderProcessor
    {
        private readonly ControlRepository repository;
        private readonly VisibilityProcessor visibility;

        public UpdateOrderProcessor(ControlRepository repository, VisibilityProcessor visibility)
        {
            this.repository = repository;
            this.visibility = visibility;

            repository.Controls.CollectionChanged += (_, _) => Refresh();
            visibility.Changed += Refresh;
            Refresh();
        }

        public IReadOnlyList<Guid> OrderedIds { get; private set; } = Array.Empty<Guid>();

        private void Refresh() =>
            OrderedIds = repository.Controls
                .Where(control => !visibility.IsHidden(control.ID))
                .OrderByDescending(ControlRepository.GetRenderPriority)
                .Select(control => control.ID)
                .ToArray();
    }
}
