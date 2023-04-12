// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentations.Texturing.Sources
{
    public class TypeWriter : ObservableRecipient, IControl
    {
        public TypeWriter(TypeWriterModel typeWriterModel, CompositionService compositionService)
        {
            ID = typeWriterModel.ID;
            StringControl = new StringValue(typeWriterModel.StringControl, compositionService);
            CompositionService = compositionService;
            FontFamily = new StringValue(typeWriterModel.FontFamily, compositionService);
            FontSize = new FloatValue(typeWriterModel.FontSize, compositionService);
            Style = new GenericValue<FontStyle>(typeWriterModel.Style, compositionService);
            FontColor = new ColorSelector(typeWriterModel.FontColor, compositionService);
            BackgroundColor = new ColorSelector(typeWriterModel.BackgroundColor, compositionService);
            Resolution = new Integer2(typeWriterModel.Resolution, compositionService);
            Position = new Vector2(typeWriterModel.Position, compositionService);
            TextInputGotFocusCommand = new RelayCommand(TextInputGotFocus);
            TextInputLostFocusCommand = new RelayCommand(TextInputLostFocus);
        }


        public ICommand TextInputGotFocusCommand { get; set; }
        public ICommand TextInputLostFocusCommand { get; set; }

        public CompositionService CompositionService { get; set; }
        public Guid ID { get; set; }
        public StringValue StringControl { get; set; }
        public GenericValue<FontStyle> Style { get; set; }
        public StringValue FontFamily { get; set; }
        public FloatValue FontSize { get; set; }
        public ColorSelector FontColor { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public Integer2 Resolution { get; set; }
        public Vector2 Position { get; set; }

        public void TextInputGotFocus()
        {
            //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageTextInputFocusState(this, true), MessageType.In);
        }
        public void TextInputLostFocus()
        {
            //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageTextInputFocusState(this, false), MessageType.Internal);
        }
    }
}
