// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class GeometryModel : IModel
    {
        public GeometryModel()
        {
            this.ID = Guid.NewGuid();
            GeometryFXModel = new GeometryFXModel();
            TransformModel = new TransformModel();
            ModifierManagerModel = new ModifierManagerModel();
            AssetPathSelectorModel = new GeometrySelectorModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TransformModel TransformModel { get; set; }
        public ModifierManagerModel ModifierManagerModel { get; set; }
        public GeometryFXModel GeometryFXModel { get; set; }
        public GeometrySelectorModel AssetPathSelectorModel { get; set; }
    }
}
