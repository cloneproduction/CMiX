// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TriColor : ObservableObject, IPrefab, ITextureFilter
    {
        public TriColor(PrefabService prefabService,
                        GenericValue<float> control, 
                        GenericValue<string> colorA, 
                        GenericValue<string> colorB, 
                        GenericValue<string> colorC, 
                        GenericValue<float> smooth, 
                        GenericValue<float> center, 
                        GenericValue<bool> singleChannel, 
                        GenericValue<bool> clampColor)
        {
            PrefabService = prefabService;
            Control = control;
            ColorA = colorA;
            ColorB = colorB;
            ColorC = colorC;
            Smooth = smooth;
            Center = center;
            SingleChannel = singleChannel;
            ClampColor = clampColor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<float> Smooth { get; set; }
        public GenericValue<float> Center { get; set; }
        public GenericValue<string> ColorA { get; set; }
        public GenericValue<string> ColorB { get; set; }
        public GenericValue<string> ColorC { get; set; }
        public GenericValue<bool> SingleChannel { get; set; }
        public GenericValue<bool> ClampColor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new TriColorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            ColorA = (GenericValueModel<string>)ColorA.ToModel(),
            ColorB = (GenericValueModel<string>)ColorB.ToModel(),
            ColorC = (GenericValueModel<string>)ColorC.ToModel(),
            Smooth = (GenericValueModel<float>)Smooth.ToModel(),
            Center = (GenericValueModel<float>)Center.ToModel(),
            SingleChannel = (GenericValueModel<bool>)SingleChannel.ToModel(),
            ClampColor = (GenericValueModel<bool>)ClampColor.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TriColorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Control.FromModel(m.Control);
            ColorA.FromModel(m.ColorA);
            ColorB.FromModel(m.ColorB);
            ColorC.FromModel(m.ColorC);
            Smooth.FromModel(m.Smooth);
            Center.FromModel(m.Center);
            SingleChannel.FromModel(m.SingleChannel);
            ClampColor.FromModel(m.ClampColor);
        }
    }
}
