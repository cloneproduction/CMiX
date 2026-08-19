// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public record LayerMaskSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsMask { get; init; } = new();
        public GenericValueModel<MaskChannel> MaskChannel { get; init; } = new();
        public GenericValueModel<MaskMode> MaskMode { get; init; } = new();
        public GenericValueModel<bool> Invert { get; init; } = new();
    }
}
