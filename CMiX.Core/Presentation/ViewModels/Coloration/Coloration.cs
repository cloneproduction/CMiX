// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Coloration : ObservableObject, IControl, IModalDialogViewModel
    {
        public Coloration(ColorationModel colorationModel, Guid componentID)
        {
            BeatModifier = new BeatModifier(colorationModel.BeatModifierModel, componentID);
            ColorSelector = new ColorSelector(colorationModel.ColorSelectorModel);

            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }

        public ICommand OpenColorSelectorCommand { get; set; }
        public ColorSelector ColorSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }

        public bool? DialogResult { get; set; }
        public Guid ID { get; set; }





        public IModel GetModel()
        {
            ColorationModel model = new ColorationModel(this.ID);

            model.ID = this.ID;
            model.ColorSelectorModel = (ColorSelectorModel)this.ColorSelector.GetModel();
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            ColorationModel colorationModel = model as ColorationModel;
            this.ID = colorationModel.ID;
            this.ColorSelector.SetViewModel(colorationModel.ColorSelectorModel);
            this.BeatModifier.SetViewModel(colorationModel.BeatModifierModel);
        }

        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService?.ShowDialog<ColorSelectorWindow>(this, ColorSelector);
            //bool? success = dialogService.ShowDialog<ColorSelectorWindow>(this, ColorSelector);

            //if (success == true)
            //{

            //}
            ////server.SetSettings(settings);
        }


    }
}
