// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace CMiX.UI.Controls.Collection
{
    // A panel that is an item of a list. It has a Title, a body (its Content) and the buttons of its header.
    // It knows nothing about the model: the application sets the properties. A theme chooses which
    // parts of the header exist, so a variant is a theme and not a switch. The default theme has
    // a body that collapses, the IsChecked toggle as an eye, a close button and a drag handle.
    public class PanelItem : ContentControl
    {
        private Expander? _expander;

        static PanelItem()
        {
            PanelBackgroundProperty.Changed.AddClassHandler<PanelItem>((item, e) => item.UpdatePanelBackground());
        }

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<PanelItem, string>(nameof(Title), string.Empty);

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // The toggle in the header. The theme gives it a face and a meaning, for example visible or on.
        public static readonly StyledProperty<bool> IsCheckedProperty =
            AvaloniaProperty.Register<PanelItem, bool>(nameof(IsChecked), false, defaultBindingMode: BindingMode.TwoWay);

        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<PanelItem, bool>(nameof(IsExpanded), false, defaultBindingMode: BindingMode.TwoWay);

        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public static readonly StyledProperty<ICommand?> CloseCommandProperty =
            AvaloniaProperty.Register<PanelItem, ICommand?>(nameof(CloseCommand));

        public ICommand? CloseCommand
        {
            get => GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly StyledProperty<object?> CloseCommandParameterProperty =
            AvaloniaProperty.Register<PanelItem, object?>(nameof(CloseCommandParameter));

        public object? CloseCommandParameter
        {
            get => GetValue(CloseCommandParameterProperty);
            set => SetValue(CloseCommandParameterProperty, value);
        }

        public static readonly StyledProperty<bool> DragHandleIsPressedProperty =
            AvaloniaProperty.Register<PanelItem, bool>(nameof(DragHandleIsPressed), false, defaultBindingMode: BindingMode.TwoWay);

        public bool DragHandleIsPressed
        {
            get => GetValue(DragHandleIsPressedProperty);
            set => SetValue(DragHandleIsPressedProperty, value);
        }

        // The background of the body. When it is not set, the theme decides.
        public static readonly StyledProperty<IBrush?> PanelBackgroundProperty =
            AvaloniaProperty.Register<PanelItem, IBrush?>(nameof(PanelBackground));

        public IBrush? PanelBackground
        {
            get => GetValue(PanelBackgroundProperty);
            set => SetValue(PanelBackgroundProperty, value);
        }

        // True while the pointer is over the panel. The application can bind it one way to the model.
        public static readonly StyledProperty<bool> IsHoveredProperty =
            AvaloniaProperty.Register<PanelItem, bool>(nameof(IsHovered), false);

        public bool IsHovered
        {
            get => GetValue(IsHoveredProperty);
            private set => SetValue(IsHoveredProperty, value);
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
            IsHovered = true;
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            IsHovered = false;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _expander = e.NameScope.Find<Expander>("PART_Expander");
            UpdatePanelBackground();
        }

        // A TemplateBinding would reset the theme background to null when no PanelBackground is set.
        private void UpdatePanelBackground()
        {
            if (_expander != null && PanelBackground != null)
                _expander.Background = PanelBackground;
        }
    }
}
