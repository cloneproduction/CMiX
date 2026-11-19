// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
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

        private Composition? SelectedComposition => _project.CompositionManager.SelectedItem as Composition;

        private PrefabManager? SelectedLayerManager => SelectedComposition?.LayerManager;

        [RelayCommand]
        private void AddLayer() => SelectedLayerManager?.AddItemCommand.Execute(typeof(Layer));

        [RelayCommand]
        private void AddComposition() => _project.CompositionManager.AddItemCommand.Execute(typeof(Composition));

        [RelayCommand]
        private void RemoveComposition()
        {
            if (SelectedComposition is { } composition)
                _project.CompositionManager.RemoveItem(composition);
        }

        [RelayCommand]
        private void DuplicateComposition()
        {
            if (SelectedComposition is { } composition)
                _project.CompositionManager.DuplicateItem(composition);
        }

        [RelayCommand]
        private void RemoveLayer()
        {
            if (SelectedLayerManager is { SelectedItem: Layer layer } layers)
                layers.RemoveItem(layer);
        }

        [RelayCommand]
        private void DuplicateLayer()
        {
            if (SelectedLayerManager is { SelectedItem: Layer layer } layers)
                layers.DuplicateItem(layer);
        }
    }
}
