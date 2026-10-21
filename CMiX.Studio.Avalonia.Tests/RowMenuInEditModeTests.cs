using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The menu of a row never opens while the name of the row is in the edit mode. This holds for every kind of list.
    public class RowMenuInEditModeTests
    {
        public enum RowKind { LayerList, SlotList, RepositoryList }

        public static TheoryData<RowKind> Kinds => new() { RowKind.LayerList, RowKind.SlotList, RowKind.RepositoryList };

        private static Control FirstRow(RowKind kind)
        {
            var provider = TestServiceProviderFactory.Create();
            Control view;
            Func<Control> row;

            if (kind == RowKind.LayerList)
            {
                var compositions = provider.GetRequiredService<PrefabManager>();
                compositions.AddItem(typeof(Composition));
                var layers = ((Composition)compositions.SelectedItem).LayerManager;
                layers.AddItem(typeof(Layer));
                view = new ComposableManager { ItemType = typeof(Layer), DataContext = layers };
                row = () => view.GetVisualDescendants().OfType<ComposableManagerItem>().First();
            }
            else
            {
                var manager = provider.GetRequiredService<PrefabManager>();
                view = kind == RowKind.SlotList
                    ? new PrefabSlotManager { DataContext = manager, RowMenu = RowMenuKind.Slot }
                    : new RepositoryManager { DataContext = manager, ItemsSource = manager.ManagerData.Items, RowMenu = RowMenuKind.Repository };
                new Window { Content = view, Width = 500, Height = 600 }.Show();
                Dispatcher.UIThread.RunJobs();
                manager.AddItem(typeof(Entity));
                Dispatcher.UIThread.RunJobs();
                row = () => view.GetVisualDescendants().OfType<ListBoxItem>().First();
            }

            if (kind == RowKind.LayerList)
            {
                new Window { Content = view, Width = 500, Height = 600 }.Show();
                Dispatcher.UIThread.RunJobs();
            }

            return row();
        }

        private static void RequestMenuFrom(Control row)
        {
            var input = row.GetVisualDescendants().OfType<TextBox>().First();
            input.RaiseEvent(new ContextRequestedEventArgs { Source = input });
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void ContextRequest_OnTheName_OpensTheMenuOfTheRow(RowKind kind)
        {
            var row = FirstRow(kind);

            RequestMenuFrom(row);

            Assert.True(row.ContextMenu!.IsOpen);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void ContextRequest_WhileTheNameIsEdited_OpensNoMenu(RowKind kind)
        {
            var row = FirstRow(kind);
            row.GetVisualDescendants().OfType<TextBoxEditable>().First().IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            RequestMenuFrom(row);

            Assert.False(row.ContextMenu!.IsOpen);
        }
    }
}
