// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public class BubbleNoiseModel : IControlModel
    {
        public BubbleNoiseModel()
        {
            ID = Guid.NewGuid();
            Resolution = new Integer2Model(512, 512);
            Speed = new GenericValueModel<float>(0.0f);
            Frequency = new GenericValueModel<float>(3.5f);
            Contrast = new GenericValueModel<float>(0.15f);
            BackgroundColor = new GenericValueModel<string>("#FF000000");
            BubbleColor = new GenericValueModel<string>("#FFFFFFFF");
        }

        public Guid ID { get; set; }

        public Integer2Model Resolution { get; set; }
        public GenericValueModel<float> Speed { get; set; }
        public GenericValueModel<float> Frequency { get; set; }
        public GenericValueModel<float> Contrast { get; set; }
        public GenericValueModel<string> BackgroundColor { get; set; }
        public GenericValueModel<string> BubbleColor { get; set; }
    }
}
