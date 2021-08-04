// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class XYZModifierModel : IModel
    {
        public XYZModifierModel()
        {
            X = new AnimParameterModel();
            Y = new AnimParameterModel();
            Z = new AnimParameterModel();
        }

        public string Name { get; set; }
        public AnimParameterModel X { get; set; }
        public AnimParameterModel Y { get; set; }
        public AnimParameterModel Z { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
    }
}
