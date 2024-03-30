// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class BubbleNoise : ObservableObject, ITextureSource, IPrefab
    {
        public BubbleNoise(PrefabService prefabService,
                           PrefabManager filterManager,
                           Integer2 resolution, 
                           GenericValue<float> speed, 
                           GenericValue<float> frequency, 
                           GenericValue<float> contrast, 
                           GenericValue<string> backgroundColor, 
                           GenericValue<string> bubbleColor)
        {
            ID = Guid.NewGuid();
            PrefabService = prefabService;
            Resolution = resolution;
            Speed = speed;
            Frequency = frequency;
            Contrast = contrast;
            BackgroundColor = backgroundColor;
            BubbleColor = bubbleColor;
            FilterManager = filterManager;
        }

        public Guid ID { get; set; }
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Speed { get; set; }
        public GenericValue<float> Frequency { get; set; }
        public GenericValue<float> Contrast { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<string> BubbleColor { get; set; }
        public PrefabManager FilterManager { get; set; }
    }
}
