// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public record LayerModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public LayerSettingsModel LayerSettings { get; init; } = new();
        public GenericValueModel<bool> Invert { get; init; } = new(false);
        public GenericValueModel<bool> IsMask { get; init; } = new(false);
        public GenericValueModel<MaskMode> MaskMode { get; init; } = new(Texturing.MaskMode.AllBelow);
        public PrefabManagerModel TextureModifierManager { get; init; } = new();
        public PrefabManagerModel ModifierManager { get; init; } = new();
        public AmbientOcclusionModel AmbientOcclusion { get; init; } = new();
        public LocalReflectionModel LocalReflection { get; init; } = new();
        public GenericValueModel<MaskChannel> MaskChannel { get; init; } = new(Texturing.MaskChannel.Alpha);
        public PrefabManagerModel ModelEntityManager { get; init; } = new();
        public PrefabManagerModel CameraManager { get; init; } = new();
        public PrefabManagerModel LightManager { get; init; } = new();
    }
}
