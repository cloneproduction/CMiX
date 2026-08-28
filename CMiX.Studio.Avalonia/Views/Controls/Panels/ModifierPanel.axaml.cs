// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Controls.Panels
{
    public partial class ModifierPanel : UserControl
    {
        public ModifierPanel()
        {
            InitializeComponent();

            // This root spans the whole row (header + collapsible body), unlike a hover handler
            // placed on the hosted content itself, which stops receiving pointer events once the
            // Expander collapses. A no-op for every non-Modulation usage of this shared control,
            // since only IModulator-implementing items ever set IsHovered.
            PointerEntered += (sender, e) => SetHovered(true);
            PointerExited += (sender, e) => SetHovered(false);
        }

        private void SetHovered(bool value)
        {
            if (DataContext is IModulator modulator)
                modulator.IsHovered = value;
        }

        public static readonly StyledProperty<ICommand> ResetModifierCommandProperty =
            AvaloniaProperty.Register<ModifierPanel, ICommand>(nameof(ResetModifierCommand));
        public ICommand ResetModifierCommand
        {
            get => GetValue(ResetModifierCommandProperty);
            set => SetValue(ResetModifierCommandProperty, value);
        }

        // The WPF original registered this parameter as ICommand by mistake; Avalonia typed
        // properties reject the view model binding, so the parameter is object here.
        public static readonly StyledProperty<object> ResetModifierCommandParameterProperty =
            AvaloniaProperty.Register<ModifierPanel, object>(nameof(ResetModifierCommandParameter));
        public object ResetModifierCommandParameter
        {
            get => GetValue(ResetModifierCommandParameterProperty);
            set => SetValue(ResetModifierCommandParameterProperty, value);
        }

        public static readonly StyledProperty<ICommand> CloseModifierCommandProperty =
            AvaloniaProperty.Register<ModifierPanel, ICommand>(nameof(CloseModifierCommand));
        public ICommand CloseModifierCommand
        {
            get => GetValue(CloseModifierCommandProperty);
            set => SetValue(CloseModifierCommandProperty, value);
        }

        public static readonly StyledProperty<object> CloseModifierCommandParameterProperty =
            AvaloniaProperty.Register<ModifierPanel, object>(nameof(CloseModifierCommandParameter));
        public object CloseModifierCommandParameter
        {
            get => GetValue(CloseModifierCommandParameterProperty);
            set => SetValue(CloseModifierCommandParameterProperty, value);
        }

        public static readonly StyledProperty<bool> DragHandlerIsPressedProperty =
            AvaloniaProperty.Register<ModifierPanel, bool>(nameof(DragHandlerIsPressed), false, defaultBindingMode: BindingMode.TwoWay);
        public bool DragHandlerIsPressed
        {
            get => GetValue(DragHandlerIsPressedProperty);
            set => SetValue(DragHandlerIsPressedProperty, value);
        }
    }
}
