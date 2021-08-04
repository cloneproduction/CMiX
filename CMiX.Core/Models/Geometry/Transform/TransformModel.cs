// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class TransformModel : IModel
    {
        public TransformModel()
        {
            this.ID = Guid.NewGuid();
            GeometryTranslateModel = new GeometryTranslateModel();
            GeometryScaleModel = new GeometryScaleModel();
            GeometryRotationModel = new GeometryRotationModel();

            TranslateModel = new TranslateModel();
            ScaleModel = new ScaleModel();
            RotationModel = new RotationModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TranslateModel TranslateModel { get; set; }
        public ScaleModel ScaleModel { get; set; }
        public RotationModel RotationModel { get; set; }

        public GeometryTranslateModel GeometryTranslateModel { get; set; }
        public GeometryScaleModel GeometryScaleModel { get; set; }
        public GeometryRotationModel GeometryRotationModel { get; set; }

        public bool Is3D { get; set; }
    }
}
