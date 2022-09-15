// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public interface ILayer
    {
        Guid ID { get; set; }
        AmbientOcclusion AmbientOcclusion { get; set; }
        ColorSelector BackgroundColor { get; set; }
        ModifierManager TextureModifierManager { get; set; }
        ICommand OpenColorSelectorCommand { get; set; }
        ToggleButton Visibility { get; set; }

        PrefabManager<Entity> ModelEntityManager { get; set; }
        PrefabManager<Camera> CameraEntityManager { get; set; }

        void OpenColorSelector();

        IModel GetModel();
        void SetViewModel(IModel model);
    }
}
