// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public record DirectionXYModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> DirectionX { get; set; } = new(true);
        public GenericValueModel<bool> DirectionY { get; set; } = new(false);
    }
}
