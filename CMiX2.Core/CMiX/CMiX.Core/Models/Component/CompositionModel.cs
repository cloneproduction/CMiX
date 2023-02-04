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

            MasterBeat = new MasterBeatModel();
            Visibility = new VisibilityModel();
            OutputSettings = new OutputSettingsModel();
            TextureModifierManager = new ModifierManagerModel();
            LayerManager = new PrefabManagerModel();
        }


        public VisibilityModel Visibility { get; set; }
        public MasterBeatModel MasterBeat { get; set; }


        public string Name { get; set; }
        public Guid ID { get; set; }


        public OutputSettingsModel OutputSettings { get; set; }
        public ModifierManagerModel TextureModifierManager { get; internal set; }
        public PrefabManagerModel LayerManager { get; set; }
    }
}
