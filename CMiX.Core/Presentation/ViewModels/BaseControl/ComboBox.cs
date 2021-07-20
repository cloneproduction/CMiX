// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ComboBox<T> : ObservableObject, IControl
    {
        public ComboBox(ComboBoxModel<T> comboBoxModel)
        {
            this.ID = comboBoxModel.ID;
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }


        private T _selection;
        public T Selection
        {
            get => _selection;
            set
            {
                SetProperty(ref _selection, value);
                Communicator?.SendMessage(new MessageUpdateViewModel(this));
                Console.WriteLine("Combobox Selection is " + Selection.ToString());
            }
        }


        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public IModel GetModel()
        {
            ComboBoxModel<T> model = new ComboBoxModel<T>();
            model.ID = this.ID;
            model.Selection = this.Selection;
            return model;
        }

        public void SetViewModel(IModel model)
        {
            ComboBoxModel<T> comboBoxModel = model as ComboBoxModel<T>;
            this.ID = comboBoxModel.ID;
            this.Selection = comboBoxModel.Selection;
        }
    }
}
