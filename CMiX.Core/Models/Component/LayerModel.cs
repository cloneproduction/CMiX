// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models.Component
{
    public class LayerModel : IPrefabModel//, IComponentModel
    {
        public LayerModel()
        {
            ID = Guid.NewGuid();
            Name = "Layer";

            Opacity = new SliderModel(1.0f);
            BlendMode = new BlendModeModel(BlendModeEnum.Normal);
            LayerScene = new LayerSceneModel();
            LayerMask = new LayerMaskModel();
            Visibility = new ToggleButtonModel();
            ModifierManager = new ModifierManagerModel();
        }

        public LayerModel(Guid id) : this()
        {
            ID = id;
        }


        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public SliderModel Opacity { get; set; }
        public BlendModeModel BlendMode { get; set; }
        public ToggleButtonModel Visibility { get; set; }
        public LayerSceneModel LayerScene { get; set; }
        public LayerMaskModel LayerMask { get; set; }
        public ModifierManagerModel ModifierManager { get; internal set; }
    }
}
