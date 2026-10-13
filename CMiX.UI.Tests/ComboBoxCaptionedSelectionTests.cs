using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Enum;
using Xunit;

namespace CMiX.UI.Tests
{
    // ComboBoxCaptioned keeps its own SelectedItem and passes only real picks to the model.
    // A plain ComboBox loses a selection that comes before its items, and it sends an empty
    // selection to the model while its items are replaced. These tests keep that protection.
    public class ComboBoxCaptionedSelectionTests
    {
        private class Model : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            public List<string> Writes { get; } = new();
            private object? _selected;

            public object? Selected
            {
                get => _selected;
                set
                {
                    Writes.Add(value?.ToString() ?? "null");
                    _selected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
                }
            }

            // The model changes its own value without a write from the control.
            public void Change(object? value)
            {
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }

        private static ComboBoxCaptioned Show(Model model, IEnumerable<string>? items)
        {
            var combo = new ComboBoxCaptioned { Width = 300, Height = 30 };
            combo.Bind(ComboBoxCaptioned.SelectedItemProperty, new Binding("Selected", BindingMode.TwoWay) { Source = model });
            if (items != null)
                combo.ItemsSource = items;
            new Window { Content = combo, Width = 400, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();
            return combo;
        }

        private static ComboBox Inner(ComboBoxCaptioned combo) => combo.GetVisualDescendants().OfType<ComboBox>().Single();

        [AvaloniaFact]
        public void ASelectionThatComesBeforeTheItems_IsShownWhenTheItemsArrive()
        {
            var model = new Model { Selected = "B" };
            model.Writes.Clear();
            var combo = Show(model, null);

            combo.ItemsSource = new[] { "A", "B" };
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("B", combo.SelectedItem);
            Assert.Equal("B", Inner(combo).SelectedItem);
            Assert.Empty(model.Writes);
        }

        [AvaloniaFact]
        public void ReplacingTheItems_NeverSendsAnEmptySelectionToTheModel()
        {
            var model = new Model { Selected = "B" };
            model.Writes.Clear();
            var combo = Show(model, new ObservableCollection<string> { "A", "B" });

            combo.ItemsSource = new ObservableCollection<string> { "A", "B", "C" };
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("B", Inner(combo).SelectedItem);
            Assert.DoesNotContain("null", model.Writes);
            Assert.Equal("B", model.Selected);
        }

        [AvaloniaFact]
        public void ClearingAndRefillingTheItemsInPlace_KeepsTheSelection_AndSendsNoEmptySelection()
        {
            var model = new Model { Selected = "B" };
            model.Writes.Clear();
            var items = new ObservableCollection<string> { "A", "B" };
            var combo = Show(model, items);

            items.Clear();
            items.Add("A");
            items.Add("B");
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("B", combo.SelectedItem);
            Assert.Equal("B", Inner(combo).SelectedItem);
            Assert.DoesNotContain("null", model.Writes);
            Assert.Equal("B", model.Selected);
        }

        [AvaloniaFact]
        public void ARealPick_ReachesTheModel()
        {
            var model = new Model { Selected = "B" };
            model.Writes.Clear();
            var combo = Show(model, new[] { "A", "B" });

            Inner(combo).SelectedItem = "A";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("A", combo.SelectedItem);
            Assert.Equal("A", model.Selected);
        }

        [AvaloniaFact]
        public void AChangeOfTheModel_IsShown()
        {
            var model = new Model { Selected = "B" };
            var combo = Show(model, new[] { "A", "B" });

            model.Change("A");
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("A", combo.SelectedItem);
            Assert.Equal("A", Inner(combo).SelectedItem);
        }
    }
}
