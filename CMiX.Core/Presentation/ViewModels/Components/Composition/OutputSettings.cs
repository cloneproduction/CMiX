// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Components
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

        public IModel GetModel()
        {
            OutputSettingsModel outputPropertiesModel = new OutputSettingsModel();

            outputPropertiesModel.ID = ID;
            outputPropertiesModel.Resolution = (Integer2Model)Resolution.GetModel();
            outputPropertiesModel.BackgroundColor = (ColorSelectorModel)BackgroundColor.GetModel();

            return outputPropertiesModel;
        }

        public void SetViewModel(IModel model)
        {
            OutputSettingsModel outputPropertiesModel = model as OutputSettingsModel;
            this.ID = outputPropertiesModel.ID;
            this.Resolution.SetViewModel(outputPropertiesModel.Resolution);
            this.BackgroundColor.SetViewModel(outputPropertiesModel.BackgroundColor);
        }
    }
}
