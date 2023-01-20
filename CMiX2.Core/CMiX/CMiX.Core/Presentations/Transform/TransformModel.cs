// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformModel : IPrefabModel
    {
        public TransformModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            TransformSRT = new TransformSRTModel();
            TransformModifier = new ModifierManagerModel();
            //Scale = new VectorXYZModel(nameof(Scale), 1.0f, 1.0f, 1.0f);
            //Rotate = new VectorXYZModel(nameof(Rotate), 0.0f, 0.0f, 0.0f);
            //Translate = new VectorXYZModel(nameof(Translate), 0.0f, 0.0f, 0.0f);
            //Uniform = new SliderModel(1.0f);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ModifierManagerModel TransformModifier { get; set; }
        //public VectorXYZModel Scale { get; set; }
        //public VectorXYZModel Rotate { get; set; }
        //public VectorXYZModel Translate { get; set; }
        //public SliderModel Uniform { get; internal set; }
        public TransformSRTModel TransformSRT { get; internal set; }
    }
}
