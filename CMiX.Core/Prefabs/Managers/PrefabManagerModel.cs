// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs.Managers
{
    public record PrefabManagerModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public ManagerDataModel ManagerData { get; init; } = new();

        // True when the whole list is hidden. The items keep their own visibility. A file without it opens with the list visible.
        public GenericValueModel<bool> Hidden { get; init; } = new(false);

        // An empty list that no manager owns. Its three ids come from the owner, so every ToModel gives the same model.
        public static PrefabManagerModel Placeholder(Guid owner, string name) => new()
        {
            ID = DerivedId.Of(owner, name),
            ManagerData = new() { ID = DerivedId.Of(owner, name + "." + nameof(ManagerData)) },
            Hidden = new(false) { ID = DerivedId.Of(owner, name + "." + nameof(Hidden)) }
        };
    }
}
