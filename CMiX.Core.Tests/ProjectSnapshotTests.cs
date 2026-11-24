// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // ApplySnapshot puts the model of a saved project into a project. It sends no message and writes no undo step.
    public class ProjectSnapshotTests
    {
        private static Project NewProject() => TestServiceProviderFactory.Create().GetRequiredService<Project>();

        private static Project ProjectWithThreeCompositions()
        {
            var project = NewProject();
            foreach (var name in new[] { "A", "B", "C" })
            {
                project.CompositionManager.AddItem(typeof(Composition));
                ((Composition)project.CompositionManager.SelectedItem).PrefabService.Name.Value = name;
            }
            return project;
        }

        private static ProjectModel Save(Project project) => (ProjectModel)project.ToModel();

        private static List<string> Names(Project project) =>
            project.CompositionManager.ManagerData.Items.Cast<Composition>().Select(c => c.PrefabService.Name.Value).ToList();

        private sealed class Fixture
        {
            public Fixture()
            {
                var provider = TestServiceProviderFactory.Create();
                Project = provider.GetRequiredService<Project>();
                Messenger = provider.GetRequiredService<ControlMessenger>();
                UndoManager = provider.GetRequiredService<UndoManager>();
                Target = new ProjectSyncTarget(Project, Messenger, UndoManager);
                Sender = new RecordingMessageSender();
                Messenger.Register(Sender);
            }

            public Project Project { get; }
            public ControlMessenger Messenger { get; }
            public UndoManager UndoManager { get; }
            public ProjectSyncTarget Target { get; }
            public RecordingMessageSender Sender { get; }

            public PrefabManager CompositionManager => Project.CompositionManager;
        }

        private static Guid FirstCompositionId(ProjectModel model)
            => model.CompositionManager.ManagerData.Items[0].ID;

        private static void AddAddress(Project project, string text)
        {
            var manager = project.RedisAddressRegistry.AddressManager;
            manager.AddItem(typeof(RedisAddress));
            ((RedisAddress)manager.SelectedItem).Value = text;
        }

        private static IEnumerable<string> Addresses(Project project) =>
            project.RedisAddressRegistry.Addresses.Select(address => address.Value);

        // A project and the messages it sends from now on.
        private static (Project Project, ControlMessenger Messenger, RecordingMessageSender Sender) NewRecordedProject()
        {
            var provider = TestServiceProviderFactory.Create();
            var messenger = provider.GetRequiredService<ControlMessenger>();
            var sender = new RecordingMessageSender();
            messenger.Register(sender);
            var project = provider.GetRequiredService<Project>();
            sender.Sent.Clear();
            return (project, messenger, sender);
        }

        private static ProjectModel ModelWithAddresses(params string[] texts)
        {
            var source = NewProject();
            foreach (var text in texts)
                AddAddress(source, text);
            return (ProjectModel)source.ToModel();
        }

        private static Project NewProjectWith(string text)
        {
            var project = NewProject();
            AddAddress(project, text);
            return project;
        }

        [Fact]
        public void ApplySnapshot_OfThreeCompositions_AddsThemInFileOrder()
        {
            var model = Save(ProjectWithThreeCompositions());
            var target = NewProject();

            target.ApplySnapshot(model);

            Assert.Equal(new List<string> { "A", "B", "C" }, Names(target));
        }

        [Fact]
        public void ApplySnapshot_OfASelection_SelectsTheSameComposition()
        {
            var source = ProjectWithThreeCompositions();
            source.CompositionManager.ManagerData.SelectAt(1);
            var model = Save(source);
            var target = NewProject();

            target.ApplySnapshot(model);

            Assert.Equal(1, target.CompositionManager.ManagerData.SelectedIndex);
            Assert.Same(target.CompositionManager.ManagerData.Items[1], target.CompositionManager.SelectedItem);
        }

        [Fact]
        public void ApplySnapshot_WhenTheFileHasNoSelection_LeavesNothingSelected()
        {
            var source = ProjectWithThreeCompositions();
            source.CompositionManager.ManagerData.ClearSelection();
            var model = Save(source);
            var target = NewProject();

            target.ApplySnapshot(model);

            Assert.Equal(-1, target.CompositionManager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void ApplySnapshot_WhenTheSavedIndexIsOutsideTheList_LeavesNothingSelected()
        {
            var model = Save(ProjectWithThreeCompositions());
            model.CompositionManager.ManagerData.SelectedIndex = 7;
            var target = NewProject();

            target.ApplySnapshot(model);

            Assert.Equal(-1, target.CompositionManager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void ApplySnapshot_OfASavedProject_RestoresTheMasterBeatAndTheName()
        {
            var source = NewProject();
            source.MasterBeat.Period.Value = 437.5f;
            source.PrefabService.Name.Value = "Saved project";
            var model = Save(source);
            var target = NewProject();

            target.ApplySnapshot(model);

            Assert.Equal(437.5f, target.MasterBeat.Period.Value);
            Assert.Equal("Saved project", target.PrefabService.Name.Value);
        }

        [Fact]
        public void ApplySnapshot_OfAProjectWithoutCompositions_LeavesNothingSelected()
        {
            var model = Save(NewProject());
            var target = NewProject();

            target.ApplySnapshot(model);

            Assert.Empty(target.CompositionManager.ManagerData.Items);
            Assert.Null(target.CompositionManager.SelectedItem);
        }

        [Fact]
        public void ApplySnapshot_OnTheTarget_SendsNothingRecordsNoUndoAndReplacesTheState()
        {
            var fixture = new Fixture();
            fixture.Messenger.IsSendingBlocked = false;
            fixture.CompositionManager.AddItem(typeof(Composition));
            fixture.Sender.Sent.Clear();

            var model = ModelWithOneComposition();
            fixture.Target.ApplySnapshot(model);

            Assert.Empty(fixture.Sender.Sent);
            Assert.False(fixture.Messenger.IsSendingBlocked);
            Assert.False(fixture.UndoManager.CanUndo);
            Assert.False(fixture.UndoManager.IsSuppressed);
            var item = Assert.Single(fixture.CompositionManager.ManagerData.Items);
            Assert.Equal(FirstCompositionId(model), item.ID);
        }

        [Fact]
        public void ApplySnapshot_WhileSendingIsBlocked_LeavesItBlocked()
        {
            var fixture = new Fixture();
            fixture.Messenger.IsSendingBlocked = true;

            fixture.Target.ApplySnapshot(ModelWithOneComposition());

            Assert.True(fixture.Messenger.IsSendingBlocked);
            Assert.Empty(fixture.Sender.Sent);
        }

        [Fact]
        public void Apply_WithSnapshotMessage_BehavesLikeApplySnapshot()
        {
            var fixture = new Fixture();
            fixture.Messenger.IsSendingBlocked = false;
            fixture.CompositionManager.AddItem(typeof(Composition));
            fixture.Sender.Sent.Clear();

            var model = ModelWithOneComposition();
            fixture.Target.Apply(new MessageProjectSnapshot(Guid.NewGuid(), model));

            Assert.Empty(fixture.Sender.Sent);
            Assert.False(fixture.Messenger.IsSendingBlocked);
            Assert.False(fixture.UndoManager.CanUndo);
            var item = Assert.Single(fixture.CompositionManager.ManagerData.Items);
            Assert.Equal(FirstCompositionId(model), item.ID);
        }

        [Fact]
        public void ApplySnapshot_WhenTheApplyThrows_RestoresTheMessengerAndTheUndoManager()
        {
            var fixture = new Fixture();
            fixture.Messenger.IsSendingBlocked = false;

            Assert.ThrowsAny<Exception>(() => fixture.Target.ApplySnapshot(null));

            Assert.False(fixture.Messenger.IsSendingBlocked);
            Assert.False(fixture.UndoManager.IsSuppressed);
            Assert.Empty(fixture.Sender.Sent);
        }

        [Fact]
        public void ApplySnapshot_WithAddresses_SendsNothing()
        {
            var model = ModelWithAddresses("cmix:a");
            var (project, messenger, sender) = NewRecordedProject();
            messenger.IsSendingBlocked = true;

            project.ApplySnapshot(model);

            Assert.Empty(sender.Sent);
            Assert.Equal(new[] { "cmix:a" }, Addresses(project));
        }

        [Fact]
        public void ApplySnapshot_OfAModelWithoutAddresses_KeepsTheFixedID()
        {
            var project = NewProject();

            project.ApplySnapshot(new ProjectModel());

            Assert.Equal(ManagerIDs.RedisAddressManager, project.RedisAddressRegistry.AddressManager.ManagerData.ID);
            Assert.Empty(Addresses(project));
        }

        [Fact]
        public void Reset_OfAProjectWithAnAddress_LeavesNoAddress()
        {
            var project = NewProject();
            AddAddress(project, "cmix:a");

            project.Reset();

            Assert.Empty(Addresses(project));
        }

        [Fact]
        public void ApplySnapshot_WithAddressesAndCompositions_RestoresTheAddressesFirst()
        {
            var source = NewProjectWith("cmix:a");
            source.CompositionManager.AddItem(typeof(Composition));
            var snapshot = (ProjectModel)source.ToModel();
            var target = NewProject();
            var addressesWhenACompositionIsAdded = -1;
            target.CompositionManager.ManagerData.Items.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add && addressesWhenACompositionIsAdded < 0)
                    addressesWhenACompositionIsAdded = target.RedisAddressRegistry.Addresses.Count();
            };

            target.ApplySnapshot(snapshot);

            Assert.Equal(1, addressesWhenACompositionIsAdded);
        }

        [Fact]
        public void ApplySnapshot_OverOldAddresses_ReplacesThem()
        {
            var project = NewProject();
            AddAddress(project, "cmix:old");
            var snapshot = (ProjectModel)NewProjectWith("cmix:new").ToModel();

            project.ApplySnapshot(snapshot);

            Assert.Equal(new[] { "cmix:new" }, Addresses(project));
        }
    }
}
