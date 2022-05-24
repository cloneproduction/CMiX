// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Models.Component;

namespace CMiX.Core.Models
{
    public class SceneModel : IComponentModel
    {
        public SceneModel()
        {
            TransformModel = new TransformModel();
            TextureModel = new TextureModel();
            GeometryModel = new GeometryModel();
            MaskModel = new LayerMaskModel();
            BeatModifierModel = new BeatModifierModel();
            BackgroundColorSelectorModel = new ColorSelectorModel();
            CameraModel = new CameraModel();

            ComponentModels = new ObservableCollection<IComponentModel>();
        }

        public SceneModel(Guid id) : this()
        {
            ID = id;
        }

        public bool Enabled { get; set; }
        public bool IsVisible { get; set; }

        public ColorSelectorModel BackgroundColorSelectorModel { get; set; }
        public VisibilityModel VisibilityModel { get; set; }
        public TransformModel TransformModel { get; set; }
        public LayerMaskModel MaskModel { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public TextureModel TextureModel { get; set; }
        public GeometryModel GeometryModel { get; set; }
        public CameraModel CameraModel { get; set; }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
    }
}
