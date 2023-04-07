// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.BaseControl;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Network;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public class Integer2 : ObservableRecipient, IControl
    {
        public Integer2(Integer2Model integer2Model, CompositionService compositionService)
        {
            ID = integer2Model.ID;
            X = new IntegerValue(integer2Model.X, compositionService);
            Y = new IntegerValue(integer2Model.Y, compositionService);
            IsActive = true;
        }

        public Guid ID { get; set; }
        public IntegerValue X { get; set; }
        public IntegerValue Y { get; set; }
    }
}
