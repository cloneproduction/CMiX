// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public record BubbleNoiseModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel FilterManager { get; set; } = new();
        public Integer2Model Resolution { get; set; } = new(512, 512);
        public GenericValueModel<float> Speed { get; set; } = new(0.0f);
        public GenericValueModel<float> Frequency { get; set; } = new(3.5f);
        public GenericValueModel<float> Contrast { get; set; } = new(0.15f);
        public GenericValueModel<string> BackgroundColor { get; set; } = new("#FF000000");
        public GenericValueModel<string> BubbleColor { get; set; } = new("#FFFFFFFF");
    }
}
