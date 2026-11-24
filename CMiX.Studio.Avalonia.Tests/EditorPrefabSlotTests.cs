// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Camera = CMiX.Core.Rendering.Cameras.Camera;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    public class EditorPrefabSlotTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static Layer LayerWithACamera()
        {
            var layer = (Layer)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            layer.CameraManager.AddItem(typeof(Camera));
            return layer;
        }

        private void Show(Control view)
        {
            var window = windows.Add(new Window { Content = view, Width = 400, Height = 900 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
        }

        [AvaloniaFact]
        public void EditingPanel_ItemSelected_ShowsThePanelWithTheItem()
        {
            var layer = LayerWithACamera();
            var panel = new Border();
            var editor = new EditorPrefabSlot { DataContext = layer.CameraManager, ItemsSource = layer.CameraManager.ControlRepository.Cameras, EditingPanel = panel };
            Show(editor);

            var camera = layer.CameraManager.ManagerData.Items[0];
            layer.CameraManager.SelectedItem = camera;
            Dispatcher.UIThread.RunJobs();

            Assert.True(panel.IsEffectivelyVisible);
            Assert.Same(camera, panel.DataContext);
        }

        [AvaloniaFact]
        public void Layout_CameraTab_KeepsTheListMarginAndTheEditorPosition()
        {
            var layer = LayerWithACamera();
            layer.CameraManager.SelectedItem = layer.CameraManager.ManagerData.Items[0];
            var tab = new TabLayerCamera { DataContext = layer };
            Show(tab);

            var list = tab.GetVisualDescendants().OfType<ListPrefabSlot>().Single();
            var cameraEditor = tab.GetVisualDescendants().OfType<Views.Camera>().Single();

            Assert.Equal(new Thickness(8), list.Margin);
            Assert.Equal(new Point(8, 8), list.TranslatePoint(new Point(0, 0), tab));
            Assert.Equal(new Size(376, 120), list.Bounds.Size);
            Assert.Equal(new Point(0, 136), cameraEditor.TranslatePoint(new Point(0, 0), tab));
            Assert.Equal(new Size(392, 764), cameraEditor.Bounds.Size);
        }
    }
}
