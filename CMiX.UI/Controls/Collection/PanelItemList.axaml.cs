// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;

namespace CMiX.UI.Controls.Collection
{
    // A list of items with an add button under it. The application sets the ItemTemplate, usually with
    // a PanelItem. The add button opens a popup that shows AddContent. The list is reordered by the user
    // only when ReorderService is set.
    public partial class PanelItemList : UserControl
    {
        static PanelItemList()
        {
            LogicalPanelContent.Track<PanelItemList>(AddContentProperty);
        }

        public PanelItemList()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<PanelItemList, IEnumerable?>(nameof(ItemsSource));

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
            AvaloniaProperty.Register<PanelItemList, IDataTemplate?>(nameof(ItemTemplate));

        public IDataTemplate? ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        // Without a service the items cannot be reordered.
        public static readonly StyledProperty<IReorderService?> ReorderServiceProperty =
            AvaloniaProperty.Register<PanelItemList, IReorderService?>(nameof(ReorderService));

        public IReorderService? ReorderService
        {
            get => GetValue(ReorderServiceProperty);
            set => SetValue(ReorderServiceProperty, value);
        }

        public static readonly StyledProperty<string> AddCaptionProperty =
            AvaloniaProperty.Register<PanelItemList, string>(nameof(AddCaption), string.Empty);

        public string AddCaption
        {
            get => GetValue(AddCaptionProperty);
            set => SetValue(AddCaptionProperty, value);
        }

        public static readonly StyledProperty<Control?> AddContentProperty =
            AvaloniaProperty.Register<PanelItemList, Control?>(nameof(AddContent));

        public Control? AddContent
        {
            get => GetValue(AddContentProperty);
            set => SetValue(AddContentProperty, value);
        }

        // False: the add popup stays open when the pointer goes away (see TogglePopup.ClosesOnPointerAway).
        public static readonly StyledProperty<bool> AddClosesOnPointerAwayProperty =
            AvaloniaProperty.Register<PanelItemList, bool>(nameof(AddClosesOnPointerAway), true);

        public bool AddClosesOnPointerAway
        {
            get => GetValue(AddClosesOnPointerAwayProperty);
            set => SetValue(AddClosesOnPointerAwayProperty, value);
        }

        // The distance of the pointer from the popup and the toggle, in pixels, that closes the add popup.
        public static readonly StyledProperty<double> AddPointerAwayDistanceProperty =
            AvaloniaProperty.Register<PanelItemList, double>(nameof(AddPointerAwayDistance), 64);

        public double AddPointerAwayDistance
        {
            get => GetValue(AddPointerAwayDistanceProperty);
            set => SetValue(AddPointerAwayDistanceProperty, value);
        }
    }
}
