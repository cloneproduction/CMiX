// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Where the keyboard focus is, seen from the new tree: in a text box, in its side panel, or in another panel.
    internal sealed class ListTreeFocus
    {
        private readonly Control owner;

        public ListTreeFocus(Control owner)
        {
            this.owner = owner;
        }

        public static bool IsInTextBox(object? source) => ListTreeRowClick.HasAncestor<TextBox>(source);

        public bool IsTextBoxFocused() => IsInTextBox(TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement());

        // The focus on the window itself, or on no control, is in no panel.
        public bool IsFocusInOtherPanel() => FocusedControl() is { } focused && !IsInPanel(focused);

        public bool IsPanelFocused() => FocusedControl() is { } focused && IsInPanel(focused);

        public IInputElement? FocusedControl() =>
            TopLevel.GetTopLevel(owner)?.FocusManager?.GetFocusedElement() is { } focused and not TopLevel ? focused : null;

        // The search field of the same tree, in the side panel of the view.
        public bool IsSearchFocused()
        {
            if (FocusedControl() is not Visual focused || focused.FindAncestorOfType<TextBoxSearchTree>(true) is not { } box
                || !ReferenceEquals(box.DataContext, owner.DataContext))
                return false;

            return owner.FindAncestorOfType<PanelSide>() is not { } host || host.IsVisualAncestorOf(box);
        }

        public bool IsInPanel(IInputElement focused)
        {
            Visual panel = owner.FindAncestorOfType<PanelSide>() is { } host ? host : owner;
            return focused is Visual visual && (ReferenceEquals(visual, panel) || panel.IsVisualAncestorOf(visual));
        }
    }
}
