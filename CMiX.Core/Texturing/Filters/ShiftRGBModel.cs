// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public record ShiftRGBModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Factor { get; set; } = new(1.0f);
        public GenericValueModel<float> Direction { get; set; } = new(0.25f);
        public GenericValueModel<float> Shift { get; set; } = new(0.2f);
        public GenericValueModel<float> Hue { get; set; } = new(0.0f);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
    }
}
