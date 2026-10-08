// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public abstract class ModulatorAssignableUserControl : CaptionedUserControl
    {
        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<ModulatorAssignableUserControl, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
        }
    }
}
