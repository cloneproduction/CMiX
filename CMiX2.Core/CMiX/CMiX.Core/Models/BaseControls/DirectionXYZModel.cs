// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class DirectionXYZModel : IModel
    {
        public DirectionXYZModel()
        {
            this.ID = Guid.NewGuid();
            DirectionX = true;
            DirectionY = false;
            DirectionZ = false;
        }
        public Guid ID { get; set; }
        public bool DirectionX { get; set; }
        public bool DirectionY { get; set; }
        public bool DirectionZ { get; set; }
    }
}
