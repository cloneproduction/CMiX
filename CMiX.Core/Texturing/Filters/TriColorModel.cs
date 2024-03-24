// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class TriColorModel : IModifierModel
    {
        public TriColorModel()
        {
            ID = Guid.NewGuid();

            Visible = new GenericValueModel<bool>(true);
            Control = new GenericValueModel<float>(1.0f);
            ColorA = new GenericValueModel<string>("#FFFF00FF");
            ColorB = new GenericValueModel<string>("#FFFF00FF");
            ColorC = new GenericValueModel<string>("#FFFF00FF");
            Smooth = new GenericValueModel<float>(0.5f);
            Center = new GenericValueModel<float>(0.5f);
            SingleChannel = new GenericValueModel<bool>(true);
            ClampColor = new GenericValueModel<bool>(true);
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public GenericValueModel<float> Control { get; set; }
        public GenericValueModel<string> ColorA { get; set; }
        public GenericValueModel<string> ColorB { get; set; }
        public GenericValueModel<string> ColorC { get; set; }
        public GenericValueModel<float> Smooth { get; set; }
        public GenericValueModel<float> Center { get; set; }
        public GenericValueModel<bool> SingleChannel { get; set; }
        public GenericValueModel<bool> ClampColor { get; set; }
    }
}
