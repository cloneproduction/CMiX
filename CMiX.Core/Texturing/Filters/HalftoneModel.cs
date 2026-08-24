// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Studio.Views.Texturing.Filter
{
    public record HalftoneModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public GenericValueModel<HalftoneMode> Mode { get; set; } = new(HalftoneMode.Pixels);
        public GenericValueModel<float> NumberOfTiles { get; set; } = new(48.0f);
        public GenericValueModel<float> DotSize { get; set; } = new(0.01f);
        public GenericValueModel<float> Softness { get; set; } = new(1.35f);
        public GenericValueModel<float> Brightness { get; set; } = new(1.0f);
        public BlendModel Blend { get; set; } = new();

    }
}
