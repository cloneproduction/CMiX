using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Number;

namespace CMiX.UI.Gallery
{
    // Shows every kind of control in its states: idle, hover, pressed, on and on with the pointer over it.
    // The states are forced with pseudo classes, so they can be compared side by side.
    public partial class MainWindow : Window
    {
        private enum State { Idle, Hover, Pressed, On, OnHover }

        private static readonly string[] Greys =
            { "Grey300Brush", "Grey400Brush", "Grey500Brush", "Grey600Brush", "Grey700Brush", "Grey800Brush" };

        private static readonly State[] States = { State.Idle, State.Hover, State.Pressed, State.On, State.OnHover };

        private sealed record Sample(string Name, Func<Control> Make, bool CanBeOn);

        public MainWindow()
        {
            InitializeComponent();

            Bind(IdleBox, "StateIdleBrush", "Grey600Brush");
            Bind(HoverBox, "StateHoverBrush", "Grey700Brush");
            Bind(PressedBox, "StatePressedBrush", "Grey800Brush");
            Bind(RowBox, "RowHoverBrush", "Grey500Brush");

            Sections.Children.Add(Section("Surface controls", "surface", SurfaceSamples()));
            Sections.Children.Add(Section("Rows", "row", RowSamples()));
            Sections.Children.Add(Tabs());
            Sections.Children.Add(Section("Icon only (the rule is already this one)", "icon", IconSamples()));

        }

        private void Bind(ComboBox box, string resource, string initial)
        {
            box.ItemsSource = Greys;
            box.SelectedItem = initial;
            Application.Current!.Resources[resource] = Brush(initial);
            box.SelectionChanged += (_, _) => Application.Current!.Resources[resource] = Brush((string)box.SelectedItem!);
        }

        private static IBrush Brush(string key) => (IBrush)Application.Current!.FindResource(key)!;

        private static ControlTheme Theme(string key) => (ControlTheme)Application.Current!.FindResource(key)!;

        // ------------------------------------------------------------------ samples

        private static IEnumerable<Sample> SurfaceSamples()
        {
            yield return new("Button", () => new Button { Content = "Button", Width = 120 }, false);
            yield return new("ToggleButton", () => new ToggleButton { Content = "Toggle", Width = 120 }, true);
            yield return new("ToggleButtonBox", () => new ToggleButton { Theme = Theme("ToggleButtonBox") }, true);
            yield return new("ComboBox", () => new ComboBox { ItemsSource = new[] { "Item A", "Item B" }, SelectedIndex = 0, Width = 120 }, false);
            yield return new("DragValue", () => new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 140 }, false);
            yield return new("SliderCMiX", () => new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.4, Width = 140 }, false);
        }

        private static IEnumerable<Sample> RowSamples()
        {
            yield return new("List row", () => new ListBox { ItemsSource = new[] { "Row" }, Width = 140, Height = 34 }, true);
            yield return new("Combo item", () => new ComboBoxItem { Content = "Item", Width = 140 }, true);
        }

        private static IEnumerable<Sample> IconSamples()
        {
            yield return new("Eye", () => new ToggleButton { Theme = Theme("ToggleButtonVisibility") }, true);
            yield return new("Power", () => new ToggleButton { Theme = Theme("ToggleButtonPower") }, true);
            yield return new("Update", () => new ToggleButton { Theme = Theme("ToggleButtonUpdate") }, true);
            yield return new("Close", () => new Button { Theme = Theme("ButtonIconClose") }, false);
        }

        // ------------------------------------------------------------------ layout

        private Control Section(string title, string kind, IEnumerable<Sample> samples)
        {
            var grid = new Grid { Margin = new Thickness(8) };
            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(130)));
            foreach (var _ in States)
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(135)));

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            for (var column = 0; column < States.Length; column++)
                grid.Children.Add(At(Header(States[column].ToString()), 0, column + 1));

            var row = 1;
            foreach (var sample in samples)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                grid.Children.Add(At(Header(sample.Name), row, 0));

                for (var column = 0; column < States.Length; column++)
                {
                    var state = States[column];
                    if ((state is State.On or State.OnHover) && !sample.CanBeOn)
                        continue;

                    var control = sample.Make();
                    grid.Children.Add(At(Cell(control, state), row, column + 1));
                }
                row++;
            }

            return Frame(title, kind, grid);
        }

        private Control Tabs()
        {
            var strip = new TabControl { Theme = Theme("TabControlMain"), Width = 220, Height = 70, HorizontalAlignment = HorizontalAlignment.Left };
            foreach (var icon in new[] { "Settings", "Menu", "Folder" })
            {
                strip.Items.Add(new TabItem
                {
                    Theme = Theme("TabItemHorizontal"),
                    Header = new AppIcon { IconKey = icon, Size = 16 },
                });
            }

            var passes = 0;
            void OnLayout(object? sender, EventArgs e)
            {
                var second = strip.GetVisualDescendants().OfType<TabItem>().ElementAtOrDefault(1);
                if (second != null)
                    foreach (var part in second.GetSelfAndVisualDescendants().OfType<Control>())
                        ((IPseudoClasses)part.Classes).Set(":pointerover", true);
                if (++passes >= 8)
                    strip.LayoutUpdated -= OnLayout;
            }

            strip.LayoutUpdated += OnLayout;

            var holder = new StackPanel { Spacing = 4, Margin = new Thickness(8) };
            holder.Children.Add(Header("Selected, hovered (second), idle (third)"));
            holder.Children.Add(strip);
            return Frame("Tab strip with icons", "tabs", holder);
        }

        private Control Frame(string title, string kind, Control content)
        {
            var host = new Border
            {
                Background = Brush("Grey200Brush"),
                CornerRadius = new CornerRadius(4),
                Child = content,
            };

            var frame = new StackPanel { Spacing = 6 };
            frame.Children.Add(new TextBlock { Text = title, FontWeight = FontWeight.SemiBold });
            frame.Children.Add(host);
            return frame;
        }

        private static TextBlock Header(string text) =>
            new() { Text = text, Opacity = 0.7, VerticalAlignment = VerticalAlignment.Center };

        private static Control At(Control control, int row, int column)
        {
            control.Margin = new Thickness(5, 3);
            Grid.SetRow(control, row);
            Grid.SetColumn(control, column);
            return control;
        }

        // ------------------------------------------------------------------ states

        private static Control Cell(Control control, State state)
        {
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Center;

            if (state is State.On or State.OnHover)
                SetOn(control);

            if (state is State.Hover or State.OnHover)
                Force(control, ":pointerover");
            else if (state == State.Pressed)
                Force(control, ":pointerover", ":pressed");

            return control;
        }

        private static void SetOn(Control control)
        {
            switch (control)
            {
                case ToggleButton toggle:
                    toggle.IsChecked = true;
                    break;
                case ListBox list:
                    list.SelectedIndex = 0;
                    break;
                case ComboBoxItem item:
                    item.IsSelected = true;
                    break;
            }
        }

        // The pseudo classes go on the control and on every part of its template. The parts of a template exist
        // only after some layout passes, so this runs again for the first layout passes.
        private static void Force(Control control, params string[] pseudoClasses)
        {
            void Apply()
            {
                foreach (var part in control.GetSelfAndVisualDescendants().OfType<Control>())
                    foreach (var name in pseudoClasses)
                        ((IPseudoClasses)part.Classes).Set(name, true);
            }

            var passes = 0;
            void OnLayout(object? sender, EventArgs e)
            {
                Apply();
                if (++passes >= 8)
                    control.LayoutUpdated -= OnLayout;
            }

            control.LayoutUpdated += OnLayout;
        }
    }
}
