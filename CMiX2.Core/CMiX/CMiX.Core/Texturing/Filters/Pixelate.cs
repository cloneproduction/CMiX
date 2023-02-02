// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Pixelate : ObservableObject, ITextureFilter
    {
        public Pixelate(PixelateModel pixelateModel)
        {
            ID = pixelateModel.ID;
            Visible = new BooleanValue(pixelateModel.Visible);
            Name = pixelateModel.Name;
            Control = new FloatValue(pixelateModel.Control);
            Factor = new Vector2(pixelateModel.Factor);

            IsExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }

        public Vector2 Factor { get; set; }
        public FloatValue Control { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public IModel GetModel()
        {
            PixelateModel pixelateModel = new PixelateModel();

            pixelateModel.ID = ID;
            pixelateModel.Name = Name;
            pixelateModel.Visible = (BooleanValueModel)Visible.GetModel();
            pixelateModel.Control = (FloatValueModel)Control.GetModel();
            pixelateModel.Factor = (Vector2Model)Factor.GetModel();

            return pixelateModel;
        }

        public void Dispose()
        {

        }
    }
}
