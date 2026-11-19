// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Data;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CMiX.UI.Controls.Collection
{
    // Rows to pick from, at most ComboBoxVisibleItems at once. The owner moves HighlightedItem.
    public partial class ItemPickerList : ItemPickerBase
    {
        static ItemPickerList()
        {
            HighlightedItemProperty.Changed.AddClassHandler<ItemPickerList>((list, _) => list.ShowHighlight());
        }

        public ItemPickerList()
        {
            InitializeComponent();
            PART_Items.ItemTemplate = new FuncDataTemplate<object>((item, _) => Row(item));
        }

        public static readonly StyledProperty<object?> HighlightedItemProperty =
            AvaloniaProperty.Register<ItemPickerList, object?>(nameof(HighlightedItem));

        public object? HighlightedItem
        {
            get => GetValue(HighlightedItemProperty);
            set => SetValue(HighlightedItemProperty, value);
        }

        private Button Row(object item)
        {
            var row = new Button { CommandParameter = item };

            if (this.TryFindResource("ButtonItem", ActualThemeVariant, out var theme) && theme is ControlTheme controlTheme)
                row.Theme = controlTheme;

            row.Bind(Button.CommandProperty, new Binding(nameof(ItemCommand)) { Source = this });
            row.Classes.Set("highlighted", ReferenceEquals(item, HighlightedItem));

            // The click runs the command first. Picked follows after it, so a host that closes its popup does not cut the command.
            row.Click += (_, _) => Dispatcher.UIThread.Post(RaisePicked);

            // Only a real move of the mouse counts. A row that scrolls under a still pointer does not take the highlight.
            row.PointerMoved += (_, _) => HighlightedItem = item;

            if (string.IsNullOrEmpty(IconPath))
            {
                if (string.IsNullOrEmpty(TextPath))
                    row.Content = item.ToString();
                else
                    row.Bind(ContentControl.ContentProperty, new Binding(TextPath) { Source = item });

                return row;
            }

            // The icon, then the text. The icon has the size of the icons of the buttons.
            var icon = new AppIcon { VerticalAlignment = VerticalAlignment.Center };
            icon.Bind(AppIcon.IconKeyProperty, new Binding(IconPath) { Source = item });
            if (this.TryFindResource("IconSizeXS", ActualThemeVariant, out var size) && size is double iconSize)
                icon.Size = iconSize;

            var text = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
            if (string.IsNullOrEmpty(TextPath))
                text.Text = item.ToString();
            else
                text.Bind(TextBlock.TextProperty, new Binding(TextPath) { Source = item });

            var spacing = this.TryFindResource("SpacingMD", ActualThemeVariant, out var gap) && gap is double gapSize ? gapSize : 8;
            row.Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = spacing, Children = { icon, text } };
            return row;
        }

        private void ShowHighlight()
        {
            Button? highlighted = null;

            foreach (var row in PART_Items.GetVisualDescendants().OfType<Button>())
            {
                var isHighlighted = HighlightedItem != null && ReferenceEquals(row.CommandParameter, HighlightedItem);
                row.Classes.Set("highlighted", isHighlighted);
                if (isHighlighted)
                    highlighted = row;
            }

            // The scroll moves after the layout, so a row that was just made has its place.
            if (highlighted != null)
                Dispatcher.UIThread.Post(() => highlighted.BringIntoView(), DispatcherPriority.Loaded);
        }
    }
}
