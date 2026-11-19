// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core.Compositing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public sealed partial class MainMenuViewModel : ObservableObject
    {
        public MainMenuViewModel(Project project)
        {
            _project = project;
        }

        private readonly Project _project;

        public event EventHandler? QuitRequested;

        [RelayCommand]
        private void Quit() => QuitRequested?.Invoke(this, EventArgs.Empty);

        [RelayCommand]
        private void AddLayer()
        {
            var layerManager = (_project.CompositionManager.SelectedItem as Composition)?.LayerManager;
            layerManager?.AddItemCommand.Execute(typeof(Layer));
        }

        [RelayCommand]
        private void AddComposition()
        {
            _project.CompositionManager.AddItemCommand.Execute(typeof(Composition));
        }

        [RelayCommand]
        private void RemoveComposition()
        {
            if (_project.CompositionManager.SelectedItem is not Composition composition) return;
            _project.CompositionManager.RemoveItem(composition);
        }

        [RelayCommand]
        private void DuplicateComposition()
        {
            if (_project.CompositionManager.SelectedItem is not Composition composition) return;
            _project.CompositionManager.DuplicateItem(composition);
        }

        [RelayCommand]
        private void RemoveLayer()
        {
            if (_project.CompositionManager.SelectedItem is not Composition composition) return;
            if (composition.LayerManager.SelectedItem is not Layer layer) return;
            composition.LayerManager.RemoveItem(layer);
        }

        [RelayCommand]
        private void DuplicateLayer()
        {
            if (_project.CompositionManager.SelectedItem is not Composition composition) return;
            if (composition.LayerManager.SelectedItem is not Layer layer) return;
            composition.LayerManager.DuplicateItem(layer);
        }
    }
}
