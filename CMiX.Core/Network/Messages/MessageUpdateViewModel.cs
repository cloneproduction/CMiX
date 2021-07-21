// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageUpdateViewModel : IMessage
    {
        public MessageUpdateViewModel()
        {

        }

        public MessageUpdateViewModel(IControl control)
        {
            Model = control.GetModel();
            ID = control.ID;
        }

        public Guid ID { get; set; }
        public IModel Model { get; set; }

        public void Process<T>(T receiver)
        {

            var control = receiver as IControl;
            if (control.ID == this.ID)
            {
                Console.WriteLine("MessageUpdateViewModel ProcessMessage");
                control.SetViewModel(Model);
            }
        }
    }
}
