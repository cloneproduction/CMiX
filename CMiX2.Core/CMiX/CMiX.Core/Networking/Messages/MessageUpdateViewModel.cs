// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Networking.Messages
{
    public class MessageUpdateViewModel : IMessage
    {
        public MessageUpdateViewModel()
        {

        }

        public MessageUpdateViewModel(IModel model)
        {
            Model = model;
            ID = model.ID;
        }

        public Guid ID { get; set; }
        public IModel Model { get; set; }
    }
}
