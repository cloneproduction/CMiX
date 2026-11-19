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
            Project = project;
        }

        public Project Project { get; }

        public event EventHandler? QuitRequested;

        [RelayCommand]
        private void Quit() => QuitRequested?.Invoke(this, EventArgs.Empty);

        [RelayCommand]
        private void AddLayer()
        {
            // Mirrors the layer manager view's AddItemCommand, CommandParameter typeof(Layer) button.
            var layerManager = (Project.CompositionManager.SelectedItem as Composition)?.LayerManager;
            layerManager?.AddItemCommand.Execute(typeof(Layer));
        }

        [RelayCommand]
        private void AddComposition()
        {
            // Mirrors the Composition tab's New Composition button, CommandParameter typeof(Composition).
            Project.CompositionManager.AddItemCommand.Execute(typeof(Composition));
        }

        [RelayCommand]
        private void RemoveComposition()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            Project.CompositionManager.RemoveItem(composition);
        }

        [RelayCommand]
        private void DuplicateComposition()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            Project.CompositionManager.DuplicateItem(composition);
        }

        [RelayCommand]
        private void RemoveLayer()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            if (composition.LayerManager.SelectedItem is not Layer layer) return;
            composition.LayerManager.RemoveItem(layer);
        }

        [RelayCommand]
        private void DuplicateLayer()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            if (composition.LayerManager.SelectedItem is not Layer layer) return;
            composition.LayerManager.DuplicateItem(layer);
        }
    }
}
