// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class SamplerState : ObservableObject, IControl, IRecipient<IMessage>
    {
        public SamplerState(SamplerStateModel samplerStateModel)
        {
            ID = samplerStateModel.ID;
            BorderColor = new ColorSelector(samplerStateModel.ColorSelectorModel);
            AddressU = samplerStateModel.AddressU;
            AddressV = samplerStateModel.AddressV;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }

        public Guid ID { get; set; }
        public ICommand OpenColorSelectorCommand { get; set; }

        public ColorSelector BorderColor { get; set; }

        private string _AddressU;
        public string AddressU
        {
            get => _AddressU;
            set
            {
                SetProperty(ref _AddressU, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }

        private string _AddressV;
        public string AddressV
        {
            get => _AddressV;
            set
            {
                SetProperty(ref _AddressV, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }

        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.BorderColor);
        }

        public IModel GetModel()
        {
            SamplerStateModel samplerStateModel = new SamplerStateModel();

            samplerStateModel.ID = ID;
            samplerStateModel.AddressU = AddressU;
            samplerStateModel.AddressV = AddressV;
            samplerStateModel.ColorSelectorModel = (ColorSelectorModel)BorderColor.GetModel();

            return samplerStateModel;
        }

        public void SetViewModel(IModel model)
        {
            SamplerStateModel samplerStateModel = model as SamplerStateModel;
            ID = samplerStateModel.ID;
            AddressU = samplerStateModel.AddressU;
            AddressV = samplerStateModel.AddressV;
            BorderColor.SetViewModel(samplerStateModel.ColorSelectorModel);
        }

        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if(message is MessageUpdateViewModel messageUpdateViewModel)
                this.SetViewModel(messageUpdateViewModel.Model);
        }
    }
}
