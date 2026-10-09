// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // Shared source for the prefab list item DataTemplates previously duplicated in
    // RepositoryManager.axaml and PrefabSlotManager.axaml. The ordered template list
    // lives in PrefabListItemTemplates.axaml (declaration order matters, derived types
    // before base types); this class loads it once and exposes it as a single
    // IDataTemplate, following the precedent of Views\ViewModelToViewTemplate.cs.
    // Consumers add an instance to BaseControls:CMiXListBox.DataTemplates.
    public class PrefabListItemTemplates : IDataTemplate
    {
        private static readonly Lazy<DataTemplates> Templates = new(LoadTemplates);

        public bool Match(object? data)
        {
            return data != null && Templates.Value.Any(template => template.Match(data));
        }

        public Control? Build(object? data)
        {
            return Templates.Value.FirstOrDefault(template => template.Match(data))?.Build(data);
        }

        private static DataTemplates LoadTemplates()
        {
            var uri = new Uri("avares://CMiX.Studio.Avalonia/Views/Managers/PrefabListItemTemplates.axaml");
            return (DataTemplates)AvaloniaXamlLoader.Load(uri, null);
        }
    }
}
