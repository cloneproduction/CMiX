// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using System.Linq;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CMiX.UI.Behaviors;
using Avalonia.LogicalTree;
using CMiX.Studio.Avalonia.Notices;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        // The window takes Alt before Avalonia, so Alt alone does not open the main menu and the tree keys with Alt still run.
        static MainWindow()
        {
            static void TakeAlt(MainWindow _, KeyEventArgs e)
            {
                if (e.Key is Key.LeftAlt or Key.RightAlt)
                    e.Handled = true;
            }

            KeyDownEvent.AddClassHandler<MainWindow>(TakeAlt, RoutingStrategies.Tunnel);
            KeyUpEvent.AddClassHandler<MainWindow>(TakeAlt, RoutingStrategies.Tunnel);
            KeyDownEvent.AddClassHandler<MainWindow>((window, e) => window.OnKeyDownAfterTextBox(e), RoutingStrategies.Bubble);
        }

        // A class handler runs before the Tab navigation, so Ctrl+Tab in a text box moves the focus to the next panel.
        private void OnKeyDownAfterTextBox(KeyEventArgs e) => textGuard.RunPausedBinding(e);

        private readonly PanelFocusCycle? panelFocus;
        private readonly NotificationService? notifications;

        public MainWindow(PanelFocusCycle panelFocus, NotificationService notifications) : this()
        {
            this.panelFocus = panelFocus;
            this.notifications = notifications;
        }

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClosing;
            InstallToggleDrag();
            // A text box gets Ctrl+Z first. A key that it does not handle still undoes or redoes the project (see OnKeyDownAfterTextBox).
            textGuard = KeyBindingTextGuard.Install(this);
        }

        private readonly KeyBindingTextGuard textGuard;

        // The window opens on the right edge of the screen, as high as the working area. A program can turn this off.
        public bool DockOnOpen { get; set; } = true;

        protected override void OnOpened(EventArgs e)
        {
            panelFocus?.Attach(this);
            notifications?.Attach(panelNotification);
            if (DockOnOpen)
                WindowDock.DockRight(this);
            base.OnOpened(e);
        }

        // A drag over the visibility or the mask buttons sets them all to one state. The whole drag is one undo step.
        private void InstallToggleDrag()
        {
            var toggleDrag = new ToggleDragBehavior();
            toggleDrag.StrokeStarted += (_, _) => (DataContext as MainViewModel)?.BeginUndoCaptureCommand.Execute(null);
            toggleDrag.StrokeEnded += (_, _) => (DataContext as MainViewModel)?.EndUndoCaptureCommand.Execute(null);
            Interaction.GetBehaviors(this).Add(toggleDrag);
        }

        // The cross button, Alt+F4 and the close command all end here. With unsaved changes the close is
        // put off until the user has answered. The window then closes for good, without asking again.
        private async void OnClosing(object? sender, WindowClosingEventArgs e)
        {
            if (DataContext is not MainViewModel { ProjectFile.HasUnsavedChanges: true } viewModel)
                return;

            e.Cancel = true;

            if (!await viewModel.ProjectFile.AskToDiscardAsync())
                return;

            Closing -= OnClosing;
            Close();
        }

        private MainMenuViewModel? _menu;

        // The Quit row of the menu closes the window through OnClosing, so unsaved changes are asked about first.
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (_menu != null)
                _menu.QuitRequested -= OnQuitRequested;
            _menu = (DataContext as MainViewModel)?.MainMenu;
            if (_menu != null)
                _menu.QuitRequested += OnQuitRequested;
        }

        private void OnQuitRequested(object? sender, EventArgs e) => Close();

        private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.Source is ILogical source &&
                source.FindLogicalAncestorOfType<Windows.MainWindowController>(true) != null)
                return;

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                BeginMoveDrag(e);
        }
    }
}
