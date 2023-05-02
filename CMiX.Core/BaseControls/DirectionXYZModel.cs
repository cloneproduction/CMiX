// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public class DirectionXYZModel : IModel
    {
        public DirectionXYZModel()
        {
            ID = Guid.NewGuid();
            DirectionX = new BooleanValueModel(true);
            DirectionY = new BooleanValueModel(false);
            DirectionZ = new BooleanValueModel(false);
        }
        public Guid ID { get; set; }
        public BooleanValueModel DirectionX { get; set; }
        public BooleanValueModel DirectionY { get; set; }
        public BooleanValueModel DirectionZ { get; set; }
    }
}
