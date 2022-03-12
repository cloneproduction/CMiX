// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using System;
using System.Collections.ObjectModel;

namespace CMiX.Core.Models
{
    public class EntityModel : IComponentModel
    {
        public EntityModel()
        {
            BeatModifierModel = new BeatModifierModel();
            GeometryModel = new GeometryModel();
            TextureModel = new TextureModel();
            ColorationModel = new MaterialModel();

            VisibilityModel = new VisibilityModel();
        }

        public EntityModel(Guid id) : this ()
        {
            ID = id;
        }

        public bool Enabled { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }

        public BeatModifierModel BeatModifierModel { get; set; }
        public GeometryModel GeometryModel { get; set; }
        public TextureModel TextureModel { get; set; }
        public MaterialModel ColorationModel { get; set; }
        public VisibilityModel VisibilityModel { get; set; }

        public string Address { get; set; }
        public bool IsVisible { get; set; }

        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
    }
}
