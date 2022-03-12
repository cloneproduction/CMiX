// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Texture : ObservableObject, IControl
    {
        public Texture(TextureModel textureModel)
        {
            this.ID = textureModel.ID;

            ModifierManager = new ModifierManager(textureModel.ModifierManagerModel, new TextureFilterFactory());

            Inverter = new Inverter(nameof(Inverter), textureModel.InverterModel);

            Brightness = new Slider(nameof(Brightness), textureModel.Brightness) { Minimum = -1.0f, Maximum = 1.0f };
            Contrast = new Slider(nameof(Contrast), textureModel.Contrast) { Minimum = -1.0f, Maximum = 1.0f };
            Hue = new Slider(nameof(Hue), textureModel.Hue) { Minimum = -1.0f, Maximum = 1.0f };
            Saturation = new Slider(nameof(Saturation), textureModel.Saturation) { Minimum = -1.0f, Maximum = 1.0f };
            Luminosity = new Slider(nameof(Luminosity), textureModel.Luminosity) { Minimum = -1.0f, Maximum = 1.0f };
            Keying = new Slider(nameof(Keying), textureModel.Keying);

            TranslateU = new Slider(nameof(TranslateU), textureModel.TranslateU) { Minimum = -1.0f, Maximum = 1.0f };
            TranslateV = new Slider(nameof(TranslateV), textureModel.TranslateV) { Minimum = -1.0f, Maximum = 1.0f };

            ScaleU = new Slider(nameof(Scale), textureModel.ScaleU) { Minimum = -1.0f, Maximum = 1.0f };
            ScaleV = new Slider(nameof(Scale), textureModel.ScaleV) { Minimum = -1.0f, Maximum = 1.0f };

            Rotate = new Slider(nameof(Rotate), textureModel.Rotate) { Minimum = -1.0f, Maximum = 1.0f };

            ImageSelector = new ImageSelector(new AssetTexture(), textureModel.TextureSelectorModel);

            VideoPlayer = new VideoPlayer(textureModel.VideoPlayerModel);
        }


        public Guid ID { get; set; }
        public ModifierManager ModifierManager { get; set; }


        public VideoPlayer VideoPlayer { get; set; }
        public ImageSelector ImageSelector { get; set; }
        public Slider Brightness { get; set; }
        public Slider Contrast { get; set; }
        public Slider Hue { get; set; }
        public Slider Saturation { get; set; }
        public Slider Luminosity { get; set; }
        public Slider Keying { get; set; }

        public Slider TranslateU { get; set; }
        public Slider TranslateV { get; set; }

        public Slider ScaleU { get; set; }
        public Slider ScaleV { get; set; }

        public Slider Rotate { get; set; }
        public Inverter Inverter { get; set; }


        public IModel GetModel()
        {
            TextureModel model = new TextureModel();

            model.ID = this.ID;

            model.ModifierManagerModel = (ModifierManagerModel)this.ModifierManager.GetModel();

            model.TextureSelectorModel = (ImageSelectorModel)this.ImageSelector.GetModel();
            model.InverterModel = (InvertModel)this.Inverter.GetModel();
            model.Brightness = (SliderModel)this.Brightness.GetModel();
            model.Contrast = (SliderModel)this.Contrast.GetModel();
            model.Saturation = (SliderModel)this.Saturation.GetModel();
            model.Luminosity = (SliderModel)this.Luminosity.GetModel();
            model.Hue = (SliderModel)this.Hue.GetModel();

            model.TranslateU = (SliderModel)this.TranslateU.GetModel();
            model.TranslateV = (SliderModel)this.TranslateV.GetModel();

            model.ScaleU = (SliderModel)this.ScaleU.GetModel();
            model.ScaleV = (SliderModel)this.ScaleV.GetModel();

            model.Rotate = (SliderModel)this.Rotate.GetModel();
            model.Keying = (SliderModel)this.Keying.GetModel();
            model.VideoPlayerModel = (VideoPlayerModel)this.VideoPlayer.GetModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            TextureModel textureModel = model as TextureModel;

            this.ID = textureModel.ID;

            this.ModifierManager.SetViewModel(textureModel.ModifierManagerModel);

            this.ImageSelector.SetViewModel(textureModel.TextureSelectorModel);

            this.Inverter.SetViewModel(textureModel.InverterModel);
            this.Brightness.SetViewModel(textureModel.Brightness);
            this.Contrast.SetViewModel(textureModel.Contrast);
            this.Saturation.SetViewModel(textureModel.Saturation);
            this.Luminosity.SetViewModel(textureModel.Luminosity);
            this.Hue.SetViewModel(textureModel.Hue);

            this.TranslateU.SetViewModel(textureModel.TranslateU);
            this.TranslateV.SetViewModel(textureModel.TranslateV);

            this.ScaleU.SetViewModel(textureModel.ScaleU);
            this.ScaleV.SetViewModel(textureModel.ScaleV);

            this.Rotate.SetViewModel(textureModel.Rotate);
            this.Keying.SetViewModel(textureModel.Keying);
            this.VideoPlayer.SetViewModel(textureModel.VideoPlayerModel);
        }
    }
}
