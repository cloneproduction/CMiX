// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class ShiftRGB : ObservableObject, IPrefab, ITextureFilter
    {
        public ShiftRGB(PrefabService prefabService,
                        GenericValue<float> direction,
                        GenericValue<float> shift,
                        GenericValue<float> hue,
                        GenericValue<float> control)
        {
            PrefabService = prefabService;
            Direction = direction;
            Shift = shift;
            Hue = hue;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Direction { get; set; }
        public GenericValue<float> Shift { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new ShiftRGBModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Direction = (GenericValueModel<float>)Direction.ToModel(),
            Shift = (GenericValueModel<float>)Shift.ToModel(),
            Hue = (GenericValueModel<float>)Hue.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ShiftRGBModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Direction.FromModel(m.Direction);
            Shift.FromModel(m.Shift);
            Hue.FromModel(m.Hue);
            Control.FromModel(m.Control);
        }
    }
}
