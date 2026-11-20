// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Threading;

namespace CMiX.UI.Controls.Parts
{
    // Gives a ComboBox a search field at the top of its drop-down. The theme of the ComboBox holds the field (a TextBoxSearch named
    // PART_ComboSearch) and shows it when IsSearchable is true. The field filters the rows by their text, in any case. Down and Up move a highlight
    // over the rows that are left, and Enter picks the highlighted row. The highlight starts on the selected row, or on the first row that is left.
    // The field is empty each time the drop-down opens, and it has the focus.
    // The rows are not virtualized, so every row exists and the filter reaches all of them. Use it for a list of some tens of rows.
    public static class ComboBoxSearch
    {
        public const string PartName = "PART_ComboSearch";

        public static readonly AttachedProperty<bool> IsSearchableProperty =
            AvaloniaProperty.RegisterAttached<ComboBox, bool>("IsSearchable", typeof(ComboBoxSearch), false);

        public static bool GetIsSearchable(ComboBox comboBox) => comboBox.GetValue(IsSearchableProperty);
        public static void SetIsSearchable(ComboBox comboBox, bool value) => comboBox.SetValue(IsSearchableProperty, value);

        private static readonly ConditionalWeakTable<ComboBox, Search> searches = new();

        static ComboBoxSearch()
        {
            IsSearchableProperty.Changed.AddClassHandler<ComboBox>((comboBox, e) =>
            {
                if (e.NewValue is true)
                {
                    if (!searches.TryGetValue(comboBox, out _))
                        searches.Add(comboBox, new Search(comboBox));
                }
                else if (searches.TryGetValue(comboBox, out var search))
                {
                    search.Release();
                    searches.Remove(comboBox);
                }
            });
        }

        // The rows that the filter keeps for a text: the text of an item is its ToString, as the drop-down shows it.
        public static bool Matches(object? item, string? text) =>
            string.IsNullOrEmpty(text) || (item?.ToString() ?? string.Empty).Contains(text, StringComparison.CurrentCultureIgnoreCase);

        private sealed class Search
        {
            private readonly ComboBox comboBox;
            private readonly ITemplate<Panel?>? panelBefore;
            private TextBoxSearch? box;
            private int highlighted = -1;

            public Search(ComboBox comboBox)
            {
                this.comboBox = comboBox;
                panelBefore = comboBox.ItemsPanel;
                comboBox.ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel());
                comboBox.TemplateApplied += OnTemplateApplied;
                comboBox.DropDownOpened += OnOpened;
            }

            public void Release()
            {
                comboBox.TemplateApplied -= OnTemplateApplied;
                comboBox.DropDownOpened -= OnOpened;
                Detach();
                comboBox.ItemsPanel = panelBefore!;
            }

            private void OnTemplateApplied(object? sender, Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
            {
                Detach();
                box = e.NameScope.Find<TextBoxSearch>(PartName);
                if (box == null)
                    return;

                box.PropertyChanged += OnBoxChanged;
                box.KeyDown += OnBoxKeyDown;
            }

            private void Detach()
            {
                if (box == null)
                    return;

                box.PropertyChanged -= OnBoxChanged;
                box.KeyDown -= OnBoxKeyDown;
                box = null;
            }

            private void OnOpened(object? sender, EventArgs e)
            {
                if (box == null)
                    return;

                box.Text = string.Empty;
                Filter();
                Dispatcher.UIThread.Post(() => box?.FocusInput(), DispatcherPriority.Input);
            }

            private void OnBoxChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                if (e.Property == TextBoxSearch.TextProperty)
                    Filter();
            }

            // The rows that show, as indexes of the items.
            private List<int> Visible() =>
                Enumerable.Range(0, comboBox.ItemCount).Where(index => comboBox.ContainerFromIndex(index)?.IsVisible == true).ToList();

            // A row that does not match is hidden. The rows are in the order of the items. The highlight goes to the first row that is left,
            // or to the selected row when the field is empty and the row shows.
            private void Filter()
            {
                var text = box?.Text;
                for (var index = 0; index < comboBox.ItemCount; index++)
                {
                    if (comboBox.ContainerFromIndex(index) is Control row)
                        row.IsVisible = Matches(comboBox.Items[index], text);
                }

                var visible = Visible();
                Highlight(string.IsNullOrEmpty(text) && visible.Contains(comboBox.SelectedIndex) ? comboBox.SelectedIndex : visible.FirstOrDefault(-1));
            }

            // Marks one row as the highlighted row. -1 marks none.
            private void Highlight(int index)
            {
                for (var i = 0; i < comboBox.ItemCount; i++)
                    comboBox.ContainerFromIndex(i)?.Classes.Set("highlighted", i == index);

                highlighted = index;
                if (index >= 0)
                    comboBox.ContainerFromIndex(index)?.BringIntoView();
            }

            // Down and Up move the highlight over the rows that show, Enter picks the highlighted row, Escape clears the text.
            private void OnBoxKeyDown(object? sender, KeyEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.Down:
                    case Key.Up:
                        var visible = Visible();
                        if (visible.Count > 0)
                        {
                            var at = visible.IndexOf(highlighted);
                            var next = e.Key == Key.Down ? Math.Min(at + 1, visible.Count - 1) : Math.Max(at - 1, 0);
                            Highlight(visible[Math.Max(next, 0)]);
                        }

                        e.Handled = true;
                        break;

                    case Key.Enter:
                        var pick = Visible().Contains(highlighted) ? highlighted : Visible().FirstOrDefault(-1);
                        if (pick >= 0)
                        {
                            comboBox.SelectedIndex = pick;
                            comboBox.IsDropDownOpen = false;
                        }

                        e.Handled = true;
                        break;

                    case Key.Escape when !string.IsNullOrEmpty(box?.Text):
                        box!.Text = string.Empty;
                        e.Handled = true;
                        break;
                }
            }
        }
    }
}
