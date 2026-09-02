// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using CMiX.Core.Prefabs.Managers;
using Modulation = CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Shared base for every control that lets a ModulatableFloat be pointed at a Modulator - same pattern
    // CaptionedUserControl already uses to give every BaseControl a Caption property once instead
    // of copy-pasted into each one.
    public abstract class ModulatorAssignableUserControl : CaptionedUserControl
    {
        public static readonly StyledProperty<PrefabManager> ModulatorManagerProperty =
            AvaloniaProperty.Register<ModulatorAssignableUserControl, PrefabManager>(nameof(ModulatorManager));
        public PrefabManager ModulatorManager
        {
            get => GetValue(ModulatorManagerProperty);
            set => SetValue(ModulatorManagerProperty, value);
        }

        // Shared by any control whose own DataContext directly implements IModulatorBindable
        // (ModulatableFloat today, e.g. via ModulatableFloatValue/ModulatableFloatSlider). Does not hide the
        // flyout - the caller still owns its own named assignButton for that, declared in its own
        // XAML. The clicked row's DataContext is a ModulatorOutputSelection for a selectable row;
        // the "Unassign" button's own DataContext never matches that type, so it falls through to
        // null the same way a raw IModulator null once did, preserving today's unassign behavior.
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
