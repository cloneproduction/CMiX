// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class FrameMeshShapeTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private Control Show(Control view)
        {
            view.DataContext = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Mesh));
            windows.Add(new Window { Content = view, Width = 400, Height = 900 }).Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static StackPanel Frame(Control view) =>
            (StackPanel)view.GetVisualDescendants().OfType<ScaleOffset>().Single().GetVisualParent()!;

        [AvaloniaFact]
        public void Template_EveryShape_PutsScaleOffsetFirstAndTessellationLast()
        {
            var views = new Control[] { new MeshCone(), new MeshCylinder(), new MeshSphere(), new MeshTorus() };

            var frames = views.Select(view => Frame(Show(view))).ToList();

            Assert.All(frames, frame =>
            {
                Assert.IsType<ScaleOffset>(frame.Children.First());
                var tessellation = Assert.IsType<DragValueInteger>(frame.Children.Last());
                Assert.Equal("Tessellation", tessellation.Caption);
                Assert.Equal(8, tessellation.Margin.Top);
            });
        }
    }
}
