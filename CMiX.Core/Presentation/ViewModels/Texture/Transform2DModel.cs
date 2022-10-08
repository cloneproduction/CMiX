// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Transform2DModel : IModel
    {
        public Transform2DModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            Translate = new VectorXYModel(0.0f, 0.0f);
            Scale = new VectorXYModel(1.0f, 1.0f);
            Rotate = new SliderModel(0.0f);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public VectorXYModel Translate { get; set; }
        public VectorXYModel Scale { get; set; }
        public SliderModel Rotate { get; set; }
    }
}
