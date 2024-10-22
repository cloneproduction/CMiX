// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors
{
    public class ColorationModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public PrefabManagerModel ColorPaletteManager { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
    }
}
