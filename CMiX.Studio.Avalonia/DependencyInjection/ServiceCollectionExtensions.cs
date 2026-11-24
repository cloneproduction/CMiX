// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Notices;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.DependencyInjection
{
    // The object graph of the Studio. The app and the tests call it; a test registers its fakes after the call.
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCMiXStudio(this IServiceCollection services, string settingsPath, string shortcutsPath)
        {
            services.AddSingleton<Views.MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindowController>();
            services.AddSingleton<MainMenuViewModel>();
            services.AddSingleton<ProjectFileViewModel>();
            services.AddSingleton<SettingsSyncViewModel>();
            services.AddSingleton(ProjectSession.Create);
            services.AddSingleton<UnsavedChanges>();
            services.AddSingleton<IUnsavedChangesPrompt, UnsavedChangesPrompt>();
            services.AddSingleton<IMoveBlockedNotice, MoveBlockedNotice>();
            services.AddSingleton<IMessageNotice, MessageNotice>();
            services.AddSingleton<StatusNotice>();
            services.AddSingleton<IStatusNotice>(provider => provider.GetRequiredService<StatusNotice>());
            services.AddSingleton<PathDropRules>();
            services.AddSingleton<ProjectTree>();
            services.AddSingleton<PanelFocusCycle>();
            services.AddSingleton(_ => new StudioSettingsFile(settingsPath));
            services.AddSingleton<ICredentialStore, WindowsCredentialStore>();
            services.AddSingleton(provider =>
            {
                var state = new InterfaceState(provider.GetRequiredService<StudioSettingsFile>());
                state.Load();
                return state;
            });
            services.AddSingleton(_ =>
            {
                var shortcuts = new ShortcutMap(shortcutsPath);
                shortcuts.Load();
                return shortcuts;
            });
            services.AddSingleton<IDialogService>(provider => new DialogService(
                new MainWindowDialogManager(new EmptyViewLocator(), new DialogFactory()),
                viewModelFactory: type => provider.GetService(type)));
            services.AddSingleton<ManagerReorderServiceFactory>(
                _ => (data, move) => new ManagerReorderService(data, move));
            return services;
        }
    }
}
