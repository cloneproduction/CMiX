// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering
{
    public record OutputSettingsModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public Integer2Model Resolution { get; init; } = new (1080, 1920);
        public GenericValueModel<string> BackgroundColor { get; init; } = new ("#FFFFFFFF");
    }
}
