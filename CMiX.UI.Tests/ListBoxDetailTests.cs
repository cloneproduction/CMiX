using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // ListBoxDetail is a list with a delete button, an add area and the detail of the selected item.
    public class ListBoxDetailTests
    {
        private sealed class Row
        {
            public Row(string name) { Name = name; }
            public string Name { get; }
        }

        private static (Window window, ListBoxDetail detail, ObservableCollection<Row> rows) Show(Action<ListBoxDetail>? configure = null)
        {
            var rows = new ObservableCollection<Row> { new("one"), new("two") };
            var detail = new ListBoxDetail { ItemsSource = rows };
            detail.DataTemplates.Add(new FuncDataTemplate<Row>((row, _) => new TextBlock { Text = "row " + row.Name }));
            configure?.Invoke(detail);
            var window = new Window { Content = detail, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, detail, rows);
        }

        private static ListBoxCMiX ListOf(ListBoxDetail detail) => detail.GetVisualDescendants().OfType<ListBoxCMiX>().Single();

        [AvaloniaFact]
        public void ShowsTheItems_WithTheTemplatesOfTheApplication()
        {
            var (_, detail, _) = Show();

            var texts = detail.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();

            Assert.Contains("row one", texts);
            Assert.Contains("row two", texts);
        }

        [AvaloniaFact]
        public void ThePickOfARow_IsWrittenToSelectedItem_AndBack()
        {
            var (_, detail, rows) = Show();

            ListOf(detail).SelectedItem = rows[1];
            Dispatcher.UIThread.RunJobs();
            Assert.Same(rows[1], detail.SelectedItem);

            detail.SelectedItem = rows[0];
            Dispatcher.UIThread.RunJobs();
            Assert.Same(rows[0], ListOf(detail).SelectedItem);
        }

        [AvaloniaFact]
        public void TheDeleteButton_HasTheCommandAndTheSelectedItemAsItsParameter()
        {
            object? received = null;
            var command = new RelayCommand<object?>(p => received = p);
            var (_, detail, rows) = Show(d => d.DeleteCommand = command);
            detail.SelectedItem = rows[1];
            Dispatcher.UIThread.RunJobs();
            var delete = detail.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, "-"));

            Assert.Same(command, delete.Command);
            Assert.Same(rows[1], delete.CommandParameter);

            delete.Command!.Execute(delete.CommandParameter);
            Assert.Same(rows[1], received);
        }

        [AvaloniaFact]
        public void TheAddContent_IsShownUnderTheList()
        {
            var add = new Button { Content = "add here" };
            var (_, detail, _) = Show(d => d.AddContent = add);

            Assert.True(add.IsEffectivelyVisible);
            Assert.Contains(detail.GetVisualDescendants(), v => v == add);
        }

        [AvaloniaFact]
        public void TheDetail_ShowsOnlyWhileAnItemIsSelected_AndHasThatItemAsDataContext()
        {
            var content = new Button();
            content.Bind(ContentControl.ContentProperty, new Binding("Name"));
            var (_, detail, rows) = Show(d => d.DetailContent = content);

            detail.SelectedItem = rows[1];
            Dispatcher.UIThread.RunJobs();

            Assert.True(content.IsEffectivelyVisible);
            Assert.Equal("two", content.Content);

            detail.SelectedItem = null;
            Dispatcher.UIThread.RunJobs();
            Assert.False(content.IsEffectivelyVisible);
        }
    }
}
