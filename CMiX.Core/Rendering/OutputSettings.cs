// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(Integer2 resolution, 
                              GenericValue<string> backgroundColor)
        {
            Resolution = resolution;
            BackgroundColor = backgroundColor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }

        public IControlModel ToModel() => new OutputSettingsModel
        {
            ID = ID,
            Resolution = (Integer2Model)Resolution.ToModel(),
            BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (OutputSettingsModel)model;
            ID = m.ID;
            Resolution.FromModel(m.Resolution);
            BackgroundColor.FromModel(m.BackgroundColor);
        }
    }
}
