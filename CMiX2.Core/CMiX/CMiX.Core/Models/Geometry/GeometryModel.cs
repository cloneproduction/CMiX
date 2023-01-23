// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class GeometryModel : IModel
    {
        public GeometryModel()
        {
            this.ID = Guid.NewGuid();


            ModifierManagerModel = new ModifierManagerModel();
            AssetPathSelectorModel = new GeometrySelectorModel();
            VisibilityModel = new VisibilityModel();
        }

        public Guid ID { get; set; }

        public ModifierManagerModel ModifierManagerModel { get; set; }

        public GeometrySelectorModel AssetPathSelectorModel { get; set; }
        public VisibilityModel VisibilityModel { get; internal set; }
    }
}
