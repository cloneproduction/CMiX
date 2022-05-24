// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class OutputProperties : ObservableObject, IControl
    {
        public OutputProperties(OutputPropertiesModel outputPropertiesModel)
        {
            ID = outputPropertiesModel.ID;
            ResolutionX = new Counter(outputPropertiesModel.ResolutionX);
            ResolutionY = new Counter(outputPropertiesModel.ResolutionY);
            BackgroundColor = new ColorSelector(outputPropertiesModel.BackgroundColor);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }


        public Guid ID { get; set; }
        public Counter ResolutionX { get; set; }
        public Counter ResolutionY { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ICommand OpenColorSelectorCommand { get; set; }


        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }

        public IModel GetModel()
        {
            OutputPropertiesModel outputPropertiesModel = new OutputPropertiesModel();

            outputPropertiesModel.ID = ID;
            outputPropertiesModel.ResolutionX = (CounterModel)ResolutionX.GetModel();
            outputPropertiesModel.ResolutionY = (CounterModel)ResolutionY.GetModel();
            outputPropertiesModel.BackgroundColor = (ColorSelectorModel)BackgroundColor.GetModel();

            return outputPropertiesModel;
        }

        public void SetViewModel(IModel model)
        {
            OutputPropertiesModel outputPropertiesModel = model as OutputPropertiesModel;
            this.ID = outputPropertiesModel.ID;
            this.ResolutionX.SetViewModel(outputPropertiesModel.ResolutionX);
            this.ResolutionY.SetViewModel(outputPropertiesModel.ResolutionY);
            this.BackgroundColor.SetViewModel(outputPropertiesModel.BackgroundColor);
        }
    }
}
