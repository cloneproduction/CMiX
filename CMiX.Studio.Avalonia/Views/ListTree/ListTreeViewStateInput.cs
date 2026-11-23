// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.ListTree;

namespace CMiX.Studio.Avalonia.Views
{
    // The new tree pushes its pointer and its focus into ListTreeViewState on its own events and on the focus events of its window.
    internal sealed class ListTreeViewStateInput
    {
        private readonly Control owner;
        private readonly ListTreeFocus focus;
        private readonly ListTreeViewState state;

        private TopLevel? window;

        public ListTreeViewStateInput(Control owner, ListTreeFocus focus, ListTreeViewState state)
        {
            this.owner = owner;
            this.focus = focus;
            this.state = state;
        }

        public void Attach()
        {
            owner.PropertyChanged += OnOwnerChanged;
            AttachWindow(TopLevel.GetTopLevel(owner));
            Push();
        }

        public void Detach()
        {
            owner.PropertyChanged -= OnOwnerChanged;
            AttachWindow(null);
        }

        private void AttachWindow(TopLevel? attached)
        {
            window?.RemoveHandler(InputElement.GotFocusEvent, OnWindowFocusChanged);
            window?.RemoveHandler(InputElement.LostFocusEvent, OnWindowFocusChanged);
            window = attached;
            window?.AddHandler(InputElement.GotFocusEvent, OnWindowFocusChanged, RoutingStrategies.Bubble, handledEventsToo: true);
            window?.AddHandler(InputElement.LostFocusEvent, OnWindowFocusChanged, RoutingStrategies.Bubble, handledEventsToo: true);
        }

        private void OnOwnerChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == InputElement.IsPointerOverProperty || e.Property == InputElement.IsKeyboardFocusWithinProperty)
                Push();
        }

        private void OnWindowFocusChanged(object? sender, RoutedEventArgs e) => Push();

        private void Push()
        {
            state.IsPointerOverTree = owner.IsPointerOver;
            state.IsTreeFocused = owner.IsKeyboardFocusWithin;
            state.IsTextBoxFocused = focus.IsTextBoxFocused();
            state.IsFocusInOtherPanel = focus.IsFocusInOtherPanel();
            state.IsPanelFocused = focus.IsPanelFocused();
        }
    }
}
