using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Drives the target on a real project, because the messages and the undo steps a snapshot
    // apply must not produce come from the real managers and controls.
    public class ProjectSyncTargetTests
    {
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

        private static ProjectModel ModelWithOneComposition()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        private static Guid FirstCompositionId(ProjectModel model)
            => model.CompositionManager.ManagerData.Items[0].ID;

        [Fact]
        public void ApplySnapshot_SendsNothing_RecordsNoUndo_AndReplacesTheState()
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
    }
}
