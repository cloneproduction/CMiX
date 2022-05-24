// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Echo : ObservableObject, ITextureFilter
    {
        public Echo(EchoModel echoModel)
        {
            ID = echoModel.ID;
            Visible = new ToggleButton(echoModel.Visible);
            Name = echoModel.Name;
            Factor = new Slider(nameof(Factor), echoModel.Factor);

            IsExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }
        public Guid ID { get; set; }
        public Slider Factor { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public IModel GetModel()
        {
            EchoModel echoModel = new EchoModel();

            echoModel.ID = ID;
            echoModel.Name = Name;
            echoModel.Visible = (ToggleButtonModel)Visible.GetModel();
            echoModel.Factor = (SliderModel)Factor.GetModel();

            return echoModel;
        }

        public void SetViewModel(IModel model)
        {
            EchoModel pixelateModel = model as EchoModel;

            ID = pixelateModel.ID;
            Name = pixelateModel.Name;
            Visible.SetViewModel(pixelateModel.Visible);
            Factor.SetViewModel(pixelateModel.Factor);
        }

        public void Dispose()
        {

        }
    }
}
