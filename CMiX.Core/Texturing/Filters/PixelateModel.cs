// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record PixelateModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> X { get; set; } = ModulatableValueModel<float>.Of("X", 0.5f);
        public ModulatableValueModel<float> Y { get; set; } = ModulatableValueModel<float>.Of("Y", 0.5f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
