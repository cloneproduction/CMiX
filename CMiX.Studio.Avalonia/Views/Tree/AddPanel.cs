// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;

namespace CMiX.Studio.Avalonia.Views
{
    // The panel that adds something under a node of the tree: a card with a search field and the groups of what the node can take
    // (see ProjectTree.GetAddGroups). The row menu, the toolbar and the shortcut all open this one panel.
    // The panel does not know where it was opened from. The caller gives an anchor and a side. The panel puts itself on that side,
    // and its tip points at the anchor.
    // On the right or the left of the anchor, the search field lines up with the middle of the anchor, so the pointer moves only
    // a little to reach the search field and the first rows. Above or below the anchor, the tip is on the middle of the anchor.
    // A pick closes the panel and then runs the add, so the lists change after the popup is gone.
    public sealed class AddPanel
    {
        // Under the anchor, the card starts this far before the anchor.
        private const double BelowOverhang = 8;

        // The space between the header and the list inside the card (see the StackPanel).
        private const double Spacing = 4;

        private readonly Flyout flyout = new() { ShowMode = FlyoutShowMode.Standard };
        private readonly PopupAnchoring anchoring;
        private readonly PopupCard card = new();
        private readonly TextBlock title = new() { VerticalAlignment = VerticalAlignment.Center };
        private readonly GroupedSearchList list = new() { Collapsible = false };
        private readonly Dictionary<TreeCategory, IBrush?> brushes = new();
        private readonly Converters.TreeToneConverter tone = new();
        private bool closedByCode;

        public AddPanel()
        {
            var content = new StackPanel { Spacing = Spacing };
            content.Children.Add(title);
            content.Children.Add(list);
            card.Content = content;
            flyout.Content = card;

            anchoring = PopupAnchoring.Use(flyout);
            anchoring.Placed += result => card.Place(result);

            flyout.Closed += OnFlyoutClosed;
            list.Picked += OnPicked;
            list.Cancelled += (_, _) => Close();
        }

        public bool IsOpen => flyout.IsOpen;

        // Raised when the panel has closed, after a pick, a click away or Escape.
        public event EventHandler? Closed
        {
            add => flyout.Closed += value;
            remove => flyout.Closed -= value;
        }

        // Raised when the user picks a row, before the panel closes and the add runs.
        public event EventHandler? Picked;

        // Raised when the panel closes by itself: a click away, the loss of the focus. It is not raised when the code closes the panel
        // (Close), when Escape closes it, or when a pick closes it.
        public event EventHandler? Dismissed;

        // For the tests: the rule of the place, the list and the card.
        internal PopupAnchoring Anchoring => anchoring;
        internal GroupedSearchList List => list;
        internal PopupCard Card => card;
        internal TextBlock Title => title;

        // Opens the panel on one side of the anchor. The header is a line of text above the search field, such as "Add to Layer 02".
        // The pass through element keeps receiving the pointer while the panel is open, when the popups share the window with the content (the
        // overlay mode): the menu that opened the panel stays usable. The panel shows only what the node can take. Nothing opens when there is nothing to add.
        public void Open(Control anchor, PopupSide side, IReadOnlyList<TreeAddGroup> groups, string? header = null, Rect? anchorRect = null,
                         IInputElement? passThrough = null, bool showsTip = false)
        {
            if (!Prepare(anchor, groups, header))
                return;

            this.showsTip = showsTip;

            var rowHeight = Number(anchor, "ControlHeightMD", 24);
            anchoring.Gap = DefaultGap;
            anchoring.Side = side;
            anchoring.Along = null;
            anchoring.Anchor = anchorRect;
            anchoring.AlignOffset = side is PopupSide.Left or PopupSide.Right
                ? SearchCenter(anchor, rowHeight)
                : anchor.Bounds.Width / 2 + BelowOverhang;

            Show(anchor, passThrough);
        }

        // Opens the panel over a point of a control, with the middle of the search field under the point, so the user can type at once and
        // the pointer has not moved. The point is in the coordinates of the control.
        public void OpenAtPoint(Control reference, Point point, IReadOnlyList<TreeAddGroup> groups, string? header = null)
        {
            showsTip = false;
            if (!Prepare(reference, groups, header) || TopLevel.GetTopLevel(reference) is not { } top || reference.TranslatePoint(point, top) is not { } inWindow)
                return;

            var rowHeight = Number(reference, "ControlHeightMD", 24);
            var padding = PaddingOf(reference);

            // The card hangs below the point by a negative gap: its top is above the point by the distance to the middle of the field.
            anchoring.Side = PopupSide.Bottom;
            anchoring.Gap = -SearchCenter(reference, rowHeight);
            anchoring.Anchor = new Rect(inWindow, new Size(0, 0));
            anchoring.Along = inWindow.X;
            anchoring.AlignOffset = padding.Left + list.Width / 2;

            Show(reference, null);
        }

        private const double DefaultGap = 8;

        // Fills the card. False when there is nothing to show.
        private bool Prepare(Control scope, IReadOnlyList<TreeAddGroup> groups, string? header)
        {
            groups = Available(groups);
            if (groups.Count == 0)
                return false;

            Close();

            var rowHeight = Number(scope, "ControlHeightMD", 24);
            title.Text = header;
            title.IsVisible = !string.IsNullOrEmpty(header);
            title.Height = rowHeight;
            list.Width = Number(scope, "AddPanelWidth", 200);
            list.KindBrush = kind => BrushOf(scope, kind);
            list.Groups = groups.Select(ToPicker).ToList();
            return true;
        }

        // Whether the card points at its anchor with a tip: a panel beside a row does, a panel at the pointer does not.
        private bool showsTip;

        private void Show(Control anchor, IInputElement? passThrough)
        {
            card.ShowsTip = showsTip;
            if (anchor.TryFindResource("FlyoutPresenterCard", out var theme) && theme is ControlTheme presenterTheme)
                flyout.FlyoutPresenterTheme = presenterTheme;

            flyout.OverlayInputPassThroughElement = passThrough;
            flyout.ShowAt(anchor);
        }

        // Closes the panel from the code. The panel raises Closed but not Dismissed.
        public void Close()
        {
            if (!flyout.IsOpen)
                return;

            closedByCode = true;
            flyout.Hide();
        }

        private void OnFlyoutClosed(object? sender, EventArgs e)
        {
            var byCode = closedByCode;
            closedByCode = false;
            if (!byCode)
                Dismissed?.Invoke(this, EventArgs.Empty);
        }

        // The distance from the top of the card to the middle of the search field: the padding, the header and its space.
        private static Thickness PaddingOf(Control scope) =>
            scope.TryFindResource("PopupCardPadding", out var value) && value is Thickness thickness ? thickness : new Thickness(10, 8);

        private double SearchCenter(Control scope, double rowHeight)
        {
            var padding = PaddingOf(scope);
            var header = title.IsVisible ? rowHeight + Spacing : 0;
            return padding.Top + header + rowHeight / 2;
        }

        private void OnPicked(object? sender, PickerPickedEventArgs e)
        {
            var add = e.Item.Value as TreeAdd;
            Picked?.Invoke(this, EventArgs.Empty);
            Close();

            // Posted so the popup is gone before the lists change.
            Dispatcher.UIThread.Post(() => add?.Execute?.Invoke());
        }

        private static PickerGroup ToPicker(TreeAddGroup group) =>
            new(group.Header, group.Items.Select(item => new PickerItem(item.Header, item, item.Category)).ToList(), group.Category);

        // The entries that the node cannot take are not in the panel, and a group with no entry left is not in it either.
        private static IReadOnlyList<TreeAddGroup> Available(IReadOnlyList<TreeAddGroup> groups) =>
            groups.Select(group => group with { Items = group.Items.Where(item => item.Enabled).ToList() })
                  .Where(group => group.Items.Count > 0)
                  .ToList();

        // The square of a kind in the list: the same tone as the kind squares of a collapsed row of the tree (TreeTone Kind, see
        // KindCountBadges), from the one palette (CategoryPalette), so the two never drift apart.
        private IBrush? BrushOf(Control scope, object? kind)
        {
            if (kind is not TreeCategory category)
                return null;

            if (!brushes.TryGetValue(category, out var brush))
            {
                brush = tone.Convert(new KindCount(category, 0), typeof(IBrush), nameof(ToneRole.Kind), System.Globalization.CultureInfo.InvariantCulture) as IBrush;
                brushes[category] = brush;
            }

            return brush;
        }

        private static double Number(Control scope, string key, double fallback) =>
            scope.TryFindResource(key, out var value) && value is double number ? number : fallback;
    }
}
