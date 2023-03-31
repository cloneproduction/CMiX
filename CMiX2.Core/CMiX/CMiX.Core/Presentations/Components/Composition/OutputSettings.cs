// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Components
{
    public class OutputSettings : ObservableObject, IControl
    {
        public OutputSettings(OutputSettingsModel outputPropertiesModel, CompositionService compositionService)
        {
            ID = outputPropertiesModel.ID;
            CompositionService = compositionService;

            Resolution = new Integer2(outputPropertiesModel.Resolution);
            BackgroundColor = new ColorSelector(outputPropertiesModel.BackgroundColor);
        }


        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public Integer2 Resolution { get; set; }
        public ColorSelector BackgroundColor { get; set; }
    }
}
