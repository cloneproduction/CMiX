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

            StringControl = new StringValueModel();
            FontFamily = new StringValueModel("Arial");
            FontSize = new FloatValueModel(0.45f);
            FontColor = new ColorSelectorModel("#ff000000");
            BackgroundColor = new ColorSelectorModel("#00000000");
            Resolution = new Integer2Model(1024, 1024);
            Position = new Vector2Model();
            Style = new GenericValueModel<FontStyle>(FontStyle.Normal);
        }

        public Guid ID { get; set; }

        public StringValueModel StringControl { get; internal set; }
        public ColorSelectorModel FontColor { get; internal set; }
        public ColorSelectorModel BackgroundColor { get; internal set; }
        public Integer2Model Resolution { get; internal set; }
        public Vector2Model Position { get; internal set; }
        public FloatValueModel FontSize { get; internal set; }
        public StringValueModel FontFamily { get; internal set; }
        public GenericValueModel<FontStyle> Style { get; internal set; }
    }
}
