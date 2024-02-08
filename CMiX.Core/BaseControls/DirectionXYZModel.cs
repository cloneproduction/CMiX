// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public class DirectionXYZModel : IControlModel
    {
        public DirectionXYZModel()
        {
            ID = Guid.NewGuid();
            DirectionX = new GenericValueModel<bool>(true);
            DirectionY = new GenericValueModel<bool>(false);
            DirectionZ = new GenericValueModel<bool>(false);
        }
        public Guid ID { get; set; }
        public GenericValueModel<bool> DirectionX { get; set; }
        public GenericValueModel<bool> DirectionY { get; set; }
        public GenericValueModel<bool> DirectionZ { get; set; }
    }
}
