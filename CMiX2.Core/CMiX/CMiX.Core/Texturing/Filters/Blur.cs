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
            IsExpanded = true;

            Strength = new FloatValue(blurModel.Strength);
            Visible = new BooleanValue(blurModel.Visible);
        }

        public Guid ID { get; set; }
        public FloatValue Strength { get; set; }
        public TextureFilterName Name { get; set; }
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
