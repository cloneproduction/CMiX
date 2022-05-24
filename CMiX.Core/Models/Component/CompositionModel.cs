// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Models.Beat;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Services;

namespace CMiX.Core.Models
{
    public class CompositionModel : IComponentModel
    {
        public CompositionModel()
        {

        }

        public CompositionModel(Guid id)
        {
            ID = id;
            ComponentModels = new ObservableCollection<IComponentModel>();
            MasterBeatModel = new MasterBeatModel();
            CameraModel = new CameraModel();
            TransitionModel = new SliderModel();
            VisibilityModel = new VisibilityModel();
            CameraManagerModel = new PrefabManagerModel();
            OutputProperties = new OutputPropertiesModel();
            ModifierManager = new ModifierManagerModel();
            MaterialManager = new PrefabManagerModel();

            CompositionService = new CompositionServiceModel();
        }

        public VisibilityModel VisibilityModel { get; set; }
        public MasterBeatModel MasterBeatModel { get; set; }
        public CameraModel CameraModel { get; set; }
        public SliderModel TransitionModel { get; set; }


        public bool Enabled { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }
        public bool IsVisible { get; set; }
        public string Address { get; set; }
        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
        public PrefabManagerModel CameraManagerModel { get; set; }
        public OutputPropertiesModel OutputProperties { get; set; }
        public ModifierManagerModel ModifierManager { get; internal set; }
        public PrefabManagerModel MaterialManager { get; internal set; }
        public CompositionServiceModel CompositionService { get; internal set; }
    }
}
