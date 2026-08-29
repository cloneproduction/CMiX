// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;

namespace CMiX.Core.Modulation
{
    public record CameraRandomModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public List<ModulatableModel> Channels { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public GenericValueModel<bool> PingPong { get; set; } = new(false);
        public GenericValueModel<CameraAxis> Axis { get; set; } = new();
    }
}
