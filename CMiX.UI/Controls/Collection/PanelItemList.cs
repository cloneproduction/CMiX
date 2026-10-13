// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Collection
{
    // A list of items with an add button under it. The application sets the ItemTemplate, usually with
    // a PanelItem. The add button opens a popup that shows AddContent. For a list that the user can
    // reorder, set ListBoxReorder.Service on the list. Without it, the list is not reordered.
    public class PanelItemList : ItemsControl
    {
        static PanelItemList()
        {
            LogicalPanelContent.Track<PanelItemList>(AddContentProperty);
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
    }
}
