// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public interface ILayer
    {
        Guid ID { get; set; }
        AmbientOcclusion AmbientOcclusion { get; set; }
        ColorSelector BackgroundColor { get; set; }
        MasterBeat MasterBeat { get; set; }
        ModifierManager ModifierManager { get; set; }
        ICommand OpenColorSelectorCommand { get; set; }
        ToggleButton Visibility { get; set; }

        void OpenColorSelector();

        IComponentModel GetModel();
        void SetViewModel(IComponentModel model);
    }
}
