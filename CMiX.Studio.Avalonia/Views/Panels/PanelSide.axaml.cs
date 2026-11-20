// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CMiX.Studio.Avalonia.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.Views
{
    // A panel of the window with an icon button at the top, in which the user picks what the panel shows.
    public partial class PanelSide : UserControl
    {
        public static readonly StyledProperty<PanelChoice?> ChoiceProperty =
            AvaloniaProperty.Register<PanelSide, PanelChoice?>(nameof(Choice), defaultBindingMode: BindingMode.TwoWay);

        public PanelChoice? Choice
        {
            get => GetValue(ChoiceProperty);
            set => SetValue(ChoiceProperty, value);
        }

        public static readonly StyledProperty<IEnumerable?> ChoicesProperty =
            AvaloniaProperty.Register<PanelSide, IEnumerable?>(nameof(Choices));

        public IEnumerable? Choices
        {
            get => GetValue(ChoicesProperty);
            set => SetValue(ChoicesProperty, value);
        }

        public static readonly StyledProperty<MainViewModel?> ContextProperty =
            AvaloniaProperty.Register<PanelSide, MainViewModel?>(nameof(Context));

        // The view model that the views of the choices show.
        public MainViewModel? Context
        {
            get => GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        // The command of the rows of the list: it makes the picked choice the choice of the panel.
        public ICommand PickCommand { get; }

        public PanelSide()
        {
            PickCommand = new RelayCommand<PanelChoice>(choice => Choice = choice);
            InitializeComponent();
        }

        // A row was picked. The list closes.
        private void OnPicked(object? sender, EventArgs e) => pickerToggle.IsChecked = false;
    }
}
