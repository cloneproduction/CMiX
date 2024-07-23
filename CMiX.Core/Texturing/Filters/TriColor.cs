// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TriColor : ObservableObject, IPrefab
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
            isExpanded = true;

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
        private bool isExpanded;
    }
}
