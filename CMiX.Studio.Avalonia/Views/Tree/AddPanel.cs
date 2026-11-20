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
    // Beside the anchor, the search field lines up with the middle of the anchor, so the pointer moves only a little.
    public sealed class AddPanel
    {
        // Under the anchor, the card starts this far before the anchor.
        private const double BelowOverhang = 8;

        private const double HeaderToListSpacing = 4;

        private readonly Flyout flyout = new() { ShowMode = FlyoutShowMode.Standard };
        private readonly PopupAnchoring anchoring;
        private readonly PopupCard card = new();
        private readonly TextBlock title = new() { VerticalAlignment = VerticalAlignment.Center };
        private readonly ListGroupedSearch list = new() { Collapsible = false };
        private readonly Dictionary<TreeCategory, IBrush?> brushes = new();
        private readonly Converters.TreeToneConverter tone = new();
        private bool closedByCode;

        public AddPanel()
        {
            var content = new StackPanel { Spacing = HeaderToListSpacing };
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

        public event EventHandler? Closed
        {
            add => flyout.Closed += value;
            remove => flyout.Closed -= value;
        }

        public event EventHandler? Picked;

        // Not raised when the code, Escape or a pick closes the panel.
        public event EventHandler? Dismissed;

        // For the tests.
        internal PopupAnchoring Anchoring => anchoring;
        internal ListGroupedSearch List => list;
        internal PopupCard Card => card;
        internal TextBlock Title => title;

        // passThrough keeps the pointer on the menu that opened the panel, in the overlay mode of the popups.
        public void Open(Control anchor, PopupSide side, IReadOnlyList<TreeAddGroup> groups, string? header = null, Rect? anchorRect = null,
                         IInputElement? passThrough = null)
        {
            if (!Prepare(anchor, groups, header))
                return;

            showsTip = false;
            ShowBeside(anchor, side, anchorRect, passThrough);
        }

        public void OpenWithTip(Control anchor, PopupSide side, IReadOnlyList<TreeAddGroup> groups, string? header, Rect? anchorRect)
        {
            if (!Prepare(anchor, groups, header))
                return;

            showsTip = true;
            ShowBeside(anchor, side, anchorRect, null);
        }

        private void ShowBeside(Control anchor, PopupSide side, Rect? anchorRect, IInputElement? passThrough)
        {
            var rowHeight = TreeResources.Read<double>(anchor, "ControlHeightMD", 24);
            anchoring.Gap = DefaultGap;
            anchoring.Side = side;
            anchoring.Along = null;
            anchoring.Anchor = anchorRect;
            anchoring.AlignOffset = side is PopupSide.Left or PopupSide.Right
                ? SearchCenter(anchor, rowHeight)
                : anchor.Bounds.Width / 2 + BelowOverhang;

            Show(anchor, passThrough);
        }

        // The middle of the search field goes under the point, so the user can type at once.
        public void OpenAtPoint(Control reference, Point point, IReadOnlyList<TreeAddGroup> groups, string? header = null)
        {
            showsTip = false;
            if (!Prepare(reference, groups, header) || TopLevel.GetTopLevel(reference) is not { } top || reference.TranslatePoint(point, top) is not { } inWindow)
                return;

            var rowHeight = TreeResources.Read<double>(reference, "ControlHeightMD", 24);
            var padding = PaddingOf(reference);

            // A negative gap lifts the card, so the middle of the field is at the point.
            anchoring.Side = PopupSide.Bottom;
            anchoring.Gap = -SearchCenter(reference, rowHeight);
            anchoring.Anchor = new Rect(inWindow, new Size(0, 0));
            anchoring.Along = inWindow.X;
            anchoring.AlignOffset = padding.Left + list.Width / 2;

            Show(reference, null);
        }

        private const double DefaultGap = 8;

        private bool Prepare(Control scope, IReadOnlyList<TreeAddGroup> groups, string? header)
        {
            groups = Available(groups);
            if (groups.Count == 0)
                return false;

            Close();

            var rowHeight = TreeResources.Read<double>(scope, "ControlHeightMD", 24);
            title.Text = header;
            title.IsVisible = !string.IsNullOrEmpty(header);
            title.Height = rowHeight;
            list.Width = TreeResources.Read<double>(scope, "AddPanelWidth", 200);
            list.KindBrush = kind => BrushOf(scope, kind);
            list.Groups = groups.Select(ToPicker).ToList();
            return true;
        }

        private bool showsTip;

        private void Show(Control anchor, IInputElement? passThrough)
        {
            card.ShowsTip = showsTip;
            if (anchor.TryFindResource("FlyoutPresenterCard", out var theme) && theme is ControlTheme presenterTheme)
                flyout.FlyoutPresenterTheme = presenterTheme;

            flyout.OverlayInputPassThroughElement = passThrough;
            flyout.ShowAt(anchor);
        }

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

        private static Thickness PaddingOf(Control scope) =>
            TreeResources.Read(scope, "PopupCardPadding", new Thickness(10, 8));

        private double SearchCenter(Control scope, double rowHeight)
        {
            var padding = PaddingOf(scope);
            var header = title.IsVisible ? rowHeight + HeaderToListSpacing : 0;
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

        private static IReadOnlyList<TreeAddGroup> Available(IReadOnlyList<TreeAddGroup> groups) =>
            groups.Select(group => group with { Items = group.Items.Where(item => item.Enabled).ToList() })
                  .Where(group => group.Items.Count > 0)
                  .ToList();

        // The same palette as the kind squares of a collapsed row, so the two never drift apart.
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
    }
}
