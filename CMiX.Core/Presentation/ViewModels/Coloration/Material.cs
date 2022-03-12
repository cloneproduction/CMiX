// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using System.Windows.Media;
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
    public class Material : ObservableObject, IControl, IBeatable, IDisposable
    {
        public Material(MaterialModel colorationModel)
        {
            this.ID = colorationModel.ID;
            BeatModifier = new BeatModifier(colorationModel.BeatModifierModel);
            BaseColor = new ColorSelector(colorationModel.ColorSelectorModel);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);
        }

        public Guid ID { get; set; }
        public ICommand OpenColorSelectorCommand { get; set; }
        public ColorSelector BaseColor { get; set; }
        public BeatModifier BeatModifier { get; set; }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            BeatModifier.SetMasterBeat(masterBeat);
        }

        public void OpenColorSelector()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.BaseColor);
        }

        public IModel GetModel()
        {
            MaterialModel model = new MaterialModel();
            model.ID = this.ID;
            model.ColorSelectorModel = (ColorSelectorModel)this.BaseColor.GetModel();
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            return model;
        }

        public void SetViewModel(IModel model)
        {
            MaterialModel colorationModel = model as MaterialModel;
            this.ID = colorationModel.ID;
            this.BaseColor.SetViewModel(colorationModel.ColorSelectorModel);
            this.BeatModifier.SetViewModel(colorationModel.BeatModifierModel);
        }


        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
