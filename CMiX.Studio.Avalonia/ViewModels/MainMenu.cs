// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using Avalonia.Threading;
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
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer name is touched.
            Dispatcher.UIThread.Post(() =>
            {
                var layerManager = (Project.CompositionManager.SelectedItem as Composition)?.LayerManager;
                layerManager?.AddItemCommand.Execute(typeof(Layer));
            });
        }

        public void AddComposition()
        {
            // Mirrors the Composition tab's New Composition button, CommandParameter typeof(Composition).
            // Posted because the MenuItem still has its own flyout Popup open while this runs, and
            // mutating SelectedItem while the Outliner header sits behind that Popup left the name
            // display painted with the pre add frame until a manual refresh such as double clicking
            // into edit mode. Posting waits for the popup teardown to finish before the header is
            // touched, matching the DispatcherPost pattern already used for popup timing elsewhere.
            Dispatcher.UIThread.Post(() => Project.CompositionManager.AddItemCommand.Execute(typeof(Composition)));
        }

        private void RemoveComposition()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound composition list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                Project.CompositionManager.RemoveItem(composition);
            });
        }

        private void DuplicateComposition()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound composition list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                Project.CompositionManager.DuplicateItem(composition);
            });
        }

        private void RemoveLayer()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                if (composition.LayerManager.SelectedItem is not Layer layer) return;
                composition.LayerManager.RemoveItem(layer);
            });
        }

        private void DuplicateLayer()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                if (composition.LayerManager.SelectedItem is not Layer layer) return;
                composition.LayerManager.DuplicateItem(layer);
            });
        }
    }
}
