// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blur : ObservableObject, IPrefab, ITextureFilter
    {
        public Blur(PrefabService prefabService, 
                    GenericValue<bool> visible, 
                    GenericValue<float> strength,
                    GenericValue<float> control)
        {
            PrefabService = prefabService;
            Strength = strength;
            Visible = visible;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Strength { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new BlurModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Strength = (GenericValueModel<float>)Strength.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BlurModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Strength.FromModel(m.Strength);
            Control.FromModel(m.Control);
        }
    }
}
