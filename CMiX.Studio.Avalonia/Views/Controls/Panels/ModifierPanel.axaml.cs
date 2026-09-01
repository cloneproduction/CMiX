// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Views.Controls.Panels
{
    public partial class ModifierPanel : HeaderedContentControl
    {
        private Expander _expander;

        static ModifierPanel()
        {
            PanelBackgroundProperty.Changed.AddClassHandler<ModifierPanel>((x, e) => x.UpdatePanelBackground());
        }

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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _expander = e.NameScope.Find<Expander>("PART_Expander");
            UpdatePanelBackground();
        }

        // Only ever calls SetValue when a real override is supplied, leaving Expander.Background
        // completely unset otherwise - that lets the ItemControlExpander theme's own default
        // Setter apply exactly as it does today, with no risk of a bound null value (which would
        // still be a real, higher-priority local value) blanking out the theme's background.
        private void UpdatePanelBackground()
        {
            if (_expander != null && PanelBackground != null)
                _expander.Background = PanelBackground;
        }

        // Lets a consumer distinguish nested panels (e.g. a Modulator list rendered inside a
        // Modifier) that would otherwise share the exact same ItemControlExpander background.
        public static readonly StyledProperty<IBrush> PanelBackgroundProperty =
            AvaloniaProperty.Register<ModifierPanel, IBrush>(nameof(PanelBackground));
        public IBrush PanelBackground
        {
            get => GetValue(PanelBackgroundProperty);
            set => SetValue(PanelBackgroundProperty, value);
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
