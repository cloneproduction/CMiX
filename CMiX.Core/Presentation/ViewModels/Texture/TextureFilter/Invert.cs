// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Invert : ObservableObject, ITextureFilter
    {
        public Invert(InvertModel invertModel)
        {
            ID = invertModel.ID;
            Name = invertModel.Name;
            Enabled = invertModel.Enabled;

            Factor = new Slider(nameof(Factor), invertModel.Factor);
            Visible = new ToggleButton(invertModel.Visible);
            InvertAlpha = new ToggleButton(invertModel.InvertAlpha);
            InvertChannelSelector = new ComboBox<InvertChannel>(invertModel.InvertChannelSelector);
            IsExpanded = true;
        }

        public Guid ID { get; set; }
        public Slider Factor { get; set; }
        public TextureFilterName Name { get; set; }
        public ToggleButton Visible { get; set; }
        public ToggleButton InvertAlpha { get; set; }
        public ComboBox<InvertChannel> InvertChannelSelector { get; set; }
        public Slider Control { get; set; }


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
            InvertModel invertModel = new InvertModel();

            invertModel.ID = ID;
            invertModel.Name = Name;
            invertModel.Enabled = Enabled;

            invertModel.Factor = (SliderModel)Factor.GetModel();
            invertModel.Visible = (ToggleButtonModel)Visible.GetModel();
            invertModel.InvertAlpha = (ToggleButtonModel)InvertAlpha.GetModel();
            invertModel.InvertChannelSelector = (ComboBoxModel<InvertChannel>)InvertChannelSelector.GetModel();
            return invertModel;
        }

        public void SetViewModel(IModel model)
        {
            InvertModel invertModel = model as InvertModel;

            this.ID = invertModel.ID;
            this.Name = invertModel.Name;
            this.Enabled = invertModel.Enabled;

            this.Factor.SetViewModel(invertModel.Factor);
            this.Visible.SetViewModel(invertModel.Visible);
            this.InvertAlpha.SetViewModel(invertModel.InvertAlpha);
            this.InvertChannelSelector.SetViewModel(invertModel.InvertChannelSelector);
        }

        public void Dispose()
        {

        }
    }
}
