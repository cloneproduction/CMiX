// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class TypeWriter : ObservableRecipient, IControl
    {
        public TypeWriter(GenericValue<string> stringControl, 
                          GenericValue<string> fontFamily, 
                          GenericValue<float> fontSize, 
                          GenericValue<FontStyle> fontStyle,
                          GenericValue<string> fontColor,
                          GenericValue<string> backgroundColor, 
                          Integer2 resolution, 
                          Vector2 position)
        {
            StringControl = stringControl;
            FontFamily = fontFamily;
            FontSize = fontSize;
            Style = fontStyle;
            FontColor = fontColor;
            BackgroundColor = backgroundColor;
            Resolution = resolution;
            Position = position;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<string> StringControl { get; set; }
        public GenericValue<FontStyle> Style { get; set; }
        public GenericValue<string> FontFamily { get; set; }
        public GenericValue<float> FontSize { get; set; }
        public GenericValue<string> FontColor { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public Integer2 Resolution { get; set; }
        public Vector2 Position { get; set; }
    }
}
