using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // A peer that opens a project file must leave every other peer with the same project.
    public class ProjectOpenSyncTests
    {
        private sealed class JoinedPeer : IDisposable
        {
            public Project Project { get; }
            public UndoManager UndoManager { get; }
            public SyncPeer Peer { get; }
            public ProjectSyncTarget Target { get; }

            public JoinedPeer(InMemorySyncStore store, string name, bool isWriter)
            {
                var provider = TestServiceProviderFactory.Create();
                Project = provider.GetRequiredService<Project>();
                UndoManager = provider.GetRequiredService<UndoManager>();
                var messenger = provider.GetRequiredService<ControlMessenger>();
                Target = new ProjectSyncTarget(Project, messenger, UndoManager, provider.GetRequiredService<ControlRegistry>());
                Peer = CreatePeer(Target, store, messenger, isWriter);
                messenger.Register(Peer);
                Peer.StartAndJoin(Options(name));
            }

            public void Dispose() => Peer.Dispose();
        }

        // A project with one of each kind of part that a file holds.
        private static ProjectModel SavedProject()
        {
            var source = TestServiceProviderFactory.Create().GetRequiredService<Project>();

            var addresses = source.RedisAddressRegistry.AddressManager;
            addresses.AddItem(typeof(RedisAddress));
            ((RedisAddress)addresses.SelectedItem).Value = "PouetPouet";

            source.CompositionManager.AddItem(typeof(Composition));
            source.CompositionManager.AddItem(typeof(Composition));
            source.MasterBeat.Period.Value = 437.5f;
            source.PrefabService.Name.Value = "Saved project";
            source.OutputMappingManager.Items[0].Name.Value = "Left wall";

            return (ProjectModel)source.ToModel();
        }

        // Opens a file the way the studio does: the file is applied as one project, then pushed.
        private static async Task Open(JoinedPeer studio, ProjectModel model)
        {
            studio.Target.ApplySnapshot(model);
            await studio.Peer.PushAsync();
        }

        [Fact]
        public async Task Open_OnTheStudio_GivesTheEngineTheSameProject()
        {
            var store = new InMemorySyncStore();
            using var studio = new JoinedPeer(store, "studio", isWriter: true);
            await WaitUntilAsync(() => studio.Peer.IsJoined);
            using var engine = new JoinedPeer(store, "engine", isWriter: false);
            await WaitUntilAsync(() => engine.Peer.IsJoined);

            await Open(studio, SavedProject());

            var expected = ProjectStateHash.Compute(studio.Project);
            await WaitUntilAsync(
                () => ProjectStateHash.Compute(engine.Project) == expected,
                detail: () => $"studio addresses=[{Addresses(studio.Project)}] engine addresses=[{Addresses(engine.Project)}] "
                              + $"studio compositions={studio.Project.CompositionManager.ManagerData.Items.Count} "
                              + $"engine compositions={engine.Project.CompositionManager.ManagerData.Items.Count}");
        }

        private static string Addresses(Project project) =>
            string.Join(",", project.RedisAddressRegistry.Addresses.Select(a => a.Value));
    }
}
