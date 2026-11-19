// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Compositing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public partial class MainMenu : ObservableObject
    {
        public MainMenu(Project project)
        {
            Project = project;

            AddCompositionCommand = new RelayCommand(AddComposition);
            AddLayerCommand = new RelayCommand(AddLayer);
            RemoveCompositionCommand = new RelayCommand(RemoveComposition);
            DuplicateCompositionCommand = new RelayCommand(DuplicateComposition);
            RemoveLayerCommand = new RelayCommand(RemoveLayer);
            DuplicateLayerCommand = new RelayCommand(DuplicateLayer);
        }

        public Project Project { get; set; }

        public ICommand AddCompositionCommand { get; }
        public ICommand AddLayerCommand { get; }
        public ICommand RemoveCompositionCommand { get; }
        public ICommand DuplicateCompositionCommand { get; }
        public ICommand RemoveLayerCommand { get; }
        public ICommand DuplicateLayerCommand { get; }

        public void AddLayer()
        {
            // Mirrors the layer manager view's AddItemCommand, CommandParameter typeof(Layer) button.
            var layerManager = (Project.CompositionManager.SelectedItem as Composition)?.LayerManager;
            layerManager?.AddItemCommand.Execute(typeof(Layer));
        }

        public void AddComposition()
        {
            // Mirrors the Composition tab's New Composition button, CommandParameter typeof(Composition).
            Project.CompositionManager.AddItemCommand.Execute(typeof(Composition));
        }

        private void RemoveComposition()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            Project.CompositionManager.RemoveItem(composition);
        }

        private void DuplicateComposition()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            Project.CompositionManager.DuplicateItem(composition);
        }

        private void RemoveLayer()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            if (composition.LayerManager.SelectedItem is not Layer layer) return;
            composition.LayerManager.RemoveItem(layer);
        }

        private void DuplicateLayer()
        {
            if (Project.CompositionManager.SelectedItem is not Composition composition) return;
            if (composition.LayerManager.SelectedItem is not Layer layer) return;
            composition.LayerManager.DuplicateItem(layer);
        }
    }
}
