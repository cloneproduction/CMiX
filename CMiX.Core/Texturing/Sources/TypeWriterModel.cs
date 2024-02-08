// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public class TypeWriterModel : IControlModel
    {
        public TypeWriterModel()
        {
            ID = Guid.NewGuid();

            StringControl = new GenericValueModel<string>();
            FontFamily = new GenericValueModel<string>("Arial");
            FontSize = new GenericValueModel<float>(0.45f);
            FontColor = new GenericValueModel<string>("#ff000000");
            BackgroundColor = new GenericValueModel<string>("#00000000");
            Resolution = new Integer2Model(1024, 1024);
            Position = new Vector2Model();
            Style = new GenericValueModel<FontStyle>(FontStyle.Normal);
        }

        public Guid ID { get; set; }

        public GenericValueModel<string> StringControl { get; internal set; }
        public GenericValueModel<string> FontColor { get; internal set; }
        public GenericValueModel<string> BackgroundColor { get; internal set; }
        public Integer2Model Resolution { get; internal set; }
        public Vector2Model Position { get; internal set; }
        public GenericValueModel<float> FontSize { get; internal set; }
        public GenericValueModel<string> FontFamily { get; internal set; }
        public GenericValueModel<FontStyle> Style { get; internal set; }
    }
}
