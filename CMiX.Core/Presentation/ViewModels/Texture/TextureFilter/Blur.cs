// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Blur : ObservableObject, ITextureFilter
    {
        public Blur(BlurModel blurModel)
        {
            ID = blurModel.ID;
            Name = blurModel.Name;
            Enabled = blurModel.Enabled;
            IsExpanded = true;

            Strength = new Slider(nameof(Strength), blurModel.Strength);
            Visible = new ToggleButton(blurModel.Visible);
        }

        public Guid ID { get; set; }
        public Slider Strength { get; set; }
        public TextureFilterName Name { get; set; }
        public ToggleButton Visible { get; set; }


        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            BlurModel blurModel = new BlurModel();
            blurModel.ID = ID;
            blurModel.Name = Name;
            blurModel.Enabled = Enabled;

            blurModel.Strength = (SliderModel)Strength.GetModel();
            blurModel.Visible = (ToggleButtonModel)Visible.GetModel();

            return blurModel;
        }

        public void SetViewModel(IModel model)
        {
            BlurModel blurModel = model as BlurModel;
            this.ID = blurModel.ID;
            this.Name = blurModel.Name;
            this.Enabled = blurModel.Enabled;

            this.Visible.SetViewModel(blurModel.Visible);
            this.Strength.SetViewModel(blurModel.Strength);
        }

        public void Dispose()
        {

        }
    }
}
