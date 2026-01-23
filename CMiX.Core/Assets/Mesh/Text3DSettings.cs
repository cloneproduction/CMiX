// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Assets.Mesh
{
    public class Text3DSettings : IControl, IPrefab
    {
        public Text3DSettings(GenericValue<string> text,
                              GenericValue<int> fontSize,
                              GenericValue<float> extrudeAmount,
                              GenericValue<string> fontFamily, 
                              GenericValue<HorizontalAlignment> horizontalAlignment, 
                              GenericValue<ParagraphAlignment> paragraphAlignment)
        {
            Text = text;
            FontSize = fontSize;
            ExtrudeAmount = extrudeAmount;
            FontFamily = fontFamily;
            HorizontalAlignment = horizontalAlignment;
            ParagraphAlignment = paragraphAlignment;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<string> Text { get; set; }
        public GenericValue<int> FontSize { get; set; }
        public GenericValue<float> ExtrudeAmount { get; set; }
        public GenericValue<string> FontFamily { get; set; }
        public GenericValue<HorizontalAlignment> HorizontalAlignment { get; set; }
        public GenericValue<ParagraphAlignment> ParagraphAlignment { get; set; }
    }
}
