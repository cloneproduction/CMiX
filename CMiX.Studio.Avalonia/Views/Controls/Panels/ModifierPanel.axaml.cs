// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls.Panels
{
    public partial class ModifierPanel : UserControl
    {
        public ModifierPanel()
        {
            InitializeComponent();
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
