// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public class GradientModel : IModel
    {
        public GradientModel()
        {
            ID = Guid.NewGuid();
            Resolution = new Integer2Model(512, 512);
            From = new ColorSelectorModel("#FFFFFFFF");
            To = new ColorSelectorModel("#FF000000");
            Gamma = new FloatValueModel(2.2f);
            Horizontal = new BooleanValueModel(false);
        }

        public Guid ID { get; set; }
        public Integer2Model Resolution { get; set; }
        public FloatValueModel Gamma { get; set; }
        public ColorSelectorModel From { get; set; }
        public ColorSelectorModel To { get; set; }
        public BooleanValueModel Horizontal { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
    }
}
