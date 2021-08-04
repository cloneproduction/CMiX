// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class RotationModel : IModel
    {
        public RotationModel()
        {
            this.ID = Guid.NewGuid();
            X = new SliderModel();
            Y = new SliderModel();
            Z = new SliderModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel X { get; set; }
        public SliderModel Y { get; set; }
        public SliderModel Z { get; set; }
    }
}
