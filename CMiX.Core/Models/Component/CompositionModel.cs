// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Models.Beat;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Models
{
    public class CompositionModel : IComponentModel, IPrefabModel
    {
        public CompositionModel()
        {
            ID = Guid.NewGuid();

            ComponentModels = new ObservableCollection<IComponentModel>();
            MasterBeatModel = new MasterBeatModel();
            CameraModel = new CameraModel();
            VisibilityModel = new VisibilityModel();
            CameraManagerModel = new PrefabManagerModel();
            OutputSettings = new OutputSettingsModel();
            TextureModifierManager = new ModifierManagerModel();
            MaterialManager = new PrefabManagerModel();

            LayerManager = new PrefabManagerModel();
        }


        public VisibilityModel VisibilityModel { get; set; }
        public MasterBeatModel MasterBeatModel { get; set; }
        public CameraModel CameraModel { get; set; }


        public bool Enabled { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }
        public bool IsVisible { get; set; }
        public string Address { get; set; }


        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
        public PrefabManagerModel CameraManagerModel { get; set; }
        public OutputSettingsModel OutputSettings { get; set; }
        public ModifierManagerModel TextureModifierManager { get; internal set; }
        public PrefabManagerModel MaterialManager { get; internal set; }
        public PrefabManagerModel LayerManager { get; set; }
    }
}
