// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Filters
{
    public record BlendModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsEnabled { get; set; } = new(false);
        public GenericValueModel<BlendModeEnum> BlendMode { get; set; } = new(BlendModeEnum.Normal);
        public GenericValueModel<float> Opacity { get; set; } = new(1.0f);
    }
}
