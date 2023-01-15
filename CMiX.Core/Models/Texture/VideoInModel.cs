// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Models
{
    public class VideoInModel : IModel
    {
        public VideoInModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            SizeX = new IntegerValueModel(1920);
            SizeY = new IntegerValueModel(1080);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public IntegerValueModel SizeX { get; set; }
        public IntegerValueModel SizeY { get; set; }
    }
}
