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

            SizeX = new CounterModel(1920);
            SizeY = new CounterModel(1080);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public CounterModel SizeX { get; set; }
        public CounterModel SizeY { get; set; }
    }
}
