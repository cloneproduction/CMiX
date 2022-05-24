// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Services;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public interface ILayer
    {
        Guid ID { get; set; }
        AmbientOcclusion AmbientOcclusion { get; set; }
        ColorSelector BackgroundColor { get; set; }
        Camera Camera { get; set; }
        CameraManager CameraManager { get; set; }

        MasterBeat MasterBeat { get; set; }
        ObservableCollection<Mesh> MeshEntities { get; set; }
        ObservableCollection<LightEntity> LightEntities { get; set; }
        MeshManager MeshEntityManager { get; set; }
        LightEntityManager LightEntityManager { get; set; }

        ModifierManager ModifierManager { get; set; }
        ICommand OpenColorSelectorCommand { get; set; }
        ToggleButton Visibility { get; set; }

        void OpenColorSelector();

        IComponentModel GetModel();
        void SetViewModel(IComponentModel model);
    }
}
