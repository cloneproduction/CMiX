// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public record TriColorModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public GenericValueModel<string> ColorA { get; set; } = new("#FFFF00FF");
        public GenericValueModel<string> ColorB { get; set; } = new("#FFFF00FF");
        public GenericValueModel<string> ColorC { get; set; } = new("#FFFF00FF");
        public GenericValueModel<float> Smooth { get; set; } = new (0.5f);
        public GenericValueModel<float> Center { get; set; } = new(0.5f);
        public GenericValueModel<bool> SingleChannel { get; set; } = new(true);
        public GenericValueModel<bool> ClampColor { get; set; } = new(true);
        public BlendModel Blend { get; set; } = new();
    }
}
