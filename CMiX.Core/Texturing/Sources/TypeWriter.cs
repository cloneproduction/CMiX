// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Texturing.Sources
{
    public class TypeWriter : ObservableRecipient, IControl
    {
        public TypeWriter(
                    StringValue stringControl, 
                    StringValue fontFamily, 
                    FloatValue fontSize, 
                    GenericValue<FontStyle> fontStyle, 
                    ColorValue fontColor, 
                    ColorValue backgroundColor, 
                    Integer2 resolution, 
                    Vector2 position)
        {
            StringControl = stringControl;// new StringValue(String.Empty);
            FontFamily = fontFamily;// new StringValue("Arial");
            FontSize = fontSize;// new FloatValue(0.0f);
            Style = fontStyle;// new GenericValue<FontStyle>();
            FontColor = fontColor;// new ColorValue();
            BackgroundColor = backgroundColor;// new ColorValue();
            Resolution = resolution;// new Integer2();
            Position = position;// new Vector2();

            //TextInputGotFocusCommand = new RelayCommand(TextInputGotFocus);
            //TextInputLostFocusCommand = new RelayCommand(TextInputLostFocus);
        }

        public ICommand TextInputGotFocusCommand { get; set; }
        public ICommand TextInputLostFocusCommand { get; set; }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue StringControl { get; set; }
        public GenericValue<FontStyle> Style { get; set; }
        public StringValue FontFamily { get; set; }
        public FloatValue FontSize { get; set; }
        public ColorValue FontColor { get; set; }
        public ColorValue BackgroundColor { get; set; }
        public Integer2 Resolution { get; set; }
        public Vector2 Position { get; set; }

        //public void TextInputGotFocus()
        //{
        //    //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageTextInputFocusState(this, true), MessageType.In);
        //}
        //public void TextInputLostFocus()
        //{
        //    //WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageTextInputFocusState(this, false), MessageType.Internal);
        //}
    }
}
