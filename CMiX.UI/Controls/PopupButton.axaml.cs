// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls
{
    public partial class PopupButton : UserControl
    {
        // Roots panel content in the logical tree so its deferred bindings apply; see LogicalPanelContent.
        static PopupButton()
        {
            LogicalPanelContent.Track<PopupButton>(PopupContentProperty);
        }

        public PopupButton()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<Control> PopupContentProperty =
            AvaloniaProperty.Register<PopupButton, Control>(nameof(PopupContent));
        public Control PopupContent
        {
            get => GetValue(PopupContentProperty);
            set => SetValue(PopupContentProperty, value);
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<PopupButton, string>(nameof(Caption), string.Empty);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        // False: the popup stays open when the pointer goes away (see TogglePopup.ClosesOnPointerAway).
        public static readonly StyledProperty<bool> ClosesOnPointerAwayProperty =
            AvaloniaProperty.Register<PopupButton, bool>(nameof(ClosesOnPointerAway), true);

        public bool ClosesOnPointerAway
        {
            get => GetValue(ClosesOnPointerAwayProperty);
            set => SetValue(ClosesOnPointerAwayProperty, value);
        }

        // The distance of the pointer from the popup and the toggle, in pixels, that closes the popup.
        public static readonly StyledProperty<double> PointerAwayDistanceProperty =
            AvaloniaProperty.Register<PopupButton, double>(nameof(PointerAwayDistance), 64);

        public double PointerAwayDistance
        {
            get => GetValue(PointerAwayDistanceProperty);
            set => SetValue(PointerAwayDistanceProperty, value);
        }
    }
}
