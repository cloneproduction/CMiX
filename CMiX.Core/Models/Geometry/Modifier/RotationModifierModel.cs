// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class RotationModifierModel : IModel
    {
        public RotationModifierModel()
        {
            Rotation = new AnimParameterModel();
            RotationX = new AnimParameterModel();
            RotationY = new AnimParameterModel();
            RotationZ = new AnimParameterModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public AnimParameterModel Rotation { get; set; }
        public AnimParameterModel RotationX { get; set; }
        public AnimParameterModel RotationY { get; set; }
        public AnimParameterModel RotationZ { get; set; }
    }
}
