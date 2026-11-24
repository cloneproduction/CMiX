// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BeatStepModulator = CMiX.Core.Modulation.Modulators.BeatStepModulator;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using ViewBeatStepModulator = CMiX.Studio.Avalonia.Views.BeatStepModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // A list view and its manager agree on the selection: each user action is one undo step and one message,
    // and undo, redo and the messages of a peer bring the same selection back.
    public class SelectionEchoTests : IDisposable
    {
        public enum ListKind { Composable, PrefabSlot, Repository, BeatStep }

        public static TheoryData<ListKind> Kinds => new() { ListKind.Composable, ListKind.PrefabSlot, ListKind.Repository, ListKind.BeatStep };

        private readonly List<Window> windows = new();

        public void Dispose()
        {
            foreach (var window in windows)
                window.Close();
        }

        private sealed class Harness
        {
            private Harness() { }

            public Window Window { get; private init; } = null!;
            public UndoManager Undo { get; private init; } = null!;
            public PrefabManager Compositions { get; private init; } = null!;
            public PrefabManager Manager { get; private init; } = null!;
            public ListBox List { get; private init; } = null!;
            public Type ItemType { get; private init; } = null!;
            public MessageRecorder Recorder { get; } = new();

            public int UndoSteps => UndoCounts.StepsOf(Undo);
            public int RedoSteps => UndoCounts.RedoStepsOf(Undo);
            public int Selected => Manager.ManagerData.SelectedIndex;
            public IReadOnlyList<Guid> Ids => Manager.ManagerData.Items.Select(item => item.ID).ToList();

            // The list of the layers of a composition. A peer receives the composition of the sender.
            public static Harness Create(ListKind kind, int itemCount = 0, Harness? sender = null)
            {
                var provider = TestServiceProviderFactory.Create();
                var compositions = provider.GetRequiredService<PrefabManager>();
                PrefabManager manager;
                Control view;
                var itemType = typeof(Entity);

                switch (kind)
                {
                    case ListKind.Composable:
                        if (sender == null)
                            compositions.AddItem(typeof(Composition));
                        else
                            compositions.Receive(new MessageAddItem(compositions.ManagerData.ID, sender.Compositions.SelectedItem.ToModel(), 0));
                        manager = ((Composition)compositions.ManagerData.Items.Single()).LayerManager;
                        view = new CMiX.Studio.Avalonia.Views.ListComposable { ItemType = typeof(Layer), DataContext = manager };
                        itemType = typeof(Layer);
                        break;
                    case ListKind.PrefabSlot:
                        manager = compositions;
                        view = new ListPrefabSlot { DataContext = manager };
                        break;
                    case ListKind.Repository:
                        manager = compositions;
                        view = new ListRepository { DataContext = manager, ItemsSource = manager.ManagerData.Items };
                        break;
                    default:
                        var modulator = (BeatStepModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(BeatStepModulator));
                        manager = modulator.StepManager;
                        view = new ViewBeatStepModulator { DataContext = modulator };
                        itemType = typeof(BeatStep);
                        break;
                }

                var window = new Window { Content = view, Width = 400, Height = 400 };
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var harness = new Harness
                {
                    Window = window,
                    Undo = provider.GetRequiredService<UndoManager>(),
                    Compositions = compositions,
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
        }

        private Harness Create(ListKind kind, int itemCount = 0, Harness? sender = null)
        {
            var harness = Harness.Create(kind, itemCount, sender);
            windows.Add(harness.Window);
            return harness;
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void AddItem_ThreeTimes_RecordsOneUndoStepEachAndSelectsTheNewItem(ListKind kind)
        {
            var harness = Create(kind);

            for (var i = 0; i < 3; i++)
            {
                harness.Do(() => harness.Manager.AddItem(harness.ItemType));

                Assert.Equal(i + 1, harness.UndoSteps);
                Assert.Equal(i, harness.Selected);
                Assert.Same(harness.Manager.ManagerData.Items[i], harness.List.SelectedItem);
                Assert.Equal(i, Assert.Single(harness.Sent<MessageAddItem>()).SelectedIndex);
                Assert.Empty(harness.Sent<MessageRemoveItem>());
                Assert.Empty(harness.Sent<MessageMoveItem>());
            }
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Select_ByTheUser_RecordsOneUndoStepAndSendsOneSelectMessage(ListKind kind)
        {
            var harness = Create(kind, 3);

            harness.Do(() => harness.UserSelects(0));

            Assert.Equal(1, harness.UndoSteps);
            Assert.Equal(0, harness.Selected);
            Assert.Equal(0, Assert.Single(harness.Sent<MessageSelectItem>()).Index);
            Assert.Single(harness.Recorder.Sent);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void UndoAndRedo_TwoSelects_MoveTheSelectionAndRecordNothingNew(ListKind kind)
        {
            var harness = Create(kind, 3);
            harness.Do(() => harness.UserSelects(0));
            harness.Do(() => harness.UserSelects(1));
            Assert.Equal(2, harness.UndoSteps);

            harness.Do(harness.Undo.Undo);
            Assert.Equal((0, 1, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[0], harness.List.SelectedItem);

            harness.Do(harness.Undo.Undo);
            Assert.Equal((2, 0, 2), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[2], harness.List.SelectedItem);

            harness.Do(harness.Undo.Redo);
            Assert.Equal((0, 1, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[0], harness.List.SelectedItem);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void UndoAndRedo_AnAdd_BringBackThePreviousSelection(ListKind kind)
        {
            var harness = Create(kind, 2);
            harness.Do(() => harness.Manager.AddItem(harness.ItemType));
            var ids = harness.Ids;

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids.Take(2), harness.Ids);
            Assert.Equal((1, 0, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[1], harness.List.SelectedItem);

            harness.Do(harness.Undo.Redo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((2, 1, 0), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Same(harness.Manager.ManagerData.Items[2], harness.List.SelectedItem);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void RemoveItem_TheMiddleItem_IsOneUndoStepAndUndoBringsItBack(ListKind kind)
        {
            var harness = Create(kind, 3);
            var ids = harness.Ids;

            harness.Do(() => harness.Manager.RemoveItem(harness.Manager.ManagerData.Items[1]));

            Assert.Equal(new[] { ids[0], ids[2] }, harness.Ids);
            Assert.Equal(1, harness.UndoSteps);
            Assert.Single(harness.Sent<MessageRemoveItem>());

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((0, 1), (harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(harness.Selected, harness.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void DuplicateItem_ALayer_IsOneUndoStepAndUndoBringsBackTheSelection()
        {
            var harness = Create(ListKind.Composable, 3);
            var ids = harness.Ids;

            harness.Do(() => harness.Manager.DuplicateItem(harness.Manager.ManagerData.Items[0]));

            Assert.Equal(4, harness.Ids.Count);
            Assert.Equal(ids[0], harness.Ids[0]);
            Assert.Equal(ids.Skip(1), harness.Ids.Skip(2));
            Assert.Equal((1, 1), (harness.Selected, harness.UndoSteps));
            Assert.Equal(1, harness.List.SelectedIndex);

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((2, 0, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(2, harness.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void MoveItemTo_ALayer_IsOneUndoStepAndUndoPutsItBack()
        {
            var harness = Create(ListKind.Composable, 3);
            var ids = harness.Ids;

            harness.Do(() => harness.Manager.MoveItemTo(harness.Manager.ManagerData.Items[0], 2));

            Assert.Equal(new[] { ids[1], ids[2], ids[0] }, harness.Ids);
            Assert.Equal(1, harness.UndoSteps);
            Assert.Single(harness.Sent<MessageMoveItem>());

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((0, 1), (harness.UndoSteps, harness.RedoSteps));
        }

        // Three layers come and the first is selected. The peer receives every message of the sender.
        private (Harness Sender, Harness Peer) ReplayOnAPeer()
        {
            var sender = Create(ListKind.Composable);
            var peer = Create(ListKind.Composable, sender: sender);
            Assert.Equal(sender.Manager.ManagerData.ID, peer.Manager.ManagerData.ID);

            sender.Do(() =>
            {
                sender.Manager.AddItem(typeof(Layer));
                sender.Manager.AddItem(typeof(Layer));
                sender.Manager.AddItem(typeof(Layer));
                sender.List.SelectedIndex = 0;
            });

            peer.Do(() =>
            {
                foreach (var message in sender.Recorder.Sent.ToList())
                    peer.Manager.Receive(message);
            });
            return (sender, peer);
        }

        [AvaloniaFact]
        public void Receive_TheMessagesOfAPeer_EndsWithTheSameItemsAndSelection()
        {
            var (sender, peer) = ReplayOnAPeer();

            Assert.Equal(sender.Ids, peer.Ids);
            Assert.Equal(sender.Selected, peer.Selected);
            Assert.Equal(sender.Selected, peer.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void Receive_TheMessagesOfAPeer_RecordsNoUndoStep()
        {
            var (_, peer) = ReplayOnAPeer();

            Assert.Equal(0, peer.UndoSteps);
        }

        [AvaloniaFact]
        public void Receive_TheMessagesOfAPeer_SendsNothingBack()
        {
            var (_, peer) = ReplayOnAPeer();

            Assert.Empty(peer.Recorder.Sent);
        }
    }
}
