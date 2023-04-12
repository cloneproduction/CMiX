// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Components
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(OutputSettingsModel outputPropertiesModel, CompositionService compositionService)
        {
            ID = outputPropertiesModel.ID;
            CompositionService = compositionService;
            Resolution = new Integer2(outputPropertiesModel.Resolution, compositionService);
            BackgroundColor = new ColorSelector(outputPropertiesModel.BackgroundColor, compositionService);
        }

        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public Integer2 Resolution { get; set; }
        public ColorSelector BackgroundColor { get; set; }
    }
}
