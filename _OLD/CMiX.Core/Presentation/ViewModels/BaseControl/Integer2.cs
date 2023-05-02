// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Integer2 : ObservableRecipient, IControl
    {
        public Integer2(Integer2Model integer2Model)
        {
            ID = integer2Model.ID;
            X = new IntegerValue(integer2Model.X);
            Y = new IntegerValue(integer2Model.Y);
            IsActive = true;
        }

        public Guid ID { get; set; }

        public IntegerValue X { get; set; }
        public IntegerValue Y { get; set; }


        public void SetViewModel(IModel model)
        {
            Integer2Model counterModel = model as Integer2Model;
            this.ID = counterModel.ID;
            X.SetViewModel(counterModel.X);
            Y.SetViewModel(counterModel.Y);
        }

        public IModel GetModel()
        {
            Integer2Model model = new Integer2Model();
            model.ID = this.ID;
            model.X = (IntegerValueModel)X.GetModel();
            model.Y = (IntegerValueModel)Y.GetModel();
            return model;
        }
    }
}
