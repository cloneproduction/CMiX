// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Texture : ObservableObject, IControl
    {
        public Texture(MasterBeat beat, TextureModel textureModel)
        {
            this.ID = textureModel.ID;

            Inverter = new Inverter(nameof(Inverter), textureModel.InverterModel);

            Brightness = new Slider(nameof(Brightness), textureModel.Brightness) { Minimum = -1.0, Maximum = 1.0 };
            Contrast = new Slider(nameof(Contrast), textureModel.Contrast) { Minimum = -1.0, Maximum = 1.0 };
            Hue = new Slider(nameof(Hue), textureModel.Hue) { Minimum = -1.0, Maximum = 1.0 };
            Saturation = new Slider(nameof(Saturation), textureModel.Saturation) { Minimum = -1.0, Maximum = 1.0 };
            Luminosity = new Slider(nameof(Luminosity), textureModel.Luminosity) { Minimum = -1.0, Maximum = 1.0 };
            Keying = new Slider(nameof(Keying), textureModel.Keying);
            Scale = new Slider(nameof(Scale), textureModel.Scale) { Minimum = -1.0, Maximum = 1.0 };
            Rotate = new Slider(nameof(Rotate), textureModel.Rotate) { Minimum = -1.0, Maximum = 1.0 };
            Pan = new Slider(nameof(Pan), textureModel.Pan) { Minimum = -1.0, Maximum = 1.0 };
            Tilt = new Slider(nameof(Tilt), textureModel.Tilt) { Minimum = -1.0, Maximum = 1.0 };
            AssetPathSelector = new AssetPathSelector(new AssetTexture(), textureModel.AssetPathSelectorModel);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public AssetPathSelector AssetPathSelector { get; set; }
        public Slider Brightness { get; set; }
        public Slider Contrast { get; set; }
        public Slider Hue { get; set; }
        public Slider Saturation { get; set; }
        public Slider Luminosity { get; set; }
        public Slider Keying { get; set; }
        public Slider Pan { get; set; }
        public Slider Tilt { get; set; }
        public Slider Scale { get; set; }
        public Slider Rotate { get; set; }
        public Inverter Inverter { get; set; }


        public IModel GetModel()
        {
            TextureModel model = new TextureModel();

            model.ID = this.ID;
            model.AssetPathSelectorModel = (AssetPathSelectorModel)this.AssetPathSelector.GetModel();
            model.InverterModel = (InverterModel)this.Inverter.GetModel();
            model.Brightness = (SliderModel)this.Brightness.GetModel();
            model.Contrast = (SliderModel)this.Contrast.GetModel();
            model.Saturation = (SliderModel)this.Saturation.GetModel();
            model.Luminosity = (SliderModel)this.Luminosity.GetModel();
            model.Hue = (SliderModel)this.Hue.GetModel();
            model.Pan = (SliderModel)this.Pan.GetModel();
            model.Tilt = (SliderModel)this.Tilt.GetModel();
            model.Scale = (SliderModel)this.Scale.GetModel();
            model.Rotate = (SliderModel)this.Rotate.GetModel();
            model.Keying = (SliderModel)this.Keying.GetModel();

            return model;
        }


        public void SetViewModel(IModel model)
        {
            TextureModel textureModel = model as TextureModel;

            this.ID = textureModel.ID;
            this.AssetPathSelector.SetViewModel(textureModel.AssetPathSelectorModel);
            this.Inverter.SetViewModel(textureModel.InverterModel);
            this.Brightness.SetViewModel(textureModel.Brightness);
            this.Contrast.SetViewModel(textureModel.Contrast);
            this.Saturation.SetViewModel(textureModel.Saturation);
            this.Luminosity.SetViewModel(textureModel.Luminosity);
            this.Hue.SetViewModel(textureModel.Hue);
            this.Pan.SetViewModel(textureModel.Pan);
            this.Tilt.SetViewModel(textureModel.Tilt);
            this.Scale.SetViewModel(textureModel.Scale);
            this.Rotate.SetViewModel(textureModel.Rotate);
            this.Keying.SetViewModel(textureModel.Keying);
        }
    }
}
