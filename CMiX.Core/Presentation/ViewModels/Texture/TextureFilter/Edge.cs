// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Edge : ObservableObject, ITextureFilter
    {

        public Edge(EdgeModel edgeModel)
        {
            ID = edgeModel.ID;
            Enabled = edgeModel.Enabled;
            Visible = new ToggleButton(edgeModel.Visible);
            Radius = new Slider(nameof(Radius), edgeModel.Radius);
            Brightness = new Slider(nameof(Brightness), edgeModel.Brightness);
            IsExpanded = true;
        }


        public TextureFilterName Name { get; set; }
        public Guid ID { get; set; }
        public Slider Radius { get; set; }
        public Slider Brightness { get; set; }
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
            EdgeModel edgeModel = new EdgeModel();

            edgeModel.ID = ID;
            edgeModel.Enabled = Enabled;

            edgeModel.Visible = (ToggleButtonModel)Visible.GetModel();
            edgeModel.Radius = (SliderModel)Radius.GetModel();
            edgeModel.Brightness = (SliderModel)Brightness.GetModel();

            return edgeModel;
        }

        public void SetViewModel(IModel model)
        {
            EdgeModel edgeModel = model as EdgeModel;
            ID = edgeModel.ID;
            Enabled = edgeModel.Enabled;

            Visible.SetViewModel(edgeModel.Visible);
            Radius.SetViewModel(edgeModel.Radius);
            Brightness.SetViewModel(edgeModel.Brightness);
        }

        public void Dispose()
        {

        }
    }
}
