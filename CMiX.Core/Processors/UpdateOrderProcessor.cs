// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Processors
{
    // Keeps the ids of the prefabs that must update, in the order of the update.
    // A control that is no prefab is not in the list. A hidden prefab is not in the list. OrderedIds is a new array after each change, so another thread can read it without a lock.
    public class UpdateOrderProcessor
    {
        // The first row updates first. The kinds in one row have no order between them.
        // The modulators update before the modifiers, so a modifier reads the value of the same frame.
        private static readonly Type[][] UpdateOrder =
        {
            new[] { typeof(IModulator) },
            new[] { typeof(IModifier) },
            new[] { typeof(ITextureSource) },
            new[] { typeof(Material) },
            new[] { typeof(Entity), typeof(TextEntity), typeof(LightEntity), typeof(Camera) },
            new[] { typeof(Layer) },
            new[] { typeof(Composition) },
            new[] { typeof(Project) },
        };

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

        // The last row is tested first, so a control that fits two rows gets the later one. A control of no known kind updates first.
        private static int Position(IControl control)
        {
            for (var row = UpdateOrder.Length - 1; row >= 0; row--)
                if (UpdateOrder[row].Any(type => type.IsInstanceOfType(control)))
                    return row;

            return -1;
        }

        private void Refresh() =>
            OrderedIds = repository.Controls
                .OfType<IPrefab>()
                .Where(prefab => !visibility.IsHidden(prefab.ID))
                .OrderBy(Position)
                .Select(prefab => prefab.ID)
                .ToArray();
    }
}
