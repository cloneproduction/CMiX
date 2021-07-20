// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Counter : ObservableObject, IControl
    {
        public Counter(CounterModel counterModel)
        {
            this.ID = counterModel.ID;
            Count = 1;
            AddCommand = new RelayCommand(Add);
            SubCommand = new RelayCommand(Sub);
        }


        public event EventHandler<CounterEventArgs> CounterChangeEvent;
        public void OnCountChange()
        {
            CounterChangeEvent?.Invoke(this, new CounterEventArgs(this.Count));
        }


        public ICommand AddCommand { get; }
        public ICommand SubCommand { get; }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                SetProperty(ref _count, value);
                OnCountChange();
                Communicator?.SendMessage(new MessageUpdateViewModel(this));
            }
        }

        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }

        private void Add() => Count += 1;

        private void Sub()
        {
            if (Count > 1)
                Count -= 1;
        }

        public void SetViewModel(IModel model)
        {
            CounterModel counterModel = model as CounterModel;
            this.ID = counterModel.ID;
            this.Count = counterModel.Count;
        }

        public IModel GetModel()
        {
            CounterModel model = new CounterModel();
            model.ID = this.ID;
            model.Count = this.Count;
            return model;
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
    }
}
