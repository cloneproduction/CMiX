// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise(PrefabService prefabService,
                           PrefabManager textureModifierManager,
                           Integer2 resolution, 
                           GenericValue<float> speed, 
                           GenericValue<float> frequency, 
                           GenericValue<float> contrast, 
                           GenericValue<string> backgroundColor, 
                           GenericValue<string> bubbleColor)
        {
            PrefabService = prefabService;
            Resolution = resolution;
            Speed = speed;
            Frequency = frequency;
            Contrast = contrast;
            BackgroundColor = backgroundColor;
            BubbleColor = bubbleColor;
            TextureModifierManager = textureModifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Speed { get; set; }
        public GenericValue<float> Frequency { get; set; }
        public GenericValue<float> Contrast { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<string> BubbleColor { get; set; }
        public PrefabManager TextureModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new BubbleNoiseModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            Speed = (GenericValueModel<float>)Speed.ToModel(),
            Frequency = (GenericValueModel<float>)Frequency.ToModel(),
            Contrast = (GenericValueModel<float>)Contrast.ToModel(),
            BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel(),
            BubbleColor = (GenericValueModel<string>)BubbleColor.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BubbleNoiseModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Resolution.FromModel(m.Resolution);
            Speed.FromModel(m.Speed);
            Frequency.FromModel(m.Frequency);
            Contrast.FromModel(m.Contrast);
            BackgroundColor.FromModel(m.BackgroundColor);
            BubbleColor.FromModel(m.BubbleColor);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
        }
    }
}
