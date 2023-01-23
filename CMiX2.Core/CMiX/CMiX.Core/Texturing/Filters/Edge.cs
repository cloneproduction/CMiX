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
            Name = edgeModel.Name;
            Visible = new BooleanValue(edgeModel.Visible);
            Radius = new FloatValue(edgeModel.Radius);
            Brightness = new FloatValue(edgeModel.Brightness);
            Control = new FloatValue(edgeModel.Control);
            IsExpanded = true;
        }


        public TextureFilterName Name { get; set; }
        public Guid ID { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Brightness { get; set; }
        public FloatValue Control { get; set; }
        public BooleanValue Visible { get; set; }


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
            edgeModel.Name = Name;

            edgeModel.Control = (FloatValueModel)Control.GetModel();
            edgeModel.Visible = (BooleanValueModel)Visible.GetModel();
            edgeModel.Radius = (FloatValueModel)Radius.GetModel();
            edgeModel.Brightness = (FloatValueModel)Brightness.GetModel();

            return edgeModel;
        }

        public void SetViewModel(IModel model)
        {
            EdgeModel edgeModel = model as EdgeModel;
            ID = edgeModel.ID;
            Name = edgeModel.Name;

            Control.SetViewModel(edgeModel.Control);
            Visible.SetViewModel(edgeModel.Visible);
            Radius.SetViewModel(edgeModel.Radius);
            Brightness.SetViewModel(edgeModel.Brightness);
        }

        public void Dispose()
        {

        }
    }
}
