// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Models
{
    public class BoxModel : IEntityModel
    {
        public BoxModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            SizeX = new SliderModel();
            SizeY = new SliderModel();
            SizeZ = new SliderModel();

        }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public SliderModel SizeX { get; set; }
        public SliderModel SizeY { get; set; }
        public SliderModel SizeZ { get; set; }
    }
}
