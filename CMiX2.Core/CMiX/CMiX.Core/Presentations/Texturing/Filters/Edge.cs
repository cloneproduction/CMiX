// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
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


        public void Dispose()
        {

        }
    }
}
