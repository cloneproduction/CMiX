// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.Services
{
    // Moves the keyboard focus between the panels of the main window (Window.NextPanel, Window.PreviousPanel, Window.FocusPanel1 to 4)
    // and to the tree (Tree.Focus). The panels are the visible side panels (PanelSide), from left to right. Each panel keeps its
    // last focused control. The focus goes back to that control, else to the first control of the panel that can take the focus.
    public sealed class PanelFocusCycle
    {
        private readonly ProjectTree tree;
        private readonly Dictionary<PanelSide, Control> lastFocused = new();
        private TopLevel? window;

        public PanelFocusCycle(ProjectTree tree)
        {
            this.tree = tree;
            NextPanelCommand = new RelayCommand(() => MoveBy(1));
            PreviousPanelCommand = new RelayCommand(() => MoveBy(-1));
            FocusPanel1Command = new RelayCommand(() => FocusPanelAt(0));
            FocusPanel2Command = new RelayCommand(() => FocusPanelAt(1));
            FocusPanel3Command = new RelayCommand(() => FocusPanelAt(2));
            FocusPanel4Command = new RelayCommand(() => FocusPanelAt(3));
            FocusTreeCommand = new RelayCommand(tree.FocusTree);
        }

        public IRelayCommand NextPanelCommand { get; }
        public IRelayCommand PreviousPanelCommand { get; }

        // A panel that is not there does nothing.
        public IRelayCommand FocusPanel1Command { get; }
        public IRelayCommand FocusPanel2Command { get; }
        public IRelayCommand FocusPanel3Command { get; }
        public IRelayCommand FocusPanel4Command { get; }

        // The focus goes to the cursor row of the tree, in the panel that shows the tree (see ProjectTree.FocusTree).
        public IRelayCommand FocusTreeCommand { get; }

        // The main window calls this once. The cycle then records the focused control of each panel, and Escape goes back to the tree.
        public void Attach(TopLevel mainWindow)
        {
            window = mainWindow;
            mainWindow.AddHandler(InputElement.GotFocusEvent, OnGotFocus, RoutingStrategies.Bubble, handledEventsToo: true);
            mainWindow.AddHandler(InputElement.PointerPressedEvent, (_, e) => lastPointer = e.Pointer, RoutingStrategies.Tunnel, handledEventsToo: true);
            mainWindow.AddHandler(InputElement.KeyDownEvent, OnKeyDownTunnel, RoutingStrategies.Tunnel, handledEventsToo: true);
            mainWindow.AddHandler(InputElement.KeyDownEvent, OnKeyDownBubble, RoutingStrategies.Bubble);
        }

        // The pointer of the last press. While it has a capture, a drag runs.
        private IPointer? lastPointer;

        // True when the Escape that goes down now started where no editor takes it. The state is read before the controls see the key:
        // an editor can end its edit on Escape and not mark the key as handled.
        private bool escapeToTree;

        private void OnKeyDownTunnel(object? sender, KeyEventArgs e) =>
            escapeToTree = e.Key == Key.Escape && e.KeyModifiers == KeyModifiers.None && !e.Handled && CanGoBackToTree();

        // Escape that no control handled puts the focus on the cursor row of the tree. Escape backs out one level only: a text box,
        // a popup, a drag and the tree itself keep their Escape.
        private void OnKeyDownBubble(object? sender, KeyEventArgs e)
        {
            var goBack = escapeToTree;
            escapeToTree = false;
            if (!goBack || e.Key != Key.Escape)
                return;

            tree.FocusTree();
            e.Handled = true;
        }

        private bool CanGoBackToTree()
        {
            if (lastPointer?.Captured != null)
                return false;

            if (window?.FocusManager?.GetFocusedElement() is not Visual focused || ReferenceEquals(focused, window))
                return true;

            return ReferenceEquals(focused.GetVisualRoot(), window)
                && focused is not TextBox && focused.FindAncestorOfType<TextBox>() == null
                && focused is not ListTreeProject && focused.FindAncestorOfType<ListTreeProject>() == null;
        }

        // The visible side panels, from left to right.
        public IReadOnlyList<PanelSide> Panels()
        {
            if (window is not { } top)
                return new List<PanelSide>();

            return top.GetVisualDescendants().OfType<PanelSide>()
                .Where(panel => panel.IsEffectivelyVisible)
                .OrderBy(panel => panel.TranslatePoint(default, top)?.X ?? 0)
                .ToList();
        }

        private void OnGotFocus(object? sender, GotFocusEventArgs e)
        {
            if (e.Source is Control control && PanelOf(control) is { } panel)
                lastFocused[panel] = control;
        }

        private static PanelSide? PanelOf(Visual visual) =>
            visual as PanelSide ?? visual.FindAncestorOfType<PanelSide>();

        private PanelSide? FocusedPanel() =>
            window?.FocusManager?.GetFocusedElement() is Visual focused ? PanelOf(focused) : null;

        // With no panel focused, the next panel is the first panel and the previous panel is the last panel.
        private void MoveBy(int step)
        {
            var panels = Panels();
            if (panels.Count == 0)
                return;

            var current = FocusedPanel() is { } focused ? IndexOf(panels, focused) : -1;
            var next = current < 0
                ? (step > 0 ? 0 : panels.Count - 1)
                : (current + step + panels.Count) % panels.Count;
            FocusPanel(panels[next]);
        }

        private static int IndexOf(IReadOnlyList<PanelSide> panels, PanelSide panel)
        {
            for (var i = 0; i < panels.Count; i++)
                if (ReferenceEquals(panels[i], panel))
                    return i;

            return -1;
        }

        private void FocusPanelAt(int index)
        {
            var panels = Panels();
            if (index < panels.Count)
                FocusPanel(panels[index]);
        }

        public void FocusPanel(PanelSide panel)
        {
            if (lastFocused.TryGetValue(panel, out var last) && CanFocus(last) && panel.IsVisualAncestorOf(last) && last.Focus(NavigationMethod.Tab))
                return;

            FocusFirst(panel);
        }

        // The first control of the content of the panel, else of its bar. In a panel of the tree, the focus then goes to the cursor row.
        private void FocusFirst(PanelSide panel)
        {
            Visual content = panel.FindControl<ContentControl>("content") is { } found ? found : panel;
            var first = FirstFocusable(content) ?? FirstFocusable(panel);
            if (first == null || !first.Focus(NavigationMethod.Tab))
                return;

            if (panel.GetVisualDescendants().OfType<ListTreeProject>().Any())
                tree.FocusTree();
        }

        private static InputElement? FirstFocusable(Visual root) =>
            root.GetVisualDescendants().OfType<InputElement>().FirstOrDefault(CanFocus);

        private static bool CanFocus(InputElement element) =>
            element.Focusable && element.IsEffectivelyEnabled && element.IsEffectivelyVisible && element.GetVisualRoot() != null;
    }
}
