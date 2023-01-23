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

            Factor = new FloatValue(invertModel.Factor);
            Visible = new BooleanValue(invertModel.Visible);
            InvertAlpha = new BooleanValue(invertModel.InvertAlpha);
            InvertChannelSelector = new GenericValue<InvertChannel>(invertModel.InvertChannelSelector);
            IsExpanded = true;
        }

        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }
        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public FloatValue Control { get; set; }



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

            invertModel.Factor = (FloatValueModel)Factor.GetModel();
            invertModel.Visible = (BooleanValueModel)Visible.GetModel();
            invertModel.InvertAlpha = (BooleanValueModel)InvertAlpha.GetModel();
            invertModel.InvertChannelSelector = (GenericValueModel<InvertChannel>)InvertChannelSelector.GetModel();
            return invertModel;
        }

        public void SetViewModel(IModel model)
        {
            InvertModel invertModel = model as InvertModel;

            this.ID = invertModel.ID;
            this.Name = invertModel.Name;

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
