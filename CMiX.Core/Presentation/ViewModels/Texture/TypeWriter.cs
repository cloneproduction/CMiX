// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TypeWriter : ObservableRecipient, IControl
    {
        public TypeWriter(TypeWriterModel typeWriterModel)
        {
            ID = typeWriterModel.ID;
            StringControl = new StringControl(typeWriterModel.StringControl);

            FontFamily = new ComboBox<string>(typeWriterModel.FontFamily);

            FontSize = new Slider(nameof(FontSize), typeWriterModel.FontSize);
            Style = new ComboBox<FontStyle>(typeWriterModel.Style);

            FontColor = new ColorSelector(typeWriterModel.FontColor);
            BackgroundColor = new ColorSelector(typeWriterModel.BackgroundColor);

            ResolutionX = new Counter(typeWriterModel.ResolutionX);
            ResolutionY = new Counter(typeWriterModel.ResolutionY);

            PositionX = new Slider(nameof(PositionX), typeWriterModel.PositionX);
            PositionY = new Slider(nameof(PositionY), typeWriterModel.PositionY);

            OpenFontColorCommand = new RelayCommand(OpenFontColor);
            OpenFontBackgroundCommand = new RelayCommand(OpenBackgroundColor);

            TextInputGotFocusCommand = new RelayCommand(TextInputGotFocus);
            TextInputLostFocusCommand = new RelayCommand(TextInputLostFocus);
        }


        public ICommand OpenFontColorCommand { get; set; }
        public ICommand OpenFontBackgroundCommand { get; set; }
        public ICommand TextInputGotFocusCommand { get; set; }
        public ICommand TextInputLostFocusCommand { get; set; }

        public Guid ID { get; set; }
        public StringControl StringControl { get; set; }

        public ComboBox<FontStyle> Style { get; set; }
        public ComboBox<string> FontFamily { get; set; }
        public Slider FontSize { get; set; }

        public ColorSelector FontColor { get; set; }
        public ColorSelector BackgroundColor { get; set; }

        public Counter ResolutionX { get; set; }
        public Counter ResolutionY { get; set; }

        public Slider PositionX { get; set; }
        public Slider PositionY { get; set; }


        public void OpenFontColor()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.FontColor);
        }

        public void OpenBackgroundColor()
        {
            IDialogService dialogService = WeakReferenceMessenger.Default.Send(new MessageRequestDialogService(), MessageType.Internal).Response;
            dialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }

        public void TextInputGotFocus()
        {
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageTextInputFocusState(this, true), MessageType.In);
        }

        public void TextInputLostFocus()
        {
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageTextInputFocusState(this, false), MessageType.Internal);
        }

        public IModel GetModel()
        {
            TypeWriterModel typeWriterModel = new TypeWriterModel();

            typeWriterModel.ID = ID;
            typeWriterModel.StringControl = (StringControlModel)this.StringControl.GetModel();

            typeWriterModel.FontColor = (ColorSelectorModel)this.FontColor.GetModel();
            typeWriterModel.BackgroundColor = (ColorSelectorModel)this.BackgroundColor.GetModel();

            typeWriterModel.ResolutionX = (CounterModel)this.ResolutionX.GetModel();
            typeWriterModel.ResolutionY = (CounterModel)this.ResolutionY.GetModel();

            typeWriterModel.PositionX = (SliderModel)this.PositionX.GetModel();
            typeWriterModel.PositionY = (SliderModel)this.PositionY.GetModel();

            typeWriterModel.FontSize = (SliderModel)this.FontSize.GetModel();
            typeWriterModel.FontFamily = (ComboBoxModel<string>)this.FontFamily.GetModel();
            typeWriterModel.Style = (ComboBoxModel<FontStyle>)this.Style.GetModel();

            return typeWriterModel;
        }

        public void SetViewModel(IModel model)
        {
            TypeWriterModel typeWriterModel = model as TypeWriterModel;

            this.ID = typeWriterModel.ID;
            this.StringControl.SetViewModel(typeWriterModel.StringControl);

            this.FontColor.SetViewModel(typeWriterModel.FontColor);
            this.BackgroundColor.SetViewModel(typeWriterModel.BackgroundColor);

            this.ResolutionX.SetViewModel(typeWriterModel.ResolutionX);
            this.ResolutionY.SetViewModel(typeWriterModel.ResolutionY);

            this.PositionX.SetViewModel(typeWriterModel.PositionX);
            this.PositionY.SetViewModel(typeWriterModel.PositionY);

            this.FontSize.SetViewModel(typeWriterModel.FontSize);
            this.FontFamily.SetViewModel(typeWriterModel.FontFamily);
            this.Style.SetViewModel(typeWriterModel.Style);
        }
    }
}
