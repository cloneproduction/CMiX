// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record TouchBlobModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2Model Resolution { get; set; } = new(1024, 1024);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Size { get; set; } = new(0.2f);
        public PrefabManagerModel TextureModifierManager { get; set; } = new();
        public GenericValueModel<string> Color { get; set; } = new("#FFFFFF");
        public GenericValueModel<string> Background { get; set; } = new("#000000");
    }
}
