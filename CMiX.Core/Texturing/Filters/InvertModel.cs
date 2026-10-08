// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record InvertModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<InvertChannel> InvertChannelSelector { get; set; } = new(InvertChannel.Value);
        public GenericValueModel<bool> InvertAlpha { get; set; } = new(false);
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Factor { get; set; } = ModulatableValueModel<float>.Of("Factor", 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
