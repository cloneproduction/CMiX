// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(OutputSettingsModel outputPropertiesModel)
        {
            ID = outputPropertiesModel.ID;
            Resolution = new Integer2(outputPropertiesModel.Resolution);
            BackgroundColor = new ColorSelector(outputPropertiesModel.BackgroundColor);
        }

        public Guid ID { get; set; }
        public Integer2 Resolution { get; set; }
        public ColorSelector BackgroundColor { get; set; }
    }
}
