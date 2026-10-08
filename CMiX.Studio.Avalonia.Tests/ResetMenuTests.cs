using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A value editor shows a Reset menu only when it can reset. The menu must always work.
    public class ResetMenuTests
    {
        // Editors that must not have a reset: (view, caption).
        private static readonly HashSet<(string View, string Caption)> NoReset = new()
        {
            ("MasterBeatControl", "BPM"),
            ("Composition", "Output Mapping"),      // picks an item, not a value
        };

        // Views that show a part of another control and are not named after a Core type.
        private static readonly Dictionary<string, string> PartViews = new()
        {
            ["MeshSphere"] = "Mesh", ["MeshCone"] = "Mesh", ["MeshCylinder"] = "Mesh", ["MeshTorus"] = "Mesh", ["MeshPlane"] = "Mesh",
            ["LightPoint"] = "LightSettings", ["LightSpot"] = "LightSettings", ["LightDirectional"] = "LightSettings",
            ["Text3D"] = "Text3DSettings",
            ["MasterBeatControl"] = "MasterBeat",
        };

        private static string Caption(Control c) =>
            c.GetType().GetProperty("Caption")?.GetValue(c)?.ToString() ?? (c as ContentControl)?.Content?.ToString() ?? "";

        private static bool HasResetItem(Control c) =>
            c.ContextMenu?.Items.OfType<MenuItem>().Any(i => Equals(i.Header, "Reset")) == true;

        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 600, Height = 900 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        // A collapsed expander does not build its content. A nested expander shows up only after its parent opens.
        private static void ExpandAll(Control view)
        {
            for (var pass = 0; pass < 5; pass++)
            {
                var closed = view.GetVisualDescendants().OfType<Expander>().Where(e => !e.IsExpanded).ToList();
                if (closed.Count == 0) return;

                foreach (var expander in closed)
                    expander.IsExpanded = true;
                Dispatcher.UIThread.RunJobs();
            }
        }

        // Hosts the view of every control that has a model, and runs the check on each view.
        private static int ForEachView(Action<string, Control> check)
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var coreAssembly = typeof(ControlFactory).Assembly;
            var viewsAssembly = typeof(ViewsAssemblyMarker).Assembly;

            var pairs = coreAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IControl).IsAssignableFrom(t))
                .Where(t => t.Assembly.GetType(t.FullName + "Model") != null)
                .Select(t => (View: t.Name, Type: t))
                .ToList();
            foreach (var (view, core) in PartViews)
                pairs.Add((view, coreAssembly.GetTypes().First(t => t.Name == core && t.IsClass)));

            var viewsChecked = 0;

            foreach (var (viewName, type) in pairs)
            {
                var viewType = viewsAssembly.GetType($"CMiX.Studio.Avalonia.Views.{viewName}");
                if (viewType == null || viewType.GetConstructor(Type.EmptyTypes) == null) continue;

                var control = type == typeof(MasterBeat)
                    ? provider.GetRequiredService<MasterBeat>()
                    : factory.Create(type);

                var view = (Control)Activator.CreateInstance(viewType)!;
                view.DataContext = control;
                var window = Host(view);
                viewsChecked++;
                ExpandAll(view);
                check(viewName, view);
                window.Close();
            }

            return viewsChecked;
        }

        [AvaloniaFact]
        public void EveryEditorInEveryView_HasAWorkingResetMenu_ExceptTheListedOnes()
        {
            var dead = new List<string>();
            var missing = new List<string>();

            var viewsChecked = ForEachView((viewName, view) =>
            {
                foreach (var editor in view.GetVisualDescendants().OfType<Control>().Where(c => c is CMiXSlider || c is DragValue || c is CaptionedToggleButton || c is CMiXToggleButton || c is CaptionedComboBox))
                {
                    editor.ApplyTemplate();     // a hidden editor builds its menu only when its template applies
                    if (!HasResetItem(editor) && !NoReset.Contains((viewName, Caption(editor))))
                        missing.Add($"{viewName}: '{Caption(editor)}' has no Reset menu");
                }

                foreach (var owner in view.GetVisualDescendants().OfType<Control>().Where(c => c.ContextMenu != null).ToList())
                {
                    var menu = owner.ContextMenu!;
                    menu.Open(owner);
                    Dispatcher.UIThread.RunJobs();
                    foreach (var item in menu.Items.OfType<MenuItem>().Where(i => Equals(i.Header, "Reset")))
                    {
                        if (item.Command == null)
                            dead.Add($"{viewName}: '{Caption(owner)}' ({owner.GetType().Name}) has a Reset menu without a command");
                    }
                    menu.Close();
                }
            });

            Assert.True(viewsChecked > 50, $"Only {viewsChecked} views were checked.");
            Assert.True(dead.Count == 0, string.Join(Environment.NewLine, dead));
            Assert.True(missing.Count == 0, string.Join(Environment.NewLine, missing));
        }

        // An integer box needs Value="{Binding Value}" next to its DataContext. Without it, the box and the model do not meet.
        [AvaloniaFact]
        public void EveryIntegerBoxInEveryView_FollowsItsModelValueBothWays()
        {
            var broken = new List<string>();
            var boxes = 0;

            ForEachView((viewName, view) =>
            {
                foreach (var box in view.GetVisualDescendants().OfType<IntegerValue>())
                {
                    if (box.DataContext is not GenericValue<int> value) continue;
                    boxes++;

                    value.Value += 7;
                    Dispatcher.UIThread.RunJobs();
                    if (box.Value != value.Value)
                        broken.Add($"{viewName}: '{box.Caption}' does not show its model value");

                    box.Value += 3;
                    Dispatcher.UIThread.RunJobs();
                    if (box.Value != value.Value)
                        broken.Add($"{viewName}: '{box.Caption}' does not change its model value");
                }
            });

            Assert.True(boxes > 10, $"Only {boxes} integer boxes were checked.");
            Assert.True(broken.Count == 0, string.Join(Environment.NewLine, broken));
        }

        [AvaloniaFact]
        public void ColorPicker_HasNoResetMenu()
        {
            var window = Host(new StandardColorPicker());

            var withReset = window.GetVisualDescendants().OfType<Control>().Where(HasResetItem).ToList();

            Assert.Empty(withReset);
        }

        [AvaloniaFact]
        public void Slider_WithoutResetCommand_HasNoMenu()
        {
            var slider = new CMiXSlider();
            Host(slider);

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void Slider_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var slider = new CMiXSlider { ResetCommand = new RelayCommand(() => ran = true) };
            Host(slider);

            var reset = slider.ContextMenu!.Items.OfType<MenuItem>().Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Slider_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var slider = new CMiXSlider { ResetCommand = new RelayCommand(() => { }) };
            Host(slider);
            Assert.NotNull(slider.ContextMenu);

            slider.ResetCommand = null!;

            Assert.Null(slider.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithoutResetCommand_HasNoMenu()
        {
            var drag = new DragValue();
            Host(drag);

            Assert.Null(drag.ContextMenu);
        }

        [AvaloniaFact]
        public void DragValue_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var drag = new DragValue { ResetCommand = new RelayCommand(() => ran = true) };
            Host(drag);

            var reset = drag.ContextMenu!.Items.OfType<MenuItem>().Single();
            reset.Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void DragValue_KeepsAMenuSetInXaml()
        {
            var own = new ContextMenu();
            var drag = new DragValue { ContextMenu = own, ResetCommand = new RelayCommand(() => { }) };
            Host(drag);

            Assert.Same(own, drag.ContextMenu);
        }
    }
}
