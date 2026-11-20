// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;

namespace CMiX.UI.Controls.Parts
{
    // The search field with a magnifier. The lists with a search (GroupedSearchList, ListSearchableItemPicker) and the drop-downs with a
    // search (see ComboBoxSearch) use this one control, so every search field looks and works the same. A key of the field
    // goes up to this control as a normal routed event. ShowsClear adds a clear button inside the field.
    public partial class SearchBox : UserControl
    {
        // The padding of the text: room for the magnifier at the left, and with ShowsClear room for the clear button at the right.
        private static readonly Thickness TextPadding = new(28, 0, 6, 0);
        private static readonly Thickness TextPaddingWithClear = new(28, 0, 28, 0);

        static SearchBox()
        {
            TextProperty.Changed.AddClassHandler<SearchBox>((box, _) => box.UpdateClear());
            ShowsClearProperty.Changed.AddClassHandler<SearchBox>((box, _) => box.UpdateClear());
            IsReadOnlyProperty.Changed.AddClassHandler<SearchBox>((box, _) => box.UpdateClear());
        }

        public SearchBox()
        {
            InitializeComponent();
            UpdateClear();
        }

        // True: a clear button is inside the field, at its right, while the field holds text. False by default.
        public static readonly StyledProperty<bool> ShowsClearProperty =
            AvaloniaProperty.Register<SearchBox, bool>(nameof(ShowsClear));

        public bool ShowsClear
        {
            get => GetValue(ShowsClearProperty);
            set => SetValue(ShowsClearProperty, value);
        }

        // The name of the clear button, so a test or an owner can find it.
        public const string ClearPartName = "PART_Clear";

        // The clear button. It exists only after ShowsClear was set, so a field without it has the same parts as before.
        private Button? clear;

        // For the tests: the clear button, or null.
        internal Button? ClearButton => clear;

        private void UpdateClear()
        {
            if (PART_Search == null)
                return;

            PART_Search.Padding = ShowsClear ? TextPaddingWithClear : TextPadding;
            if (ShowsClear && clear == null)
                clear = CreateClear();

            if (clear != null)
            {
                clear.IsVisible = ShowsClear && !string.IsNullOrEmpty(Text);
                clear.IsEnabled = !IsReadOnly;
            }
        }

        // The button is a quiet icon button: it has no fill. It takes no focus, and is disabled while the field is read only.
        private Button CreateClear()
        {
            var button = new Boolean.ButtonIcon
            {
                IconKey = "Close",
                Name = ClearPartName,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Focusable = false,
            };
            button.Classes.Add("quiet");
            ToolTip.SetTip(button, "Clear");
            button.Click += OnClearClick;
            PART_Panel.Children.Add(button);
            return button;
        }

        // The clear button empties the field, and the field keeps the focus.
        private void OnClearClick(object? sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;

            Text = string.Empty;
            FocusInput();
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<SearchBox, string>(nameof(Text), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<string> WatermarkProperty =
            AvaloniaProperty.Register<SearchBox, string>(nameof(Watermark), "Search");

        public string Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        // True: the field keeps its text and takes no typing.
        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<SearchBox, bool>(nameof(IsReadOnly));

        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        // Gives the keyboard focus to the field itself.
        public void FocusInput() => PART_Search.Focus();

        public void FocusWhenShown() => Dispatcher.UIThread.Post(FocusInput, DispatcherPriority.Input);

        // Selects the whole text of the field, so the next typing replaces it.
        public void SelectAll() => PART_Search.SelectAll();
    }
}
