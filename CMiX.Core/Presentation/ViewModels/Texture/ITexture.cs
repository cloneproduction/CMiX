// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Assets;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface ITexture
    {
        Guid ID { get; set; }
        ImageSelector ImageSelector { get; set; }
        ToggleButton IsEnabled { get; set; }
        ModifierManager ModifierManager { get; set; }
        ComboBox<int> SelectedAssetType { get; set; }
        ModifierManager TextureTransformModifierManager { get; set; }

        SamplerState SamplerState { get; set; }
        TypeWriter TypeWriter { get; set; }
        VideoIn VideoIn { get; set; }
        VideoPlayer VideoPlayer { get; set; }
        VideoSelector VideoSelector { get; set; }
    }
}
