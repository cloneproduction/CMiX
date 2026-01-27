// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public record DirectionXYZModel : IControlModel
    {
        public DirectionXYZModel()
        {
            DirectionX = new(true);
            DirectionY = new(false);
            DirectionZ = new(false);
        }

        public DirectionXYZModel(bool x, bool y, bool z)
        {
            DirectionX = new(x);
            DirectionY = new(y);
            DirectionZ = new(z);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> DirectionX { get; set; }
        public GenericValueModel<bool> DirectionY { get; set; }
        public GenericValueModel<bool> DirectionZ { get; set; }
    }
}
