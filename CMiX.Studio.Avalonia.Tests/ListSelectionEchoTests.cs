using System.Reflection;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewBeatStepModulator = CMiX.Studio.Avalonia.Views.BeatStepModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListSelectionEchoTests
    {
        public enum ListKind { PrefabSlot, Repository, BeatStep }

        public static TheoryData<ListKind> Kinds => new() { ListKind.PrefabSlot, ListKind.Repository, ListKind.BeatStep };

        private sealed class MessageRecorder : IMessageSender
        {
            public List<IMessage> Sent { get; } = new();
            public void SendMessage(IMessage message) => Sent.Add(message);
        }

        private sealed class ListHarness
        {
            private ListHarness() { }

            public UndoManager Undo { get; private init; } = null!;
            public PrefabManager Manager { get; private init; } = null!;
            public ListBox List { get; private init; } = null!;
            public Type ItemType { get; private init; } = null!;
            public MessageRecorder Recorder { get; } = new();

            public int UndoSteps => StackSize("_undoStack");
            public int RedoSteps => StackSize("_redoStack");
            public IReadOnlyList<Guid> Ids => Manager.ManagerData.Items.Select(item => item.ID).ToList();

            public static ListHarness Create(ListKind kind, int itemCount)
            {
                var provider = TestServiceProviderFactory.Create();
                PrefabManager manager;
                Control view;
                var itemType = typeof(Entity);

                switch (kind)
                {
                    case ListKind.PrefabSlot:
                        manager = provider.GetRequiredService<PrefabManager>();
                        view = new PrefabSlotManager { DataContext = manager };
                        break;
                    case ListKind.Repository:
                        manager = provider.GetRequiredService<PrefabManager>();
                        view = new RepositoryManager { DataContext = manager, ItemsSource = manager.ManagerData.Items };
                        break;
                    default:
                        var modulator = (BeatStepModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(BeatStepModulator));
                        manager = modulator.StepManager;
                        view = new ViewBeatStepModulator { DataContext = modulator };
                        itemType = typeof(BeatStep);
                        break;
                }

                new Window { Content = view, Width = 400, Height = 400 }.Show();
                Dispatcher.UIThread.RunJobs();

                var harness = new ListHarness
                {
                    Undo = provider.GetRequiredService<UndoManager>(),
                    Manager = manager,
                    List = view.GetVisualDescendants().OfType<ListBox>().First(),
                    ItemType = itemType
                };
                for (var i = 0; i < itemCount; i++)
                    manager.AddItem(itemType);
                Dispatcher.UIThread.RunJobs();

                harness.Undo.Clear();
                provider.GetRequiredService<ControlMessenger>().Register(harness.Recorder);
                return harness;
            }

            public void Do(Action action)
            {
                Recorder.Sent.Clear();
                action();
                Dispatcher.UIThread.RunJobs();
            }

            public void UserSelects(int index) => List.SelectedItem = Manager.ManagerData.Items[index];

            public IReadOnlyList<T> Sent<T>() => Recorder.Sent.OfType<T>().ToList();

            private int StackSize(string field) =>
                ((System.Collections.ICollection)typeof(UndoManager)
                    .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetValue(Undo)!).Count;
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Add_RecordsOneUndoStepAndSelectsTheNewItem(ListKind kind)
        {
            var harness = ListHarness.Create(kind, 0);

            for (var i = 0; i < 3; i++)
            {
                harness.Do(() => harness.Manager.AddItem(harness.ItemType));

                Assert.Equal(i + 1, harness.UndoSteps);
                Assert.Equal(i, harness.Manager.ManagerData.SelectedIndex);
                Assert.Same(harness.Manager.ManagerData.Items[i], harness.List.SelectedItem);
                Assert.Equal(i, Assert.Single(harness.Sent<MessageAddItem>()).SelectedIndex);
            }
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void UserSelect_RecordsOneUndoStepAndSendsOneSelectMessage(ListKind kind)
        {
            var harness = ListHarness.Create(kind, 3);

            harness.Do(() => harness.UserSelects(0));

            Assert.Equal(1, harness.UndoSteps);
            Assert.Equal(0, harness.Manager.ManagerData.SelectedIndex);
            Assert.Equal(0, Assert.Single(harness.Sent<MessageSelectedItemChanged>()).Index);
            Assert.Single(harness.Recorder.Sent);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void UndoAndRedoOfSelects_MoveTheSelectionAndRecordNothingNew(ListKind kind)
        {
            var harness = ListHarness.Create(kind, 3);
            harness.Do(() => harness.UserSelects(0));
            harness.Do(() => harness.UserSelects(1));

            harness.Do(harness.Undo.Undo);
            Assert.Equal((0, 1, 1), (harness.Manager.ManagerData.SelectedIndex, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[0], harness.List.SelectedItem);

            harness.Do(harness.Undo.Undo);
            Assert.Equal((2, 0, 2), (harness.Manager.ManagerData.SelectedIndex, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[2], harness.List.SelectedItem);

            harness.Do(harness.Undo.Redo);
            Assert.Equal((0, 1, 1), (harness.Manager.ManagerData.SelectedIndex, harness.UndoSteps, harness.RedoSteps));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void UndoAndRedoOfAnAdd_BringBackThePreviousSelection(ListKind kind)
        {
            var harness = ListHarness.Create(kind, 2);
            harness.Do(() => harness.Manager.AddItem(harness.ItemType));
            var ids = harness.Ids;

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids.Take(2), harness.Ids);
            Assert.Equal((1, 0, 1), (harness.Manager.ManagerData.SelectedIndex, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[1], harness.List.SelectedItem);

            harness.Do(harness.Undo.Redo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((2, 1, 0), (harness.Manager.ManagerData.SelectedIndex, harness.UndoSteps, harness.RedoSteps));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Delete_IsOneUndoStepAndUndoBringsTheItemBack(ListKind kind)
        {
            var harness = ListHarness.Create(kind, 3);
            var ids = harness.Ids;

            harness.Do(() => harness.Manager.RemoveItem(harness.Manager.ManagerData.Items[1]));

            Assert.Equal(new[] { ids[0], ids[2] }, harness.Ids);
            Assert.Equal(1, harness.UndoSteps);

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((0, 1), (harness.UndoSteps, harness.RedoSteps));
        }
    }
}
