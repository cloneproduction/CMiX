// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using CMiX.Core.Prefabs.Managers;
using Modulation = CMiX.Core.Modulation;

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

        protected void AssignFromDataContext(object sender)
        {
            if (DataContext is Modulation.IModulatorBindable channel)
            {
                var selection = (sender as Control)?.DataContext as Modulation.ModulatorOutputSelection;
                channel.SetModulatorCommand.Execute(selection);
            }
        }
    }
}
