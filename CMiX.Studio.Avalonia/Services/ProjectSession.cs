// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Threading.Tasks;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.Services
{
    // The running session: the project, its six repository lists, and the way a file replaces them.
    public sealed class ProjectSession
    {
        private readonly Project _project;
        private readonly UndoManager _undoManager;
        private readonly ISyncTarget _syncTarget;
        private readonly SyncPeer _sync;
        private readonly PrefabManager[] _lists;

        public ProjectSession(Project project,
                              PrefabManager textures,
                              PrefabManager entities,
                              PrefabManager cameras,
                              PrefabManager lights,
                              PrefabManager beats,
                              PrefabManager colorPalettes,
                              UndoManager undoManager,
                              ISyncTarget syncTarget,
                              SyncPeer sync)
        {
            _project = project;
            Textures = textures;
            Entities = entities;
            Cameras = cameras;
            Lights = lights;
            Beats = beats;
            ColorPalettes = colorPalettes;
            _undoManager = undoManager;
            _syncTarget = syncTarget;
            _sync = sync;
            _lists = new[] { textures, entities, cameras, lights, beats, colorPalettes };
        }

        // The six lists are transient in the container, so the session is the one place that holds the live ones.
        public static ProjectSession Create(IServiceProvider provider)
        {
            var session = ActivatorUtilities.CreateInstance<ProjectSession>(provider);
            session.Start();
            return session;
        }

        public PrefabManager Textures { get; }
        public PrefabManager Entities { get; }
        public PrefabManager Cameras { get; }
        public PrefabManager Lights { get; }
        public PrefabManager Beats { get; }
        public PrefabManager ColorPalettes { get; }

        private void Start()
        {
            SetupList(Textures, ManagerIDs.TextureManager);
            SetupList(Entities, ManagerIDs.EntityManager);
            SetupList(Cameras, ManagerIDs.CameraManager);
            SetupList(Lights, ManagerIDs.LightManager);
            SetupList(Beats, ManagerIDs.BeatManager);
            SetupList(ColorPalettes, ManagerIDs.ColorPaletteManager);
            _sync.SnapshotApplied += OnSnapshotApplied;
        }

        // The ids are on the wire and in the saved files. The lists are activated by hand, so ActivateAll never runs for them.
        private void SetupList(PrefabManager list, Guid id)
        {
            list.ManagerData.ID = id;
            list.UndoManager = _undoManager;
            list.Activate();
        }

        // A snapshot carries the project only. The undo manager stays untouched here, because ProjectSyncTarget clears it.
        private void OnSnapshotApplied(object? sender, EventArgs e) => ClearLists();

        public void ClearLists()
        {
            foreach (var list in _lists)
                list.ClearAll();
        }

        // The undo clear comes last: it releases the removed controls that its commands still hold.
        public void ClearAll()
        {
            _project.CompositionManager.ClearAll();
            ClearLists();
            _project.Reset();
            _undoManager.Clear();
        }

        public void NewProject() => ClearAll();

        // The file replaces the session. The apply sends no message and records no undo step.
        public void LoadProject(ProjectModel model)
        {
            ClearAll();
            _syncTarget.ApplySnapshot(model);
        }

        // A peer that is not joined adopts the store state when it joins. A push failure is a sync failure.
        public async Task PushToPeersAsync()
        {
            try
            {
                if (_sync.IsJoined && _sync.IsWriter)
                    await _sync.PushAsync();
            }
            catch (Exception ex)
            {
                _sync.ErrorMessage = ex.Message;
            }
        }

        public ProjectModel CaptureProject() => ProjectModelBuilder.Build(_project);

        public Task<ProjectModel> ReadAsync(string path) => Task.Run(() => ProjectSerializer.Load(path));

        public Task WriteAsync(ProjectModel model, string path) => Task.Run(() => ProjectSerializer.Save(model, path));
    }
}
