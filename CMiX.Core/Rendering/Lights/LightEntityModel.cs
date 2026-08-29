// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Rendering.Lights
{
    public record LightEntityModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public TransformSRTModifierModel TransformSRT { get; set; } = new();
        public LightSettingsModel Settings { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
        public PrefabManagerModel ColorPaletteManager { get; set; } = new();
    }
}
